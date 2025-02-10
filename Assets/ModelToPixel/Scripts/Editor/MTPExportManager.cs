using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace ModelToPixel
{
    public class MTPExportManager : MonoBehaviour
    {
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }
        public static void CreateAnimationFromTextures(List<string> textures, string directoryPath, string animName, int frameRate)
        {
            List<Sprite> sprites = new List<Sprite>();

            
            foreach (string texture in textures)
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(texture) as TextureImporter;
                
                if (textureImporter != null)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.SaveAndReimport();
                    
                    // 重新导入为Sprite后，加载该Sprite
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texture);
                    if (sprite != null)
                    {
                        sprites.Add(sprite);
                    }
                }
            }

            if (sprites.Count == 0)
            {
                Debug.LogError("No sprites found after re-importing textures.");
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

            string animFilePath = Path.Combine(directoryPath, $"{animName}.anim");
            AssetDatabase.CreateAsset(animClip, animFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void GenerateOutlineMasks(string folderPath)
        {
            // 获取指定文件夹内所有贴图
            string[] files = Directory.GetFiles(folderPath, $"*{"_D"}.png", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string targetFileName = fileName.Replace("_D", "_M");
                string targetPath = Path.Combine(Path.GetDirectoryName(filePath), targetFileName + ".png");

                TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (importer != null)
                {
                    importer.isReadable = true;
                    importer.SaveAndReimport(); 
                }

                // 加载原始贴图
                Texture2D sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                if (sourceTexture == null) continue;

                Texture2D outlineTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, false);
                outlineTexture.Apply();
                // 检查是否已存在 _M 贴图
                if (File.Exists(targetPath))
                {
                    // 加载现有的 _M 贴图
                    outlineTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath);
                    TextureImporter mixImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
                    if (mixImporter != null)
                    {
                        mixImporter.isReadable = true;
                        mixImporter.textureCompression = TextureImporterCompression.Uncompressed;  // 避免压缩导致的格式不支持
                        mixImporter.SaveAndReimport(); 
                    }
                    if (outlineTexture == null) continue;
                }
                else
                {
                    // 创建目标贴图
                    outlineTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
                }


                // UV偏移计算描边mask
                for (int y = 0; y < sourceTexture.height; y++)
                {
                    for (int x = 0; x < sourceTexture.width; x++)
                    {
                        Color currentPixel = sourceTexture.GetPixel(x, y);
                        Color outLine = Color.black;

                        // 四方向偏移采样
                        Color[] neighbors = new Color[4];
                        neighbors[0] = GetPixelSafe(sourceTexture, x - (int)1, y); // 左
                        neighbors[1] = GetPixelSafe(sourceTexture, x + (int)1, y); // 右
                        neighbors[2] = GetPixelSafe(sourceTexture, x, y - (int)1); // 上
                        neighbors[3] = GetPixelSafe(sourceTexture, x, y + (int)1); // 下

                        // 检查是否是描边
                        foreach (Color neighbor in neighbors)
                        {
                            outLine.r += neighbor.a - currentPixel.a;
                        }

                        // 将描边信息写入目标贴图的r通道
                        Color outlineColor = outlineTexture.GetPixel(x, y);
                        outlineColor.r = outLine.r;
                        outlineTexture.SetPixel(x, y, outlineColor);
                    }
                }

                if (importer != null)
                {
                    importer.isReadable = false;
                    importer.SaveAndReimport(); 
                }

                outlineTexture.Apply();

                // 保存目标贴图到同路径，后缀为_M
                byte[] pngData = outlineTexture.EncodeToPNG();
                if (pngData != null)
                {
                    File.WriteAllBytes(targetPath, pngData);
                }

                // 强制刷新AssetDatabase
                AssetDatabase.Refresh();

                TextureImporter maskImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
                if (maskImporter != null)
                {
                    maskImporter.textureCompression = TextureImporterCompression.Compressed; 
                    maskImporter.maxTextureSize = 8192;
                    maskImporter.compressionQuality = (int)TextureCompressionQuality.Normal;
                    maskImporter.textureType = TextureImporterType.Sprite;
                    maskImporter.wrapMode = TextureWrapMode.Clamp;
                    maskImporter.filterMode = FilterMode.Point;
                    maskImporter.spritePixelsPerUnit = 32;
                    TextureImporterSettings settings = new TextureImporterSettings();
                    maskImporter.ReadTextureSettings(settings);
                    settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    maskImporter.SetTextureSettings(settings);
                    maskImporter.alphaSource = 0;
                    maskImporter.sRGBTexture = false;
                    maskImporter.isReadable = false;
                    maskImporter.SaveAndReimport();
                }
            }

            Debug.Log("All masks generated!");
        }

        private static Color GetPixelSafe(Texture2D texture, int x, int y)
        {
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            {
                return new Color(0, 0, 0, 0); // 超出范围，返回透明像素
            }
            return texture.GetPixel(x, y);
        }
        
        public static void ExportCompositeFrame(string filePath, List<Texture2D> frameTextures, int cols, int rows, TextureFormat textureFormat, bool sRGB, Color color)
        {
            if (frameTextures == null || frameTextures.Count == 0) return;

            int frameWidth = frameTextures[0].width;  // 使用第一帧的宽度
            int frameHeight = frameTextures[0].height; // 使用第一帧的高度

            // 创建一个新的Texture2D用于存储合成图像
            Texture2D finalTexture = new Texture2D(frameWidth * cols, frameHeight * rows, textureFormat, false)
            {
                filterMode = FilterMode.Point
            };

            Color[] blackPixels = new Color[finalTexture.width * finalTexture.height];
            for (int j = 0; j < blackPixels.Length; j++)
            {
                blackPixels[j] = color; // 黑色，Alpha 通道为 0
            }
            finalTexture.SetPixels(blackPixels);
            finalTexture.Apply();

            // 将每一帧的贴图绘制到最终合成的贴图上
            for (int i = 0; i < frameTextures.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                if (col < cols && row < rows)
                {
                    Texture2D frameTexture = frameTextures[i];

                    if (frameTexture != null)
                    {
                        // 确保frameTexture尺寸匹配
                        if (frameTexture.width != frameWidth || frameTexture.height != frameHeight)
                        {
                            Debug.LogError($"Frame texture at index {i} has a different size: {frameTexture.width}x{frameTexture.height}. Expected size: {frameWidth}x{frameHeight}.");
                            continue;
                        }

                        // 将帧的像素数据写入到finalTexture中
                        finalTexture.SetPixels(col * frameWidth, (rows - row - 1) * frameHeight, frameWidth, frameHeight, frameTexture.GetPixels());
                    }
                }
            }

            finalTexture.Apply();

            // 保存合成的贴图为PNG文件
            byte[] bytes = finalTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();

            // 清理资源
            GameObject.DestroyImmediate(finalTexture);

            // 设置TextureImporter属性
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.maxTextureSize = 8192;
                importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point;
                importer.spritePixelsPerUnit = 32;
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.sRGBTexture = sRGB;
                importer.SaveAndReimport();
            }
        }

        public static void ExportSelectedFramesBullet(List<int> selectedFrames, string exportPath, string previewAnimPaths, string effectPath)
        {
            string animationName = Path.GetFileNameWithoutExtension(previewAnimPaths);

            string nameBeforeSeparator = ExtractNameBeforeSeparator(animationName);
            string animationExportPath = Path.Combine(exportPath, nameBeforeSeparator, animationName);
            if (!Directory.Exists(animationExportPath))
            {
                Directory.CreateDirectory(animationExportPath);
            }


            List<Texture2D> effectFrameTextures = new List<Texture2D>();
            selectedFrames.Sort();

            for (int i = 0; i < selectedFrames.Count; i++)
            {
                int frameIndex = selectedFrames[i];
                string FolderPath = Path.GetDirectoryName(effectPath);

                string effectFilePath = Path.Combine(FolderPath,$"{Path.GetFileNameWithoutExtension(effectPath)}{frameIndex}.png");

                // 加载已复制的帧贴图
                Texture2D effectFrameTexture = null;

                effectFrameTexture = MTPExportManager.LoadTextureFromFile(effectFilePath);
                effectFrameTextures.Add(effectFrameTexture);
            }

            // for Config
            string characterPath = Path.Combine(exportPath, nameBeforeSeparator);
            int rows = Mathf.CeilToInt(Mathf.Sqrt(selectedFrames.Count));
            int cols = Mathf.CeilToInt(selectedFrames.Count / (float)rows);

            if (effectFrameTextures.Count > 0) // 只有在存在_E贴图时才生成合成图像
            {
                string compositeEffectFileName = $"{animationName}_E.png";
                MTPExportManager.ExportCompositeFrame(Path.Combine(animationExportPath, compositeEffectFileName), effectFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));
            }

            AssetDatabase.Refresh();
        }
        
        public static void ExportSelectedFrames(List<int> selectedFrames, string exportPath, string previewAnimPaths, string basePath, string normalPath, string effectPath, string mixPath, string customName = "")
        {
            string animationName = Path.GetFileNameWithoutExtension(previewAnimPaths);

            string nameBeforeSeparator = ExtractNameBeforeSeparator(animationName);

            string exportAnimationName = !string.IsNullOrEmpty(customName) ? customName : animationName;
            string animationExportPath = Path.Combine(exportPath, nameBeforeSeparator, exportAnimationName);
            if (!Directory.Exists(animationExportPath))
            {
                Directory.CreateDirectory(animationExportPath);
            }

            List<Texture2D> baseFrameTextures = new List<Texture2D>();
            List<Texture2D> normalFrameTextures = new List<Texture2D>();
            List<Texture2D> effectFrameTextures = new List<Texture2D>();
            List<Texture2D> mixFrameTextures = new List<Texture2D>();
            selectedFrames.Sort();

            bool hasEffect = false; 

            for (int i = 0; i < selectedFrames.Count; i++)
            {
                int frameIndex = selectedFrames[i];
                string FolderPath = Path.GetDirectoryName(basePath);

                string baseFilePath = Path.Combine(FolderPath,$"{Path.GetFileNameWithoutExtension(basePath)}{frameIndex}.png");
                string normalFilePath = Path.Combine(FolderPath,$"{Path.GetFileNameWithoutExtension(normalPath)}{frameIndex}.png");
                string effectFilePath = Path.Combine(FolderPath,$"{Path.GetFileNameWithoutExtension(effectPath)}{frameIndex}.png");
                string mixFilePath = Path.Combine(FolderPath,$"{Path.GetFileNameWithoutExtension(mixPath)}{frameIndex}.png");

                // 加载已复制的帧贴图
                Texture2D baseFrameTexture = MTPExportManager.LoadTextureFromFile(baseFilePath);
                Texture2D normalFrameTexture = MTPExportManager.LoadTextureFromFile(normalFilePath);
                Texture2D mixFrameTexture = MTPExportManager.LoadTextureFromFile(mixFilePath);
                Texture2D effectFrameTexture = null;

                if (File.Exists(effectFilePath))
                {
                    effectFrameTexture = MTPExportManager.LoadTextureFromFile(effectFilePath);
                    effectFrameTextures.Add(effectFrameTexture);
                    hasEffect = true; 
                }
                baseFrameTextures.Add(baseFrameTexture);
                normalFrameTextures.Add(normalFrameTexture);
                mixFrameTextures.Add(mixFrameTexture);
            }

            // for Config
            string characterPath = Path.Combine(exportPath, nameBeforeSeparator);
            int rows = Mathf.CeilToInt(Mathf.Sqrt(selectedFrames.Count));
            int cols = Mathf.CeilToInt(selectedFrames.Count / (float)rows);
            float scaleX = 1.0f/cols;
            float scaleY = 1.0f/rows;

            string compositeBaseFileName = $"{exportAnimationName}_D.png";
            string compositeNormalFileName = $"{exportAnimationName}_N.png";
            string compositeMixFileName = $"{exportAnimationName}_M.png";

            MTPExportManager.ExportCompositeFrame(Path.Combine(animationExportPath, compositeBaseFileName), baseFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));
            MTPExportManager.ExportCompositeFrame(Path.Combine(animationExportPath, compositeNormalFileName), normalFrameTextures, cols, rows, TextureFormat.RGB24, false, new Color(0.5f, 0.5f, 1, 0));
            MTPExportManager.ExportCompositeFrame(Path.Combine(animationExportPath, compositeMixFileName), mixFrameTextures, cols, rows, TextureFormat.RGB24, false, new Color(0, 0, 0, 0));
            if (effectFrameTextures.Count > 0) // 只有在存在_E贴图时才生成合成图像
            {
                string compositeEffectFileName = $"{exportAnimationName}_E.png";
                MTPExportManager.ExportCompositeFrame(Path.Combine(animationExportPath, compositeEffectFileName), effectFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));
            }
            GenerateOutlineMasks(exportPath);

            // 更新 JSON 配置文件
            UpdateAnimationConfig(characterPath, exportAnimationName, rows, cols, selectedFrames.Count, scaleX, scaleY, selectedFrames, hasEffect);
            AssetDatabase.Refresh();
        }

        public static void ExportSelectedFramesForEditor(List<int> selectedFrames, string editorExportPath, string previewAnimPaths, string basePath, string normalPath, string effectPath, string mixPath, AnimationClip previewClip, string customName = "")
        {
            string animationName = Path.GetFileNameWithoutExtension(previewAnimPaths);

            string nameBeforeSeparator = ExtractNameBeforeSeparator(animationName);
            string exportAnimationName = !string.IsNullOrEmpty(customName) ? customName : animationName;
            string TexExportPath = Path.Combine(editorExportPath, nameBeforeSeparator, "Sprite");
            string normalExportPath = Path.Combine(editorExportPath, nameBeforeSeparator, "Sprite(normal)");
            string effectExportPath = Path.Combine(editorExportPath, nameBeforeSeparator, "Sprite(effect)");
            string mixExportPath = Path.Combine(editorExportPath, nameBeforeSeparator, "Sprite(mix)");
            string animationExportPath = Path.Combine(editorExportPath, nameBeforeSeparator, "Animation");
            if (!Directory.Exists(TexExportPath))
            {
                Directory.CreateDirectory(TexExportPath);
            }
            if (!Directory.Exists(normalExportPath))
            {
                Directory.CreateDirectory(normalExportPath);
            }
            if (!Directory.Exists(mixExportPath))
            {
                Directory.CreateDirectory(mixExportPath);
            }
            if (!Directory.Exists(animationExportPath))
            {
                Directory.CreateDirectory(animationExportPath);
            }

            List<Texture2D> baseFrameTextures = new List<Texture2D>();
            List<string> basePaths = new List<string>();

            for (int i = 0; i < selectedFrames.Count; i++)
            {
                int frameIndex = selectedFrames[i];

                string baseFilePath = $"{TexExportPath}/{(exportAnimationName)}_D{i}.png";

                CopyFileToExportPath(basePath, baseFilePath, frameIndex);

                // 加载已复制的帧贴图
                Texture2D baseFrameTexture = MTPExportManager.LoadTextureFromFile(baseFilePath);
                baseFrameTextures.Add(baseFrameTexture);
                basePaths.Add(baseFilePath);

                // Normal Texture
                string normalFilePath = $"{normalExportPath}/{(exportAnimationName)}_N{i}.png";
                CopyFileToExportPath(normalPath, normalFilePath, frameIndex);

                TextureImporter importer = AssetImporter.GetAtPath(normalFilePath) as TextureImporter;
                if (importer != null)
                {
                    importer.sRGBTexture = false; // 确保关闭 sRGB
                    importer.SaveAndReimport();
                }

                // Mix Texture
                string mixPathlFilePath = $"{mixExportPath}/{(exportAnimationName)}_M{i}.png";
                CopyFileToExportPath(mixPath, mixPathlFilePath, frameIndex);

                
                importer = AssetImporter.GetAtPath(mixPathlFilePath) as TextureImporter;
                if (importer != null)
                {
                    importer.sRGBTexture = false; // 确保关闭 sRGB
                    importer.SaveAndReimport();
                }

                if (File.Exists(effectPath))
                {
                    if (!Directory.Exists(effectExportPath))
                    {
                        Directory.CreateDirectory(effectExportPath);
                    }

                    string effectFilePath = $"{effectExportPath}/{(exportAnimationName)}{i}_E.png";
                    CopyFileToExportPath(effectPath, effectFilePath, frameIndex);
                }
            }
            AssetDatabase.Refresh();
            int frameRate = 30;
            if(previewClip)
            {
                frameRate = (int)previewClip.frameRate;
            }

            MTPExportManager.CreateAnimationFromTextures(basePaths, animationExportPath, exportAnimationName, frameRate);
            AssetDatabase.Refresh();
        }

        private static void CopyFileToExportPath(string sourcePath, string destPath, int frameIndex)
        {
            string frameFileName = Path.GetFileNameWithoutExtension(sourcePath) + frameIndex + Path.GetExtension(sourcePath);
            string sourceFramePath = Path.Combine(Path.GetDirectoryName(sourcePath), frameFileName);

            if (File.Exists(sourceFramePath))
            {
                File.Copy(sourceFramePath, destPath, true);
                AssetDatabase.ImportAsset(destPath, ImportAssetOptions.ForceUpdate);
                TextureImporter importer = AssetImporter.GetAtPath(destPath) as TextureImporter;
                if (importer != null)
                {
                    importer.maxTextureSize = 8192;
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.wrapMode = TextureWrapMode.Clamp;
                    importer.filterMode = FilterMode.Point;
                    importer.compressionQuality = (int)TextureCompressionQuality.Normal;
                    importer.spritePixelsPerUnit = 32;
                    TextureImporterSettings settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    importer.SetTextureSettings(settings);
                    importer.isReadable = true;
                    importer.SaveAndReimport();
                }
            }
            else
            {
                Debug.LogWarning($"File not found: {sourceFramePath}");
            }
        }

    private static void ReplaceAnimationInfo(AnimationConfigSO config, string animationName, int rows, int cols, int totalFrames, float scaleX, float scaleY, List<int> selectedFrames, bool hasEffect)
    {
        // 查找动画信息是否已经存在
        var existingInfo = config.animations.Find(a => a.animationName == animationName);

        if (existingInfo != null)
        {
            // 如果找到名字相同的动画，替换其信息，但保留 selectedFrameIndices
            existingInfo.animationName = animationName;
            existingInfo.rows = rows;
            existingInfo.columns = cols;
            existingInfo.totalFrames = totalFrames;
            existingInfo.scaleX = scaleX;
            existingInfo.scaleY = scaleY;
            existingInfo.hasEffect = hasEffect;

            // 不覆盖现有的 selectedFrameIndices
            Debug.Log($"动画 {animationName} 的信息已更新，但保留了选中的帧。");
        }
        else
        {
            // 如果没有找到，创建新的动画信息，并使用传入的 selectedFrames
            AnimationInfo newAnimationInfo = new AnimationInfo
            {
                animationName = animationName,
                rows = rows,
                columns = cols,
                totalFrames = totalFrames,
                scaleX = scaleX,
                scaleY = scaleY,
                selectedFrameIndices = new List<int>(selectedFrames), // 新的动画使用传入的选中帧
                hasEffect = hasEffect
            };
            config.animations.Add(newAnimationInfo);
            Debug.Log($"动画 {animationName} 的新信息已添加。");
        }

        // 标记资产为脏并保存
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }

        private static void UpdateAnimationConfig(string characterPath, string animationName, int rows, int columns, int totalFrames, float scaleX, float scaleY, List<int> selectedFrames, bool effect)
        {
            string configPath = Path.Combine(characterPath,Path.GetFileName(characterPath)+"_config.asset");

            AnimationConfigSO config;

            // 检查是否存在已有的ScriptableObject
            config = AssetDatabase.LoadAssetAtPath<AnimationConfigSO>(configPath);
            if (config == null)
            {
                // 创建新的ScriptableObject
                config = ScriptableObject.CreateInstance<AnimationConfigSO>();
                config.characterName = Path.GetFileName(characterPath);
                AssetDatabase.CreateAsset(config, configPath);
            }

            // 更新或添加动画配置信息
            AnimationInfo animationInfo = new AnimationInfo
            {
                animationName = animationName,
                //oldName = animationName,
                rows = rows,
                columns = columns,
                totalFrames = totalFrames,
                scaleX = scaleX,
                scaleY = scaleY,
                selectedFrameIndices = new List<int>(selectedFrames),
                hasEffect =  effect,
            };

            // 查找是否已有相同名字的动画配置，若有则替换
            var existingInfo = config.animations.Find(a => a.animationName == animationName);
            if (existingInfo != null)
            {
                config.animations.Remove(existingInfo);
            }
            config.animations.Add(animationInfo);

            // 标记为已更改，并保存到资产数据库
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        public static void ExportAnimationToSpriteSheet(AnimationClip animClip, string exportPath, int frameRate)
        {
            if (animClip == null)
            {
                Debug.LogWarning("未选择动画文件！");
                return;
            }
            // 获取动画的第一帧 Sprite
            ObjectReferenceKeyframe[] spriteKeyframes = AnimationUtility.GetObjectReferenceCurve(animClip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
            if (spriteKeyframes == null || spriteKeyframes.Length == 0)
            {
                Debug.LogWarning("无法从动画中提取到Sprite序列。");
                return;
            }

            // 使用第一帧的Sprite名称来确定基础名称
            Sprite firstFrameSprite = spriteKeyframes[0].value as Sprite;
            if (firstFrameSprite == null)
            {
                Debug.LogError("动画的第一帧没有包含有效的Sprite。");
                return;
            }

            string firstFrameName = firstFrameSprite.name; // 获取第一帧的Sprite名称
            int splitIndex = firstFrameName.IndexOf('@');
            if (splitIndex == -1)
            {
                splitIndex = firstFrameName.IndexOf('_');
            }

            // 如果都没有找到，使用完整文件名；否则取分隔符之前的部分
            string nameBeforeSeparator = splitIndex == -1 ? firstFrameName : firstFrameName.Substring(0, splitIndex);
            string oldAnimationName = firstFrameName.Substring(0, firstFrameName.LastIndexOf('_'));

            string outputPath = Path.Combine(exportPath, nameBeforeSeparator, animClip.name);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            List<Texture2D> animFrameTextures = new List<Texture2D>();
            List<Texture2D> normalFrameTextures = new List<Texture2D>();
            List<Texture2D> effectFrameTextures = new List<Texture2D>();
            List<Texture2D> mixFrameTextures = new List<Texture2D>();
            int totalFrames = Mathf.FloorToInt(animClip.length * frameRate);

            // 记录处理动画中空的帧
            Sprite lastValidSprite = null;
            int keyframeIndex = 0;

            for (int i = 0; i < totalFrames; i++)
            {
                // 检查当前帧是否有对应的Sprite
                if (keyframeIndex < spriteKeyframes.Length && Mathf.RoundToInt(spriteKeyframes[keyframeIndex].time * frameRate) == i)
                {
                    lastValidSprite = spriteKeyframes[keyframeIndex].value as Sprite;
                    string path = AssetDatabase.GetAssetPath(lastValidSprite);
                    keyframeIndex++;
                }

                if (lastValidSprite != null)
                {
                    // 获取Diffuse图像
                    Texture2D frameTexture = GetTextureFromSprite(lastValidSprite);
                    animFrameTextures.Add(frameTexture);


                    // 获取Normal图像
                    Sprite normalSprite = GetCorrespondingSprite(lastValidSprite, "Sprite(normal)", "_N");
                    if (normalSprite != null)
                    {
                        Texture2D normalTexture = GetTextureFromSprite(normalSprite);
                        normalFrameTextures.Add(normalTexture);
                    }

                    // 获取Normal图像
                    Sprite mixSprite = GetCorrespondingSprite(lastValidSprite, "Sprite(mix)", "_M");
                    if (mixSprite != null)
                    {
                        Texture2D mixTexture = GetTextureFromSprite(mixSprite);
                        mixFrameTextures.Add(mixTexture);
                    }

                    // 获取Effect图像（如果存在）
                    Sprite effectSprite = GetCorrespondingSprite(lastValidSprite, "Sprite(effect)", "_E");
                    if (effectSprite != null)
                    {
                        Texture2D effectTexture = GetTextureFromSprite(effectSprite);
                        effectFrameTextures.Add(effectTexture);
                    }
                }
                else
                {
                    Debug.LogWarning($"Frame {i} has no sprite and no valid previous sprite.");
                }
            }

            int cols = Mathf.CeilToInt(Mathf.Sqrt(animFrameTextures.Count));
            int rows = Mathf.CeilToInt(animFrameTextures.Count / (float)cols);
            float scaleX = 1.0f / cols;
            float scaleY = 1.0f / rows;
            
            // 导出Diffuse序列帧
            string diffuseFilePath = $"{outputPath}/{animClip.name}_D.png";
            MTPExportManager.ExportCompositeFrame(diffuseFilePath, animFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));

            // 导出Normal序列帧
            if (normalFrameTextures.Count > 0)
            {
                string normalFilePath = $"{outputPath}/{animClip.name}_N.png";
                MTPExportManager.ExportCompositeFrame(normalFilePath, normalFrameTextures, cols, rows, TextureFormat.RGB24, false, new Color(0.5f, 0.5f, 1, 1));
            }

            // 导出Effect序列帧（如果存在）
            if (effectFrameTextures.Count > 0)
            {
                string effectFilePath = $"{outputPath}/{animClip.name}_E.png";
                MTPExportManager.ExportCompositeFrame(effectFilePath, effectFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));
            }

            if (mixFrameTextures.Count > 0)
            {
                string mixFilePath = $"{outputPath}/{animClip.name}_M.png";
                MTPExportManager.ExportCompositeFrame(mixFilePath, mixFrameTextures, cols, rows, TextureFormat.RGBA32, true, new Color(0, 0, 0, 0));
            }


            // 清理资源
            foreach (var texture in animFrameTextures) Destroy(texture);
            foreach (var texture in normalFrameTextures) DestroyImmediate(texture);
            foreach (var texture in effectFrameTextures) DestroyImmediate(texture);
            foreach (var texture in mixFrameTextures) DestroyImmediate(texture);

            // 更新动画配置文件
            string characterPath = Path.Combine(exportPath, nameBeforeSeparator);
            bool hasEffect = effectFrameTextures.Count > 0;
            List<int> selectedFrames = new List<int>();
            for (int i = 0; i < totalFrames; i++)
            {
                selectedFrames.Add(i);
            }

            // 获取配置文件
            AnimationConfigSO config = AssetDatabase.LoadAssetAtPath<AnimationConfigSO>(Path.Combine(characterPath, $"{firstFrameName.Split('@')[0]}_config.asset"));

            // 更新或替换动画信息
            ReplaceAnimationInfo(config, animClip.name, rows, cols, totalFrames, scaleX, scaleY, selectedFrames, hasEffect);
        }

        private static Texture2D GetTextureFromSprite(Sprite sprite)
        {
            string path = AssetDatabase.GetAssetPath(sprite.texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            Rect spriteRect = sprite.rect;
            Texture2D frameTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);
            Color[] pixels = sprite.texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
            frameTexture.SetPixels(pixels);
            frameTexture.Apply();

            return frameTexture;
        }

        private static Sprite GetCorrespondingSprite(Sprite baseSprite, string folderName, string suffix)
        {
            string basePath = AssetDatabase.GetAssetPath(baseSprite.texture);
            string fileName = Path.GetFileNameWithoutExtension(basePath);

            // 查找最后一个 "_D" 出现的位置，并替换为相应的后缀
            int suffixIndex = fileName.LastIndexOf("_D");

            if (suffixIndex >= 0)
            {
                // 构造新的文件名
                string newFileName = fileName.Substring(0, suffixIndex) + suffix + fileName.Substring(suffixIndex + 2);
                string normalSpritePath = Path.Combine(Path.GetDirectoryName(basePath).Replace("Sprite", folderName), newFileName + Path.GetExtension(basePath));

                Texture2D correspondingTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalSpritePath);

                if (correspondingTexture == null)
                {
                    return null;
                }

                Rect spriteRect = baseSprite.rect;
                float widthRatio = correspondingTexture.width / baseSprite.texture.width;
                float heightRatio = correspondingTexture.height / baseSprite.texture.height;

                Rect newRect;
                if (Mathf.Approximately(widthRatio, 2f) && Mathf.Approximately(heightRatio, 2f))
                {
                    // 如果 effect 的分辨率是 base 的两倍，缩放 rect
                    newRect = new Rect(spriteRect.x * 2, spriteRect.y * 2, spriteRect.width * 2, spriteRect.height * 2);
                }
                else
                {
                    // 如果尺寸一致，保持原始 rect
                    newRect = spriteRect;
                }
                Sprite correspondingSprite = Sprite.Create(correspondingTexture, newRect, new Vector2(0.5f, 0.5f), baseSprite.pixelsPerUnit);
                return correspondingSprite;
            }
            return null;
        }

        // 最终合并特效和basecolor图
        private void CombineBaseAndEffectTextures(Texture2D baseTexture, Texture2D effectTexture, Texture2D normalTexture)
        {

            int width = baseTexture.width;
            int height = baseTexture.height;

            Color[] basePixels = baseTexture.GetPixels();
            Color[] effectPixels = effectTexture.GetPixels();
            Color[] normalPixels = normalTexture != null ? normalTexture.GetPixels() : new Color[width * height];

            for (int i = 0; i < basePixels.Length; i++)
            {
                // 如果 effect 有透明度，叠加到 basecolor 上
                if (effectPixels[i].a > 0f)
                {
                    basePixels[i] = effectPixels[i];
                }

                // 将 effect 的透明度存入 normal 图的 b 通道
                normalPixels[i].b = effectPixels[i].a;
            }

            baseTexture.SetPixels(basePixels);
            baseTexture.Apply();

            // 更新 normal 图的内容
            normalTexture.SetPixels(normalPixels);
            normalTexture.Apply();
        }

        public static string ExtractNameBeforeSeparator(string name)
        {
            int splitIndex = name.IndexOf('@');
            if (splitIndex == -1)
            {
                splitIndex = name.IndexOf('_');
            }

            // 如果都没有找到，使用完整文件名；否则取分隔符之前的部分
            string nameBeforeSeparator = splitIndex == -1 ? name : name.Substring(0, splitIndex);
            return nameBeforeSeparator; // 未找到分隔符，返回完整名称
        }
    }
}