using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

[ExecuteAlways] 
public class FramesUpdate : MonoBehaviour
{
    private Material spriteMaterial;
    private Sprite lastSprite;

    void OnEnable()
    {
        UpdateMaterialIfNeeded();
    }

     void Update()
    {
        // 每帧检查贴图是否发生变化
        UpdateMaterialIfNeeded();
    }
    private void UpdateMaterialIfNeeded()
    {
        // 自动获取当前Sprite的材质
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string materialPath = Path.Combine(scriptFolder, "Shaders/M_CharacterShow.mat");
            Material characterShowMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            float lightAngleX = EditorPrefs.GetFloat("lightAngleX");
            float lightAngleY = EditorPrefs.GetFloat("lightAngleY");
            Vector3 lightDir = Quaternion.Euler(lightAngleX, lightAngleY, 0) * Vector3.forward;
            Shader.SetGlobalVector("_PreviewCharacterLightDir", lightDir);

            spriteRenderer.material = characterShowMaterial;
            spriteMaterial = spriteRenderer.sharedMaterial;
        }

        Sprite currentSprite = spriteRenderer.sprite; // 通过SpriteRenderer获取Sprite

        // 如果sprite发生变化，执行更新
        if (currentSprite != lastSprite && currentSprite != null)
        {
            lastSprite = currentSprite;
            UpdateMaterialTextures();
        }
    }

    void UpdateMaterialTextures()
    {
        string mainTexPath = AssetDatabase.GetAssetPath(lastSprite);

        // 替换路径，找到normal贴图
        string normalTexPath = ReplaceName(mainTexPath,"_N");

        // 加载法线贴图
        Texture normalTex = AssetDatabase.LoadAssetAtPath<Texture>(normalTexPath);
        Debug.Log(normalTexPath);
        Debug.Log(normalTex);

        // 如果找到法线贴图，设置_NormalMap；否则，将_NormalMap设为空
        if (normalTex != null)
        {
            spriteMaterial.SetTexture("_NormalMap", normalTex);
            spriteMaterial.SetFloat("_NoNormalMap", 0.0f); // 设置shader变量，启用normal map
        }
        else
        {
            spriteMaterial.SetTexture("_NormalMap", null);
            spriteMaterial.SetFloat("_NoNormalMap", 1.0f); // 没有法线贴图时关闭normal map
            Debug.LogWarning($"Normal map {normalTexPath} not found, setting to null.");
        }

        // 替换路径，找到effect贴图
        string effectTexPath = ReplaceName(mainTexPath,"_E");

        // 加载法线贴图
        Texture effectTex = AssetDatabase.LoadAssetAtPath<Texture>(effectTexPath);

        // 如果找到法线贴图，设置_NormalMap；否则，将_NormalMap设为空
        if (normalTex != null)
        {
            spriteMaterial.SetTexture("_EffectMap", effectTex);
        }
        else
        {
            spriteMaterial.SetTexture("_EffectMap", null);
        }
    }
    private string ReplaceName(string mainTexPath,string suffix)
    {
        int suffixIndex = mainTexPath.LastIndexOf("_D");
        string newFileName = mainTexPath.Substring(0, suffixIndex) + suffix + mainTexPath.Substring(suffixIndex + 2);
        string type = "/Sprite/";
        if(suffix=="_N")
        {
            type = "/Sprite(normal)/";
        }
        else if(suffix=="_E")
        {
            type = "/Sprite(effect)/";
        }
         newFileName = newFileName.Replace("/Sprite/", type);

        return newFileName;
    }
}