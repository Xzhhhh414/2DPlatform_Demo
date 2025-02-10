using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;

namespace ModelToPixel
{
    public class MTPPreviewManager : MonoBehaviour
    {
        private static Dictionary<string, bool> originalLoopStates = new Dictionary<string, bool>();
        private static Material lightingMaterial;
        private static Material bulletMaterial;
        private static Material tonemappingMaterial;

        private static List<Texture2D> ApplyShaderToFrames(List<Sprite> baseColorFrames, List<Sprite> normalFrames, List<Sprite> effectFrames, List<Sprite> mixFrames, Material lightingMaterial)
        {
            List<Texture2D> resultTextures = new List<Texture2D>();

            for (int i = 0; i < baseColorFrames.Count; i++)
            {
                RenderTexture rt = new RenderTexture(baseColorFrames[i].texture.width, baseColorFrames[i].texture.height, 24, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;

                lightingMaterial.SetTexture("_MainTex", baseColorFrames[i].texture);
                lightingMaterial.SetTexture("_NormalMap", normalFrames[i].texture);
                lightingMaterial.SetTexture("_MixTex", mixFrames[i].texture);
                if(effectFrames.Count == baseColorFrames.Count)
                {
                    lightingMaterial.SetTexture("_EffectMap", effectFrames[i].texture);
                }
                else
                {
                    lightingMaterial.SetTexture("_EffectMap", null);
                }

                Graphics.Blit(null, rt, lightingMaterial);

                
            // 确保只有有效帧被渲染
            if (normalFrames[i].textureRect.width > 0 && normalFrames[i].textureRect.height > 0)
            {
                Texture2D resultTexture = new Texture2D((int)normalFrames[i].textureRect.width, (int)normalFrames[i].textureRect.height, TextureFormat.RGBA32, false);
                
                // 反转 Y 轴的顺序
                resultTexture.ReadPixels(new Rect(
                    normalFrames[i].textureRect.x, 
                    normalFrames[i].textureRect.y,
                    normalFrames[i].textureRect.width, 
                    normalFrames[i].textureRect.height), 
                    0, 
                    0);
                
                resultTexture.Apply();
                resultTextures.Add(resultTexture);
            }

                RenderTexture.active = null;
                rt.Release();
            }

            return resultTextures;
        }

        public static Texture2D DrawMaterialPreview(Material previewMaterial, RenderTexture baseColorRT, RenderTexture normalRT, RenderTexture mixRT, int textureWidth, int textureHeight, string scriptPath)
        {
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string parentFolder = MTPCapture.FindParentFolder(scriptPath, "ModelToPixel");
            string tonemappingMaterialPath = Path.Combine(parentFolder,"Shaders/Custom_StaticPreviewTonemapping.mat");
            tonemappingMaterial = AssetDatabase.LoadAssetAtPath<Material>(tonemappingMaterialPath);

            // 配置材质的贴图参数
            previewMaterial.SetTexture("_MainTex", baseColorRT);
            previewMaterial.SetTexture("_NormalMap", normalRT);
            previewMaterial.SetTexture("_MixTex", mixRT);

            // 使用 RenderTexture 显示材质效果
            RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
            RenderTexture.active = rt;

            Graphics.Blit(null, rt, previewMaterial);

            RenderTexture tonemappedRT = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(rt, tonemappedRT, tonemappingMaterial);

            Texture2D resultTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            resultTexture.filterMode = FilterMode.Point;
            RenderTexture.active = tonemappedRT;
            resultTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
            resultTexture.Apply();

            // 释放 RenderTexture
            RenderTexture.ReleaseTemporary(tonemappedRT);
            RenderTexture.active = null;
            rt.Release();
            return resultTexture;
        }
        private static List<Texture2D> ApplyShaderToFramesForBullet(List<Sprite> effectFrames, Material lightingMaterial)
        {
            List<Texture2D> resultTextures = new List<Texture2D>();

            for (int i = 0; i < effectFrames.Count; i++)
            {
                RenderTexture rt = new RenderTexture(effectFrames[i].texture.width, effectFrames[i].texture.height, 24, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;

                lightingMaterial.SetTexture("_MainTex", effectFrames[i].texture);

                Graphics.Blit(null, rt, lightingMaterial);

                
            // 确保只有有效帧被渲染
            if (effectFrames[i].textureRect.width > 0 && effectFrames[i].textureRect.height > 0)
            {
                Texture2D resultTexture = new Texture2D((int)effectFrames[i].textureRect.width, (int)effectFrames[i].textureRect.height, TextureFormat.RGBA32, false);
                
                // 反转 Y 轴的顺序
                resultTexture.ReadPixels(new Rect(
                    effectFrames[i].textureRect.x, 
                    effectFrames[i].textureRect.y,
                    effectFrames[i].textureRect.width, 
                    effectFrames[i].textureRect.height), 
                    0, 
                    0);
                
                resultTexture.Apply();
                resultTextures.Add(resultTexture);
            }

                RenderTexture.active = null;
                rt.Release();
            }

            return resultTextures;
        }

        public static void PreviewAnimation(string animPath, ref SpriteRenderer previewSprite, ref SpriteRenderer previewSprite_effect, ref AnimationClip previewClip,
         ref string basePath, ref string normalPath, ref string effectPath, ref string mixPath)
        {
            previewSprite.enabled = true;
            // 加载 base color 动画
            string baseColorAnimPath = animPath;
            AnimationClip baseColorClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(baseColorAnimPath);
            previewClip = baseColorClip;
            if (baseColorClip == null)
            {
                Debug.LogError("Failed to load base color animation clip.");
                return;
            }

            // 设置 base color 动画到 previewSprite
            Animator baseAnimator = previewSprite.GetComponent<Animator>();
            if (baseAnimator == null)
            {
                baseAnimator = previewSprite.gameObject.AddComponent<Animator>();
            }

            // 保存原始循环状态
            if (!originalLoopStates.ContainsKey(baseColorAnimPath))
            {
                originalLoopStates[baseColorAnimPath] = baseColorClip.isLooping;
            }

            var serializedBaseClip = new SerializedObject(baseColorClip);
            SerializedProperty baseSettings = serializedBaseClip.FindProperty("m_AnimationClipSettings");
            SerializedProperty baseLoopTime = baseSettings.FindPropertyRelative("m_LoopTime");
            if(!baseLoopTime.boolValue)
            {
                baseLoopTime.boolValue = true;
                serializedBaseClip.ApplyModifiedProperties();
            }
            serializedBaseClip.ApplyModifiedProperties();

            AnimatorOverrideController baseAOC = new AnimatorOverrideController(baseAnimator.runtimeAnimatorController);
            var baseOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            baseAOC.GetOverrides(baseOverrides);
            foreach (var item in baseOverrides)
            {
                if (item.Key != null)
                {
                    baseAOC[item.Key] = baseColorClip;
                }
            }
            baseAnimator.runtimeAnimatorController = baseAOC;
            baseAnimator.Play(0, 0, 0);

            normalPath = Path.Combine(Path.GetDirectoryName(baseColorAnimPath), Path.GetFileNameWithoutExtension(baseColorAnimPath) + "_N.png");
            basePath = Path.Combine(Path.GetDirectoryName(baseColorAnimPath), Path.GetFileNameWithoutExtension(baseColorAnimPath) + "_D.png");
            effectPath = Path.Combine(Path.GetDirectoryName(baseColorAnimPath), Path.GetFileNameWithoutExtension(baseColorAnimPath) + "_E.png");
            mixPath = Path.Combine(Path.GetDirectoryName(baseColorAnimPath), Path.GetFileNameWithoutExtension(baseColorAnimPath) + "_M.png");

            // 加载 BaseColor 和 Effect 贴图
            Texture2D baseColorTex = AssetDatabase.LoadAssetAtPath<Texture2D>(basePath);
            Debug.Log(baseColorTex);
            Texture2D effectTex = AssetDatabase.LoadAssetAtPath<Texture2D>(effectPath);

            // 检查特效贴图的尺寸是否是 BaseColor 的两倍
            if (effectTex != null && baseColorTex != null)
            {
                float scale = 1f;
                if (effectTex.width == baseColorTex.width * 2 && effectTex.height == baseColorTex.height * 2)
                {
                    scale = 0.5f; // 如果特效贴图是 BaseColor 的两倍，缩放至 1/2
                }

                // 设置特效 SpriteRenderer 的缩放
                previewSprite_effect.transform.localScale = new Vector3(scale, scale, 1f);
            }

            //替换材质法线和mix贴图
            Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
            if (normalTex != null)
            {
                Material spriteMat = previewSprite.GetComponent<SpriteRenderer>().material;
                if (spriteMat != null)
                {
                    spriteMat.SetTexture("_NormalMap", normalTex);
                }
            }
            Texture2D mixTex = AssetDatabase.LoadAssetAtPath<Texture2D>(mixPath);
            if (mixTex != null)
            {
                Material spriteMat = previewSprite.GetComponent<SpriteRenderer>().material;
                if (spriteMat != null)
                {
                    spriteMat.SetTexture("_MixTex", mixTex);
                }
            }

            //用于显示单帧图像
            // 加载 BaseColor 和 NormalMap 的 Frames
            List<Sprite> baseColorFrames = LoadAndSliceTexture(basePath);
            List<Sprite> normalFrames = LoadAndSliceTexture(normalPath);
            List<Sprite> effectFrames = LoadAndSliceTexture(effectPath);
            List<Sprite> mixFrames = LoadAndSliceTexture(mixPath);

            if (lightingMaterial == null)
            {
                Shader lightingShader = Shader.Find("CRLuo/SpritePreview");
                lightingMaterial = new Material(lightingShader);
            }
            // 应用 Shader 处理后的结果
            List<Texture2D> processedFrames = ApplyShaderToFrames(baseColorFrames, normalFrames, effectFrames, mixFrames, lightingMaterial);
            // 释放之前的 RenderTexture
            ModelToPixelEditorWindow.ReleaseRenderTextures();

            // 创建新的 RenderTexture 列表
            foreach (var frame in processedFrames)
            {
                RenderTexture rt = new RenderTexture(frame.width, frame.height, 24, RenderTextureFormat.ARGB32);
                Graphics.Blit(frame, rt);
                ModelToPixelEditorWindow.previewRenderTextures.Add(rt);
            }

            //effect部分
            // 加载 effect 动画
            string effectAnimPath = baseColorAnimPath.Replace(".anim", "_effect.anim");
            AnimationClip effectClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(effectAnimPath);
            if (effectClip == null)
            {
                previewSprite_effect.enabled = false;
                AdjustCameraToSprite(previewSprite, previewSprite_effect, false);
                return;
            }

            previewSprite_effect.enabled = true;
            // 设置 effect 动画到 previewSprite_effect
            Animator effectAnimator = previewSprite_effect.GetComponent<Animator>();
            if (effectAnimator == null)
            {
                effectAnimator = previewSprite_effect.gameObject.AddComponent<Animator>();
            }

            if (!originalLoopStates.ContainsKey(effectAnimPath))
            {
                originalLoopStates[effectAnimPath] = effectClip.isLooping;
            }

            var serializedEffectClip = new SerializedObject(effectClip);
            SerializedProperty effectSettings = serializedEffectClip.FindProperty("m_AnimationClipSettings");
            SerializedProperty effectLoopTime = effectSettings.FindPropertyRelative("m_LoopTime");
            if(!effectLoopTime.boolValue)
            {
                effectLoopTime.boolValue = true;
                serializedEffectClip.ApplyModifiedProperties();
            }

            AnimatorOverrideController effectAOC = new AnimatorOverrideController(effectAnimator.runtimeAnimatorController);
            var effectOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(baseAOC.overridesCount);
            effectAOC.GetOverrides(effectOverrides);
            foreach (var item in effectOverrides)
            {
                if (item.Key != null)
                {
                    effectAOC[item.Key] = effectClip;
                }
            }
            effectAnimator.runtimeAnimatorController = effectAOC;
            effectAnimator.Play(0, 0, 0);
            AdjustCameraToSprite(previewSprite, previewSprite_effect, true);
            ReleaseTextures(processedFrames);
        }

        public static void PreviewBulletAnimation(string animPath, ref SpriteRenderer previewSprite_effect, ref AnimationClip previewClip, ref string effectPath)
        {
            previewSprite_effect.enabled = true;

            effectPath = Path.Combine(Path.GetDirectoryName(animPath), Path.GetFileNameWithoutExtension(animPath));
            effectPath = effectPath.Replace("_bulletEffect","_E.png");
            List<Sprite> effectFrames = LoadAndSliceTexture(effectPath);

            if (bulletMaterial == null)
            {
                Shader bulletShader = Shader.Find("CRLuo/SpriteEffect");
                bulletMaterial = new Material(bulletShader);
            }
            // 应用 Shader 处理后的结果
            List<Texture2D> processedFrames = ApplyShaderToFramesForBullet(effectFrames, bulletMaterial);
            // 释放之前的 RenderTexture
            ModelToPixelEditorWindow.ReleaseRenderTextures();

            // 创建新的 RenderTexture 列表
            foreach (var frame in processedFrames)
            {
                RenderTexture rt = new RenderTexture(frame.width, frame.height, 24, RenderTextureFormat.ARGB32);
                Graphics.Blit(frame, rt);
                ModelToPixelEditorWindow.previewRenderTextures.Add(rt);
            }

            // 加载 effect 动画
            AnimationClip effectClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);

            previewSprite_effect.enabled = true;
            // 设置 effect 动画到 previewSprite_effect
            Animator effectAnimator = previewSprite_effect.GetComponent<Animator>();
            if (effectAnimator == null)
            {
                effectAnimator = previewSprite_effect.gameObject.AddComponent<Animator>();
            }

            var serializedEffectClip = new SerializedObject(effectClip);
            SerializedProperty effectSettings = serializedEffectClip.FindProperty("m_AnimationClipSettings");
            SerializedProperty effectLoopTime = effectSettings.FindPropertyRelative("m_LoopTime");
            if(!effectLoopTime.boolValue)
            {
                effectLoopTime.boolValue = true;
                serializedEffectClip.ApplyModifiedProperties();
            }

            AnimatorOverrideController effectAOC = new AnimatorOverrideController(effectAnimator.runtimeAnimatorController);
            var effectOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(effectAOC.overridesCount);
            effectAOC.GetOverrides(effectOverrides);
            foreach (var item in effectOverrides)
            {
                if (item.Key != null)
                {
                    effectAOC[item.Key] = effectClip;
                }
            }
            effectAnimator.runtimeAnimatorController = effectAOC;
            effectAnimator.Play(0, 0, 0);
            AdjustCameraToSprite(previewSprite_effect, previewSprite_effect, true);
            ReleaseTextures(processedFrames);
        }

        private static void AdjustCameraToSprite(SpriteRenderer previewSprite, SpriteRenderer previewSprite_effect, bool hasEffect)
        {
            // if (previewSprite != null && ModelToPixelEditorWindow.previewRTCamera != null && previewSprite_effect != null)
            // {
            //     Bounds bounds = previewSprite.bounds;
            //     Bounds effectBounds = previewSprite_effect.bounds;
            //     float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y);
            //     Debug.Log(bounds);
            //     float effectMaxExtent = Mathf.Max(effectBounds.extents.x, effectBounds.extents.y);
            //     if(hasEffect)
            //     {
            //         maxExtent = Mathf.Max(maxExtent,effectMaxExtent);
            //     }
            //     ModelToPixelEditorWindow.previewRTCamera.orthographicSize = maxExtent;
            // }
        }

        private static List<Sprite> LoadAndSliceTexture(string texturePath)
        {
            List<Sprite> frames = new List<Sprite>();
            texturePath = texturePath.Replace("\\", "/");
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(texturePath);
            foreach (Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    frames.Add(sprite);
                }
            }

            return frames;
        }

        // 播放动画
        public static void PlayAnimation(SpriteRenderer previewSprite, SpriteRenderer previewSprite_effect)
        {
            if (previewSprite != null)
            {
                Animator animator = previewSprite.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 1; // 设置动画播放速度为正常速度
                }
            }
            if (previewSprite_effect != null)
            {
                Animator animator = previewSprite_effect.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 1; // 设置动画播放速度为正常速度
                }
            }
        }

        // 暂停动画
        public static void PauseAnimation(SpriteRenderer previewSprite, SpriteRenderer previewSprite_effect)
        {
            if (previewSprite != null)
            {
                Animator animator = previewSprite.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 0; // 设置动画播放速度为0，暂停动画
                }
            }
            if (previewSprite_effect != null)
            {
                Animator animator = previewSprite_effect.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 0; // 设置动画播放速度为0，暂停动画
                }
            }
        }

        public static void ReleaseTextures(List<Texture2D> textures)
        {
            if (textures == null) return;

            foreach (var texture in textures)
            {
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }
            }

            textures.Clear();
        }
    }
}