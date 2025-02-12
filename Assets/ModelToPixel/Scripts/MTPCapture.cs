using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

using System;

namespace ModelToPixel
{
    public class MTPCapture : MonoBehaviour
    {
        [HideInInspector]
        public GameObject target; // 要捕捉的3D模型
        public Camera captureCamera;
        public Camera previewRTCamera;
        public string outputFolder = "Assets/Sprites";
        public int textureWidth = 128;
        public int textureHeight = 128;
        public float normalIntensity = 0;
        [HideInInspector]
        public RenderTexture renderTexture;
        [HideInInspector]
        public RenderTexture effectRenderTexture;
        [HideInInspector]
        public RenderTexture previewRenderTexture;

        public Color effectBackgroundColor = Color.black; // 默认背景颜色设置
        public bool effectTransparency = true; // 默认透明设置
        public bool effectDoubleRes = false;

        private Animator targetAnimator;
        private Dictionary<Renderer, Material[]> originalMaterials; 
        private EffectDataCollection effectDataCollection;
        private Dictionary<GameObject, GameObject> activeEffects = new Dictionary<GameObject, GameObject>();
 
        private string animFilePath;
        private Animator effectAnimator;
        private Dictionary<ParticleSystem, ParticleSystem.Particle[]> particleCache = new Dictionary<ParticleSystem, ParticleSystem.Particle[]>();

        private GameObject effectInstance;
        private int effectResMul = 1;
  
        private Material tonemappingMaterial;

        public enum CaptureType
        {
            BaseColor,
            Normal,
            Emissive,
            Effect
        }

        public void SetupCapture()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");
            string tonemappingMaterialPath = Path.Combine(parentFolder,"Shaders/Custom_TonemapT3ACES.mat");
            tonemappingMaterial = AssetDatabase.LoadAssetAtPath<Material>(tonemappingMaterialPath);
            effectResMul = effectDoubleRes? 2 : 1;

            if (targetAnimator == null)
            {
                targetAnimator = target.GetComponent<Animator>();
            }

            SetRenderTextureAndCamera();

            EnableBloom(false);

            SaveOriginalMaterials();
            LoadEffectData();
        }

        public static string FindParentFolder(string currentPath, string targetFolderName)
        {
            string directory = Path.GetDirectoryName(currentPath);
            
            // 遍历向上查找父文件夹
            while (!string.IsNullOrEmpty(directory))
            {
                if (Path.GetFileName(directory) == targetFolderName)
                {
                    return directory;
                }
                directory = Path.GetDirectoryName(directory); // 向上移动一级
            }

            // 如果没有找到目标文件夹，返回null或抛出错误
            Debug.LogError($"无法找到名为 {targetFolderName} 的父文件夹！");
            return null;
        }

        public void SetupCaptureForBullet()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");
            string tonemappingMaterialPath = Path.Combine(parentFolder,"Shaders/Custom_TonemapT3ACES.mat");
            tonemappingMaterial = AssetDatabase.LoadAssetAtPath<Material>(tonemappingMaterialPath);
            effectResMul = 1;

            SetRenderTextureAndCamera();
            EnableBloom(false);
        }

        private void SetRenderTextureAndCamera()
        {
                        renderTexture = new RenderTexture(textureWidth * 1, textureHeight * 1, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point,
                antiAliasing = 1
            };

            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(
            textureWidth * effectResMul,
            textureHeight * effectResMul,
            RenderTextureFormat.ARGBFloat,
            24);

            descriptor.sRGB = false;  // 禁用 sRGB
            effectRenderTexture = new RenderTexture(descriptor)
            {
                filterMode = FilterMode.Point,
                antiAliasing = 1
            };

            // 设置相机参数
            captureCamera.targetTexture = renderTexture;
            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = Color.clear;
            captureCamera.allowHDR = true;
        }

        private void SaveOriginalMaterials()
        {
            originalMaterials = new Dictionary<Renderer, Material[]>();
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                originalMaterials[renderer] = renderer.materials;
            }
        }

        private void RestoreOriginalMaterials()
        {
            foreach (var pair in originalMaterials)
            {
                pair.Key.materials = pair.Value;
            }
        }

        private void ReplaceShaders(string shaderName)
        {
            Shader shader = Shader.Find(shaderName);
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    newMaterials[i] = new Material(shader);

                    // 复制原始材质的属性
                    newMaterials[i].CopyPropertiesFromMaterial(renderer.materials[i]);

                    // 设置新的属性值
                    newMaterials[i].SetFloat("_NormalIntensity", normalIntensity);
                }
                renderer.materials = newMaterials;
            }
        }

        public void ReplaceShadersForBaseColor()
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (renderer.materials[i].shader.name == "Smash/SH_DisneyLit_InGame")
                    {
                        renderer.materials[i].shader = Shader.Find("Unlit/Unlit_BaseColor");
                    }
                }
            }
        }

        public void ReplaceShadersForEmissive()
        {
            ReplaceShaders("Unlit/Unlit_Emissive");
        }

        public void ReplaceShadersForNormal()
        {
            ReplaceShaders("Unlit/Unlit_Normal");
        }

        public static void EnableBloom(bool flag)
        {
            //BloomRenderFeature.BloomEnabled = flag;
        }

        private void ClearActiveEffects()
        {
            foreach (var effect in activeEffects.Values)
            {
                Destroy(effect);
            }
            activeEffects.Clear();
        }

        public void CaptureFrame(Texture2D texture, int x, int y, CaptureType captureType)
        {
            Application.runInBackground = true;
            int currentWidth = textureWidth;
            int currentHeight = textureHeight;

            // 如果是特效类型，使用双倍分辨率的 RenderTexture
            if (captureType == CaptureType.Effect)
            {
                currentWidth = textureWidth * effectResMul;
                currentHeight = textureHeight * effectResMul;
                Shader.SetGlobalFloat("_AlphaPow",1);
            }

            RenderTexture.active = captureType == CaptureType.Effect ? effectRenderTexture : renderTexture;
            if (captureType == CaptureType.Effect)
            {
                captureCamera.cullingMask = LayerMask.GetMask("Effects");
                captureCamera.targetTexture = effectRenderTexture;
                captureCamera.Render();
            }
            else
            {
                captureCamera.cullingMask = ~0;
                captureCamera.targetTexture = renderTexture;
                captureCamera.Render();
            }

            // 创建高分辨率的临时Texture2D
            Texture2D highResTexture = new Texture2D(currentWidth, currentHeight, TextureFormat.RGBAFloat, false)
            {
                filterMode = FilterMode.Point
            };
            highResTexture.ReadPixels(new Rect(0, 0, currentWidth, currentHeight), 0, 0);
            highResTexture.Apply();

            Color[] pixels = highResTexture.GetPixels();

            if (captureType == CaptureType.Emissive)
            {
                // 将 R 和 B 通道清零，仅保留 G 通道
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(0f, pixels[i].g, 0f, pixels[i].a); // 只保留绿色通道
                }

                Shader.SetGlobalFloat("_AlphaPow", 0);
            }
    
            if (captureType == CaptureType.Effect)
            {
                AdjustColorAndAlpha(pixels);
                Shader.SetGlobalFloat("_AlphaPow",0);
            }
            texture.SetPixels(x, y, currentWidth, currentHeight, pixels);
            texture.Apply();

            DestroyImmediate(highResTexture);
            
            RenderTexture.active = null;
        }

        public void CaptureFrameForBullet(Texture2D texture, int x, int y)
        {
            Application.runInBackground = true;
            int currentWidth = textureWidth;
            int currentHeight = textureHeight;

            //Shader.SetGlobalFloat("_AlphaPow",1);

            RenderTexture.active = effectRenderTexture;
            captureCamera.targetTexture = effectRenderTexture;
            captureCamera.Render();

            // 创建高分辨率的临时Texture2D
            Texture2D highResTexture = new Texture2D(currentWidth, currentHeight, TextureFormat.RGBAFloat, false)
            {
                filterMode = FilterMode.Point
            };
            highResTexture.ReadPixels(new Rect(0, 0, currentWidth, currentHeight), 0, 0);
            highResTexture.Apply();

            Color[] pixels = highResTexture.GetPixels();

            AdjustColorAndAlpha(pixels);
            Shader.SetGlobalFloat("_AlphaPow",0);

            texture.SetPixels(x, y, currentWidth, currentHeight, pixels);
            texture.Apply();


            DestroyImmediate(highResTexture);
            
            RenderTexture.active = null;
        }

        private void AdjustColorAndAlpha(Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0.001f)
                {
                    float a = (float)Math.Pow(pixels[i].a, 0.5);

                    // 将颜色转换回 RGB 空间
                    Color color = pixels[i] / a;
                    color.r = Mathf.Pow(color.r, 1.0f / 2.2f); 
                    color.g = Mathf.Pow(color.g, 1.0f / 2.2f);  
                    color.b = Mathf.Pow(color.b, 1.0f / 2.2f);  
                    pixels[i] = color;
                    pixels[i].a = effectTransparency? pixels[i].a : 1.0f;
                }
            }
        }

        // 应用 Tonemapping 到 Texture2D
        public void ApplyTonemappingToTexture(Texture2D texture, Material tonemapMaterial)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGBFloat);

            Graphics.Blit(texture, tempRT);

            // 使用 Tonemapping 材质进行 Blit
            RenderTexture tonemappedRT = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(tempRT, tonemappedRT, tonemappingMaterial);

            RenderTexture.active = tonemappedRT;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            // 清理临时资源
            RenderTexture.ReleaseTemporary(tempRT);
            RenderTexture.ReleaseTemporary(tonemappedRT);
            RenderTexture.active = null;
        }

        private void ApplyOutlineToEmissive(Texture2D emissiveTexture, string clipName, int frameIndex)
        {
            string baseColorPath = $"{clipName}{frameIndex}.png";
            baseColorPath = baseColorPath.Substring(0, clipName.LastIndexOf("_M")) + "_D" + $"{frameIndex}.png";
            if (!File.Exists(baseColorPath))
            {
                Debug.LogError($"BaseColor frame not found: {baseColorPath}");
                return;
            }

            // 加载BaseColor帧
            byte[] fileData = File.ReadAllBytes(baseColorPath);
            Texture2D baseColorTexture = new Texture2D(emissiveTexture.width, emissiveTexture.height, TextureFormat.RGBA32, false);
            baseColorTexture.LoadImage(fileData);

            Color[] baseColors = baseColorTexture.GetPixels();
            Color[] emissiveColors = emissiveTexture.GetPixels();

            // 应用描边逻辑，将描边信息写入到 Emissive 贴图的红色通道
            for (int y = 0; y < baseColorTexture.height; y++)
            {
                for (int x = 0; x < baseColorTexture.width; x++)
                {
                    int index = y * baseColorTexture.width + x;
                    Color currentPixel = baseColors[index];
                    float outlineIntensity = 0f;

        if (x > 0 && x < baseColorTexture.width - 1 && y > 0 && y < baseColorTexture.height - 1)
        {
            // 检查相邻像素的 alpha 值，生成描边信息
            Color[] neighbors = {
                baseColors[index - 1],               // 左边的像素
                baseColors[index + 1],               // 右边的像素
                baseColors[index - baseColorTexture.width], // 上面的像素
                baseColors[index + baseColorTexture.width]  // 下面的像素
            };

            foreach (Color neighbor in neighbors)
            {
                outlineIntensity += Mathf.Max(0, neighbor.a - currentPixel.a);
            }
        }

                    emissiveColors[index].r = Mathf.Clamp01(outlineIntensity); // 将描边信息写入红色通道
                }
            }

            // 将计算好的颜色写回到Emissive贴图中
            emissiveTexture.SetPixels(emissiveColors);
            emissiveTexture.Apply();

            DestroyImmediate(baseColorTexture);
        }


        private Color GetPixelSafe(Color[] pixels, int width, int height, int x, int y, int originalX, int originalY)
        {
            if (x < 0 || x >= width - 1 || y < 0 || y >= height - 1)
            {
                return pixels[originalY * width + originalX];
            }
            return pixels[y * width + x];
        }
        
        private void ApplyOutlineToEmissive(Texture2D emissiveTexture, string clipName)
        {
            string baseColorPath = $"{clipName}.png";
            Debug.Log(baseColorPath);
            baseColorPath = baseColorPath.Substring(0, clipName.LastIndexOf("_M")) + "_D" + $".png";
            if (!File.Exists(baseColorPath))
            {
                Debug.LogError($"BaseColor frame not found: {baseColorPath}");
                return;
            }

            // 加载BaseColor帧
            byte[] fileData = File.ReadAllBytes(baseColorPath);
            Texture2D baseColorTexture = new Texture2D(emissiveTexture.width, emissiveTexture.height, TextureFormat.RGBA32, false);
            baseColorTexture.LoadImage(fileData);

            Color[] baseColors = baseColorTexture.GetPixels();
            Color[] emissiveColors = emissiveTexture.GetPixels();

            // 应用描边逻辑，将描边信息写入到 Emissive 贴图的红色通道
            for (int y = 0; y < baseColorTexture.height; y++)
            {
                for (int x = 0; x < baseColorTexture.width; x++)
                {
                    int index = y * baseColorTexture.width + x;
                    Color currentPixel = baseColors[index];
                    float outlineIntensity = 0f;

                    // 检查相邻像素的alpha值，生成描边信息
                    Color[] neighbors = {
                        GetPixelSafe(baseColors, baseColorTexture.width, baseColorTexture.height, x - 1, y, x, y), // 左
                        GetPixelSafe(baseColors, baseColorTexture.width, baseColorTexture.height, x + 1, y, x, y), // 右
                        GetPixelSafe(baseColors, baseColorTexture.width, baseColorTexture.height, x, y - 1, x, y), // 上
                        GetPixelSafe(baseColors, baseColorTexture.width, baseColorTexture.height, x, y + 1, x, y)  // 下
                    };

                    foreach (Color neighbor in neighbors)
                    {
                        outlineIntensity += Mathf.Max(0, neighbor.a - currentPixel.a);
                    }

                    emissiveColors[index].r = Mathf.Clamp01(outlineIntensity); // 将描边信息写入红色通道
                }
            }

            // 将计算好的颜色写回到Emissive贴图中
            emissiveTexture.SetPixels(emissiveColors);
            emissiveTexture.Apply();

            DestroyImmediate(baseColorTexture);
        }

        public IEnumerator CaptureAnimations(AnimationClip clip, Animator animator, int frameRate, CaptureType captureType)
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(overrides);

            foreach (var item in overrides)
            {
                if (item.Key != null)
                {
                    overrideController[item.Key] = clip;
                }
            }
            animator.runtimeAnimatorController = overrideController;

            int totalFrames = Mathf.FloorToInt(clip.length * frameRate);
            int rows = Mathf.CeilToInt(Mathf.Sqrt(totalFrames));
            int cols = Mathf.CeilToInt(totalFrames / (float)rows);
            int frameWidth = textureWidth;
            int frameHeight = textureHeight;

            TextureFormat textureFormat = (captureType == CaptureType.Normal||captureType == CaptureType.Emissive) ? TextureFormat.RGB48 : TextureFormat.RGBAFloat;

            int currentFrame = 0;

            // 检查是否有特效数据，如果没有就跳过特效捕捉
            bool hasEffect = effectDataCollection != null && effectDataCollection.effects.Exists(e => e.animationClip == clip);
            if (!hasEffect && captureType == CaptureType.Effect)
            {
                yield break; // 跳过特效捕捉
            }
            SetupCaptureEnvironment(clip, frameWidth, frameHeight, captureType);

            Texture2D finalTexture = new Texture2D(frameWidth * cols, frameHeight * rows, textureFormat, false)
            {
                filterMode = FilterMode.Point
            };

            string prefabName = target.name;
            string animName = targetAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            animName = $"{animName}";
            string directoryPath = Path.Combine(outputFolder, prefabName, animName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        
            string suffix = captureType == CaptureType.BaseColor ? "_D" : captureType == CaptureType.Normal ? "_N" : captureType == CaptureType.Emissive ? "_M" : "_E";
            string filePath = $"{directoryPath}/{animName}{suffix}.png";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (currentFrame >= totalFrames)
                        break;

                    float normalizedTime = (float)currentFrame / (totalFrames - 1);
                    animator.Play(0, 0, normalizedTime);

                    if (captureType == CaptureType.Effect)
                    {
                        HandleEffects(clip, currentFrame, totalFrames, frameRate);
                    }
                    yield return new WaitForEndOfFrame();
                    

                    CaptureFrame(finalTexture, j * frameWidth, (rows - i - 1) * frameHeight,captureType);
                    Texture2D frameTexture = new Texture2D(frameWidth, frameHeight, textureFormat, false);
                    CaptureFrame(frameTexture, 0, 0, captureType);

                    if (captureType == CaptureType.Emissive)
                    {
                        // 读取之前保存的 BaseColor 文件并生成描边
                        ApplyOutlineToEmissive(frameTexture, filePath, currentFrame);
                    }

                    ExportSingleFrame(frameTexture, currentFrame, captureType, clip.name);
                    DestroyImmediate(frameTexture);
                    currentFrame++;
                }
            }
            if (captureType == CaptureType.Emissive)
            {
                ApplyOutlineToEmissive(finalTexture, filePath);
            }

            if(captureType == CaptureType.Effect)
                animName = $"{animName}_effect";
            byte[] bytes = finalTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            DestroyImmediate(finalTexture);

            RestoreOriginalMaterials();

            AssetDatabase.Refresh();
            // 设置TextureImporter属性
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 8192;
                importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = 32;
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                if (captureType == CaptureType.Normal || captureType == CaptureType.Emissive)
                {
                    importer.alphaSource = 0;
                    importer.sRGBTexture = false;
                }

                // 创建切片信息
                List<SpriteMetaData> spriteMetaData = new List<SpriteMetaData>();
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if ((i * cols + j) >= totalFrames)
                            break;

                        SpriteMetaData metaData = new SpriteMetaData
                        {
                            name = $"{captureType}_{i * cols + j}",
                            rect = new Rect(j * frameWidth, (rows - i - 1) * frameHeight, frameWidth, frameHeight),
                            alignment = 0,
                            pivot = new Vector2(0.5f, 0)
                        };
                        spriteMetaData.Add(metaData);
                    }
                }
                SerializedObject serializedObject = new SerializedObject(importer);

            // 找到包含 Sprite 数据的属性
            SerializedProperty spriteSheetProperty = serializedObject.FindProperty("m_SpriteSheet.m_Sprites");

            // 清空现有的 spritesheet 数据
            spriteSheetProperty.ClearArray();

            // 设置新的 spriteMetaData
            for (int i = 0; i < spriteMetaData.Count; i++)
            {
                spriteSheetProperty.InsertArrayElementAtIndex(i);
                SerializedProperty element = spriteSheetProperty.GetArrayElementAtIndex(i);

                element.FindPropertyRelative("m_Rect").rectValue = spriteMetaData[i].rect;
                element.FindPropertyRelative("m_Name").stringValue = spriteMetaData[i].name;
                element.FindPropertyRelative("m_Alignment").intValue = (int)spriteMetaData[i].alignment;
                element.FindPropertyRelative("m_Pivot").vector2Value = spriteMetaData[i].pivot;
                element.FindPropertyRelative("m_Border").vector4Value = spriteMetaData[i].border;
            }

            // 应用更改并重新导入纹理
            serializedObject.ApplyModifiedProperties();
                
                importer.SaveAndReimport();

                // 创建动画
                if (captureType == CaptureType.BaseColor||captureType == CaptureType.Effect)
                {
                    CreateAnimationFromSprites(importer, directoryPath, animName, frameRate);
                }
            }
            ClearActiveEffects();
        }

        private void SetupCaptureEnvironment(AnimationClip clip, int frameWidth, int frameHeight, CaptureType captureType)
        {
            if (captureType == CaptureType.BaseColor)
            {
                Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
                ReplaceShadersForBaseColor();
                captureCamera.backgroundColor = Color.clear;
            }
            
            else if (captureType == CaptureType.Emissive)
            {
                Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
                ReplaceShadersForEmissive();
                captureCamera.backgroundColor = Color.clear;
            }

            else if (captureType == CaptureType.Normal)
            {
                ReplaceShadersForNormal();
                captureCamera.backgroundColor = new Color(0.5f, 0.5f, 1.0f, 1.0f);
            }
            else if (captureType == CaptureType.Effect)
            {
                frameWidth *= effectResMul;
                frameHeight *=effectResMul;
                captureCamera.backgroundColor = Color.clear;
            }
        }

        public void CaptureFrameToRT(RenderTexture targetRT, CaptureType captureType)
        {
            captureCamera.cullingMask = ~0;

            if (target == null || captureCamera == null)
            {
                Debug.LogError("目标对象或捕捉相机未设置！");
                return;
            }

            // 设置捕捉相机目标
            captureCamera.targetTexture = targetRT;

            // 根据类型切换 Shader
            switch (captureType)
            {
                case CaptureType.BaseColor:
                    ReplaceShadersForBaseColor();
                    break;
                case CaptureType.Normal:
                    ReplaceShadersForNormal();
                    break;
                case CaptureType.Emissive:
                    ReplaceShadersForEmissive();
                    break;
                default:
                    Debug.LogError($"未知的捕捉类型：{captureType}");
                    return; ;
            }

            // 渲染目标
            captureCamera.Render();

            RestoreOriginalMaterials();

            // 清理相机目标
            captureCamera.targetTexture = null;
        }

        public IEnumerator CaptureAnimationsForBullet(GameObject effectInstance, int frameRate)
        {
            float psLength = MTPBulletEffect.GetTotalFrame(effectInstance);
            int totalFrames = Mathf.FloorToInt(psLength * frameRate);

            int rows = Mathf.CeilToInt(Mathf.Sqrt(totalFrames));
            int cols = Mathf.CeilToInt(totalFrames / (float)rows);
            int frameWidth = textureWidth;
            int frameHeight = textureHeight;

            TextureFormat textureFormat =  TextureFormat.RGBAFloat;

            int currentFrame = 0;
            captureCamera.backgroundColor = Color.clear;  

            Texture2D finalTexture = new Texture2D(frameWidth * cols, frameHeight * rows, textureFormat, false)
            {
                filterMode = FilterMode.Point
            };

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (currentFrame >= totalFrames)
                        break;

                    float normalizedTime = (float)currentFrame / (totalFrames - 1);

                    if(effectAnimator != null)
                    {
                        AnimationClip effectClip = effectAnimator.runtimeAnimatorController.animationClips[0];
                        float effectNormalizedTime = (float)(currentFrame) / totalFrames;
                        effectAnimator.Play(0, 0, effectNormalizedTime);
                        effectAnimator.speed = 0;
                    }
                    // 处理粒子系统
                    ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                    if(particleSystems != null)
                    {
                        foreach (ParticleSystem ps in particleSystems)
                        {
                            if (!particleCache.ContainsKey(ps))
                            {
                                particleCache[ps] = new ParticleSystem.Particle[ps.main.maxParticles];
                            }
                            // 计算粒子系统的 normalizedTime
                            float particleSystemNormalizedTime = (float)(currentFrame) / (frameRate);

                            particleSystemNormalizedTime = Mathf.Clamp01(particleSystemNormalizedTime);
                            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                            ps.useAutoRandomSeed = false;
                            ps.Simulate(particleSystemNormalizedTime , true, true);
                            ps.Pause();
                        }
                    }

                
                    yield return new WaitForEndOfFrame();
                    

                    CaptureFrameForBullet(finalTexture, j * frameWidth, (rows - i - 1) * frameHeight);
                    Texture2D frameTexture = new Texture2D(frameWidth, frameHeight, textureFormat, false);
                    CaptureFrameForBullet(frameTexture, 0, 0);
                    ApplyTonemappingToTexture(frameTexture, tonemappingMaterial);
                    

                    ExportSingleFrameForBullet(frameTexture, currentFrame, effectInstance.name);
                    DestroyImmediate(frameTexture);
                    currentFrame++;
                }
            }

            ApplyTonemappingToTexture(finalTexture, tonemappingMaterial);
            
            string animName = effectInstance.name;
            animName = $"{animName}";
            string directoryPath = Path.Combine(outputFolder, animName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        
            finalTexture.filterMode = FilterMode.Point;
            byte[] bytes = finalTexture.EncodeToPNG();
            string suffix = "_E";
            string filePath = $"{directoryPath}/{animName}{suffix}.png";
            animName = $"{animName}_bulletEffect";
            File.WriteAllBytes(filePath, bytes);
            DestroyImmediate(finalTexture);

            AssetDatabase.Refresh();
            // 设置TextureImporter属性
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 8192;
                importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = 32;
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);

                // 创建切片信息
                List<SpriteMetaData> spriteMetaData = new List<SpriteMetaData>();
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if ((i * cols + j) >= totalFrames)
                            break;

                        SpriteMetaData metaData = new SpriteMetaData
                        {
                            name = $"effect_{i * cols + j}",
                            rect = new Rect(j * frameWidth, (rows - i - 1) * frameHeight, frameWidth, frameHeight),
                            alignment = 0,
                            pivot = new Vector2(0.5f, 0)
                        };
                        spriteMetaData.Add(metaData);
                    }
                }
                SerializedObject serializedObject = new SerializedObject(importer);

                // 找到包含 Sprite 数据的属性
                SerializedProperty spriteSheetProperty = serializedObject.FindProperty("m_SpriteSheet.m_Sprites");

                // 清空现有的 spritesheet 数据
                spriteSheetProperty.ClearArray();

                // 设置新的 spriteMetaData
                for (int i = 0; i < spriteMetaData.Count; i++)
                {
                    spriteSheetProperty.InsertArrayElementAtIndex(i);
                    SerializedProperty element = spriteSheetProperty.GetArrayElementAtIndex(i);

                    element.FindPropertyRelative("m_Rect").rectValue = spriteMetaData[i].rect;
                    element.FindPropertyRelative("m_Name").stringValue = spriteMetaData[i].name;
                    element.FindPropertyRelative("m_Alignment").intValue = (int)spriteMetaData[i].alignment;
                    element.FindPropertyRelative("m_Pivot").vector2Value = spriteMetaData[i].pivot;
                    element.FindPropertyRelative("m_Border").vector4Value = spriteMetaData[i].border;
                }

                // 应用更改并重新导入纹理
                serializedObject.ApplyModifiedProperties();
                importer.SaveAndReimport();


                CreateAnimationFromSprites(importer, directoryPath, animName, frameRate);
            }
            ClearActiveEffects();
        }

        private void ExportSingleFrame(Texture2D texture, int frameNumber, CaptureType captureType, string clipName)
        {
            string directoryPath = Path.Combine(outputFolder, target.name, $"{clipName}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create a Sprite directly from the Texture2D
            Sprite sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0f), 
                32, 
                0, 
                SpriteMeshType.FullRect
            );

            // Optionally, you can now use this sprite or save the texture as before
            byte[] bytes = texture.EncodeToPNG();
            string suffix = captureType == CaptureType.BaseColor ? "_D" : captureType == CaptureType.Normal ? "_N" : captureType == CaptureType.Emissive ? "_M" : "_E";
            string filePath = $"{directoryPath}/{clipName}{suffix}{frameNumber}.png";
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();

            // Optionally configure and reimport as a sprite (if you still need this part)
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 8192;
                importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.textureType = TextureImporterType.Sprite;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = 32;
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                if (captureType == CaptureType.Normal)
                {
                    importer.alphaSource = 0;
                    importer.sRGBTexture = false;
                }
                importer.SaveAndReimport();
            }
        }
        private void ExportSingleFrameForBullet(Texture2D texture, int frameNumber, string clipName)
        {
            string directoryPath = Path.Combine(outputFolder, $"{clipName}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create a Sprite directly from the Texture2D
            Sprite sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0f), 
                32, 
                0, 
                SpriteMeshType.FullRect
            );

            // Optionally, you can now use this sprite or save the texture as before
            byte[] bytes = texture.EncodeToPNG();
            string suffix = "_E";
            string filePath = $"{directoryPath}/{clipName}{suffix}{frameNumber}.png";
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();

            // Optionally configure and reimport as a sprite (if you still need this part)
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 8192;
                importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.textureType = TextureImporterType.Sprite;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = 32;
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }
            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        private void HandleEffects(AnimationClip clip, int currentFrame, int totalFrames, int frameRate)
        {
            if (effectDataCollection == null) return;

            foreach (var effectData in effectDataCollection.effects)
            {
                if (effectData.animationClip == clip)
                {
                    float startTime = (float)effectData.startFrame / totalFrames;
                    float endTime = (float)effectData.endFrame / totalFrames;
                    float normalizedTime = (float)currentFrame / (totalFrames - 1);

                    if (normalizedTime >= startTime && normalizedTime <= endTime)
                    {
                        if (!activeEffects.ContainsKey(effectData.effectPrefab))
                        {
                            Transform bone = target.transform.Find(effectData.bonePath);
                            if (bone != null)
                            {
                                effectInstance = Instantiate(effectData.effectPrefab, bone);
                                effectInstance.transform.localPosition = effectData.offset;
                                effectInstance.transform.localScale = effectData.scale;
                                effectInstance.SetActive(true);
                                SetLayerRecursively(effectInstance,LayerMask.NameToLayer("Effects"));
                                effectAnimator = effectInstance.GetComponent<Animator>();

                                activeEffects[effectData.effectPrefab] = effectInstance;
                            }
                        }
                        if (effectAnimator != null)
                        {
                            AnimationClip effectClip = effectAnimator.runtimeAnimatorController.animationClips[0];
                            int effectTotalFrames = Mathf.FloorToInt(effectClip.length * frameRate);
                            float effectNormalizedTime = (float)(currentFrame - effectData.startFrame) / effectTotalFrames;
                            effectAnimator.Play(0, 0, effectNormalizedTime);
                        }
                        ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                        foreach (ParticleSystem ps in particleSystems)
                        {
                            if (!particleCache.ContainsKey(ps))
                            {
                                particleCache[ps] = new ParticleSystem.Particle[ps.main.maxParticles];
                            }
                            // 计算粒子系统的 normalizedTime
                            float particleSystemNormalizedTime = (float)(currentFrame - effectData.startFrame) / (frameRate);
                            ps.Simulate(particleSystemNormalizedTime, true, true);
                            ps.Pause();
                        }
                    }
                    else
                    {
                        if (activeEffects.ContainsKey(effectData.effectPrefab))
                        {
                            Destroy(activeEffects[effectData.effectPrefab]);
                            activeEffects.Remove(effectData.effectPrefab);
                            effectInstance = null;
                        }
                    }
                }
            }
        }

        private void LoadEffectData()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");
            string effectDataPath = $"{parentFolder}/Effect/EffectDataCollection.asset";
            effectDataCollection = AssetDatabase.LoadAssetAtPath<EffectDataCollection>(effectDataPath);

            if (effectDataCollection == null)
            {
                Debug.LogError("EffectDataCollection asset not found at path: " + effectDataPath);
            }
        }

        private void CreateAnimationFromSprites(TextureImporter importer, string directoryPath, string animName, int frameRate)
        {
            string spriteSheetPath = importer.assetPath;
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
            List<Sprite> sprites = new List<Sprite>();

            // 遍历所有加载的资源，筛选出所有Sprite对象
            foreach (UnityEngine.Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    sprites.Add(sprite);
                }
            }
            
            if (sprites.Count == 0)
            {
                Debug.LogError("No sprites found in the spritesheet.");
                return;
            }

            AnimationClip animClip = new AnimationClip
            {
                frameRate = frameRate
            };
            
            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = i / (float)frameRate,
                    value = sprites[i]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, keyFrames);

            animFilePath = Path.Combine(directoryPath, $"{animName}.anim");
            AssetDatabase.CreateAsset(animClip, animFilePath);
            AssetDatabase.SaveAssets();
        }

        public IEnumerator CaptureBothAnimations(List<AnimationClip> clips, Animator animator, int frameRate)
        {
            foreach (var clip in clips)
            {
                if(clip != null)
                {
                    yield return StartCoroutine(CaptureAnimations(clip, animator, frameRate, CaptureType.BaseColor));
                    yield return StartCoroutine(CaptureAnimations(clip, animator, frameRate, CaptureType.Normal));
                    yield return StartCoroutine(CaptureAnimations(clip, animator, frameRate, CaptureType.Effect));
                    yield return StartCoroutine(CaptureAnimations(clip, animator, frameRate, CaptureType.Emissive));
                }

            }
        }

        public IEnumerator CaptureAnimationsForBullet(int frameRate, GameObject effectInstance)
        {
            yield return StartCoroutine(CaptureAnimationsForBullet(effectInstance, frameRate));
            
        }
    }
}