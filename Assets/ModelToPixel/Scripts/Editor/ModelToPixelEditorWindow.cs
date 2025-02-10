using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
namespace ModelToPixel
{
    public class ModelToPixelEditorWindow : EditorWindow
    {
        private GameObject target;
        private GameObject staticTarget;
        private Camera captureCamera;
        public static Camera previewRTCamera;
        private Camera effectRTCamera;
        private RenderTexture previewRenderTexture;
        private int frameRate = 30;
        private string outputFolder = "Assets/Art/ArtTools/ModelToPixel/Example";
        private int textureWidth = 128;
        private int textureHeight = 128;
        private float normalIntensity = 0;
        private float viewSize = 2f;
        private GameObject targetInstance;
        private GameObject staticTargetInstance;
        private GameObject staticTargetEffectInstance;
        private GameObject bulletInstance;
        private GameObject bulletInstanceEffect;
        private GameObject modelPivotEffect;
        private GameObject staticModelPivotEffect;
        private GameObject staticModelPivot;
        private MTPCapture captureScript;
        private Animator animator;
        private string folderPath = "Assets/Art";
        private Vector2 scrollPosition;
        private List<string> animPaths = new List<string>();
        private List<string> showAnimPaths = new List<string>();
        private List<bool> animSelections = new List<bool>();
        private int selectedAnimIndex = -1;
        private int tabIndex = 0;

        // 用于预览的动画路径和选择
        private string previewFolderPath = "Assets";
        private Vector2 previewScrollPosition;
        private List<string> showPreviewAnimPaths = new List<string>();
        private List<string> previewAnimPaths = new List<string>();
        private int selectedPreviewAnimIndex = -1;
        private SpriteRenderer previewSprite;
        private SpriteRenderer previewSprite_effect;
        private SpriteRenderer previewSprite_bulletEffect;

        private string previewBulletFolderPath = "Assets";
        private Vector2 previewBulletScrollPosition;
        private List<string> showPreviewBulletAnimPaths = new List<string>();
        private List<string> previewBulletAnimPaths = new List<string>();
        private int selectedPreviewBulletAnimIndex = -1;

        private bool flipX = false;
        private bool showOutLine = false;
        private bool showStaticOutLine = false;
        private bool captureflipX = false;
        private bool captureflipZ = false;
        private const string requiredSceneName = "3Dto2D"; 
        private Color backgroundColor;
        private Vector2 scrollPos;
        private List<int> selectedFrames = new List<int>();
        private List<int> selectedBulletFrames = new List<int>();
        private Vector3 cameraPosition = new Vector3(0, 0, 0);
        private float lightAngleX = 20f;
        private float lightAngleY = 262f;
        private Transform Light;
        public static List<RenderTexture> previewRenderTextures = new List<RenderTexture>();
        private string exportPath = "Assets/GameResource/Atlas/Character";
        private string bulletExportPath = "Assets/GameResource/Atlas/Character";
        private string editorExportPath = "Assets/Art/Character";
        private string normalPath;
        private string basePath;
        private string effectPath;
        private string mixPath;

        private VFXEditor vfxEditor;
        private const string captureCameraName = "CaptureCamera";

        private const string previewRTCameraName = "PreviewRTCamera";

        private AnimationClip previewClip;

        private GameObject modelPivot;
        private GameObject bulletPivot;
        private GameObject bulletPivotEffect;
        private AnimationClip selectedAnimClip;

        private bool effectTransparency = false; // 默认透明设置
        private bool effectDoubleRes = false; // 默认透明设置
        private Color effectBackgroundColor = Color.black; // 默认背景颜色设置


        private GameObject effectPrefab; // 特效Prefab
        private string bulletOutputFolder = "Assets/Art/ArtTools/ModelToPixel/Example";
        private int selectedIndex = 0;
        private string[] options = { "动态角色", "静态角色", "子弹特效" };
        private string[] rolePanels = { "角色序列帧烘焙", "效果预览" };
        private string[] staticRolePanels = { "静态角色烘焙"};
        private string[] bulletPanels = { "子弹特效序列帧烘焙", "效果预览" };
        private Vector3 effectScale  = Vector3.one;
        private Vector3 effectRotation  = Vector3.zero;

        int bulletCurrentFrame = 0;

        private bool useCustomName = false;   // 开关：是否启用自定义名称
        private string customExportName = ""; // 用户自定义名称

        private RenderTexture baseColorRT;
        private RenderTexture normalRT;
        private RenderTexture mixRT;
        private Texture resultTexture;

        [MenuItem("Tools/ArtTools/3Dto2D")]
        public static void ShowWindow()
        {
            ModelToPixelEditorWindow window = GetWindow<ModelToPixelEditorWindow>("3Dto2D");
            window.minSize = new Vector2(900, 800);
        }

        private static string GetScenePathByName(string sceneName)
        {
            string[] scenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
            foreach (var scene in scenes)
            {
                if (Path.GetFileNameWithoutExtension(scene) == sceneName)
                {
                    return scene;
                }
            }
            return null;
        }
        private string FindParentFolder(string currentPath, string targetFolderName)
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

        private void OnGUI()
        {
            backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.76f, 0.76f, 0.76f);
            if (SceneManager.GetActiveScene().name != requiredSceneName)
            {
                EditorGUILayout.LabelField($"请在场景 '{requiredSceneName}' 中使用此窗口。");

                if (GUILayout.Button($"打开场景 '{requiredSceneName}'"))
                {
                    string scenePath = GetScenePathByName(requiredSceneName);
                    if (!string.IsNullOrEmpty(scenePath))
                    {
                        EditorSceneManager.OpenScene(scenePath);
                        if (captureCamera == null)
                        captureCamera = GameObject.Find(captureCameraName)?.GetComponent<Camera>();

                        if (effectRTCamera == null)
                        effectRTCamera = GameObject.Find("EffectRTCamera")?.GetComponent<Camera>();
                        
                        if (previewRTCamera == null)
                            previewRTCamera = GameObject.Find(previewRTCameraName)?.GetComponent<Camera>();

                        if (vfxEditor == null)
                        {
                            vfxEditor = CreateInstance<VFXEditor>(); // 实例化VFX编辑器
                            vfxEditor.OnEnable(); // 调用VFX编辑器的OnEnable方法
                        }
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    }
                    else
                    {
                        Debug.LogError($"无法找到名为 '{requiredSceneName}' 的场景。");
                    }
                }

                return;
            }

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");
            string previewRenderTexturePath = Path.Combine(parentFolder, "RenderTexture", "PreviewRT.renderTexture").Replace("\\", "/");
            previewRenderTexture = AssetDatabase.LoadAssetAtPath<RenderTexture>(previewRenderTexturePath);
            string renderTexturePath = Path.Combine(scriptFolder, "RenderTexture", "CameraRT.renderTexture").Replace("\\", "/");
            
            EditorGUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(120)); // Adjust width to fit your design
            GUILayout.Label("烘焙类别", EditorStyles.boldLabel);
            
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, options, 1); 
            
            GUILayout.EndVertical();
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical();
            
            if (selectedIndex == 0) // 角色 selection
            {
                tabIndex = GUILayout.Toolbar(tabIndex, rolePanels);
                
                if(bulletPivotEffect)
                    bulletPivotEffect.SetActive(false);
                if(staticModelPivotEffect)
                    staticModelPivotEffect.SetActive(false);
                if(modelPivotEffect)
                    modelPivotEffect.SetActive(true);
                if(bulletPivot)
                    bulletPivot.SetActive(false);

                if(staticModelPivot)
                    staticModelPivot.SetActive(false);
                if(modelPivot)
                    modelPivot.SetActive(true);
                if (tabIndex == 0)
                {
                    DrawCaptureGUI(); // 角色序列帧烘焙
                }
                else if (tabIndex == 1)
                {
                    DrawPreviewGUI(); // 效果预览
                }
            }
            if (selectedIndex == 1) // 角色 selection
            {
                tabIndex = GUILayout.Toolbar(tabIndex, staticRolePanels);
                if(bulletPivotEffect)
                    bulletPivotEffect.SetActive(false);
                if(modelPivotEffect)
                    modelPivotEffect.SetActive(false);
                if(staticModelPivotEffect)
                    staticModelPivotEffect.SetActive(true);
                if(bulletPivot)
                    bulletPivot.SetActive(false);
                if(staticModelPivot)
                    staticModelPivot.SetActive(true);
                if(modelPivot)
                    modelPivot.SetActive(false);

                    DrawStaticCaptureGUI(); // 角色序列帧烘焙
                

            }
            else if (selectedIndex == 2) // 子弹特效 selection
            {
                tabIndex = GUILayout.Toolbar(tabIndex, bulletPanels);
                if(bulletPivotEffect)
                    bulletPivotEffect.SetActive(true);
                if(staticModelPivotEffect)
                    staticModelPivotEffect.SetActive(false);
                if(modelPivotEffect)
                    modelPivotEffect.SetActive(false);
                if(bulletPivot)
                    bulletPivot.SetActive(true);
                if(staticModelPivot)
                    staticModelPivot.SetActive(false);
                if(modelPivot)
                    modelPivot.SetActive(false);
                if (tabIndex == 0)
                {
                    DrawBulletCaptureGUI(); 
                }
                else if (tabIndex == 1)
                {
                    DrawBulletPreviewGUI(); 
                }
            }

            GUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        // 计算输入框宽度
        private float CalculateFieldWidth(string value)
        {
            return Mathf.Max(50, value.Length * 10);
        }

        // 动态调整整数输入框宽度
        private int IntFieldAutoWidth(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            string valueStr = value.ToString();
            value = EditorGUILayout.IntField(value, GUILayout.Width(CalculateFieldWidth(valueStr)));
            GUILayout.EndHorizontal();
            return value;
        }

        private float FloatFieldAutoWidth(string label, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            string valueStr = value.ToString();
            value = EditorGUILayout.FloatField(value, GUILayout.Width(CalculateFieldWidth(valueStr)));
            GUILayout.EndHorizontal();
            return value;
        }

        private void DrawRenderButton(string type)
        {
            if (captureCamera == null)
            {
                Debug.LogError("捕获相机未找到。");
                return;
            }

            captureCamera.orthographicSize = viewSize;
            effectRTCamera.orthographicSize = viewSize;
            if(bulletInstance)
            {
                bulletInstance.transform.localScale = effectScale;
                bulletInstance.transform.rotation = Quaternion.Euler(effectRotation);
            }
            if(bulletInstanceEffect)
            {
                bulletInstanceEffect.transform.localScale = effectScale;
                bulletInstanceEffect.transform.rotation = Quaternion.Euler(effectRotation);
            }

            captureCamera.transform.localPosition = cameraPosition;
            effectRTCamera.transform.localPosition = cameraPosition;


            GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            string buttonName = type == "staticCharacter" ? " 渲染像素化静态资产": "渲染序列帧";

            if (GUILayout.Button(buttonName, renderButtonStyle, GUILayout.Height(50)))
            {
                if (captureCamera != null)
                {
                    if (!EditorApplication.isPlaying)
                    {
                        EditorUtility.DisplayDialog("Play Mode Required", "提示：请在play模式进行", "确认");
                        return; // 提示用户手动进入播放模式
                    }
                    else
                    {
                        if(type =="character")
                        {           
                            if(target != null)  
                            {
                                StartCapture();   
                            }         
                            else
                            {
                                Debug.LogError("Target or Capture Camera is not assigned.");
                            }                        
                        }                

                        else if(type == "staticCharacter")
                        {
                            StartStaticModelCapture();
                        }                

                        else if(type == "bulletEffect")
                        {
                            StartCaptureForBullet();
                        }
                    }
                }

            }
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (string path in DragAndDrop.paths)
                        {
                            if (Directory.Exists(path))
                            {
                                folderPath = path;
                                LoadAnimationsInFolder(folderPath, animPaths, showAnimPaths, animSelections);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }
        
        private void HandleDragAndDrop(Rect dropArea, ref string folderPath, ref List<string> animPaths, ref List<string> showAnimPaths)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (string path in DragAndDrop.paths)
                        {
                            if (Directory.Exists(path))
                            {
                                if (folderPath != path)
                                {
                                    // 重置选择的动画列表
                                    selectedPreviewAnimIndex = -1; // 重置选中的动画索引
                                    selectedFrames.Clear(); // 清空已选择的帧列表
                                }
                                folderPath = path;
                                LoadPreviewAnimations(folderPath, animPaths, showAnimPaths);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }

        private void HandleDragAndDropForBullet(Rect dropArea, ref string folderPath, ref List<string> animPaths, ref List<string> showAnimPaths)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (string path in DragAndDrop.paths)
                        {
                            if (Directory.Exists(path))
                            {
 
                                    // 重置选择的动画列表
                                    selectedPreviewAnimIndex = -1; // 重置选中的动画索引
                                    selectedPreviewBulletAnimIndex = -1;
                                    selectedFrames.Clear(); // 清空已选择的帧列表
                                
                                folderPath = path;
                                LoadPreviewAnimationsForBullet(folderPath, animPaths, showAnimPaths);
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
        }
        
        private void DrawCaptureGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            // 烘焙设置 
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);
                float aspectRatio = (float)textureWidth / textureHeight; 
                EditorGUILayout.BeginVertical( GUILayout.Width(400) );
                {
                    GUILayout.Label("相机预览", EditorStyles.boldLabel);
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    float maxSize = 400f; // 最大宽度和高度
                    float displayWidth = maxSize, displayHeight = maxSize;

                    if (aspectRatio > 1)
                    {
                        // 宽度更大，限制宽度，高度按比例缩放
                        displayWidth = maxSize;
                        displayHeight = maxSize / aspectRatio;
                    }
                    else
                    {
                        // 高度更大，限制高度，宽度按比例缩放
                        displayHeight = maxSize;
                        displayWidth = maxSize * aspectRatio;
                    }

                    // 生成显示区域
                    Rect textureRect = GUILayoutUtility.GetRect(displayWidth, displayHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

                    EditorGUI.DrawPreviewTexture(textureRect, vfxEditor.effectRT);
                    GUILayout.Space(5);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                GUILayout.Label("烘焙设置", EditorStyles.boldLabel);
                GUILayout.Space(3);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Height(300));
                {
                    frameRate = IntFieldAutoWidth("帧率", frameRate);
                    if(vfxEditor)
                    {
                        vfxEditor.frameRate = frameRate;
                    }
                    textureWidth = Mathf.Max(IntFieldAutoWidth("序列帧图片宽度", textureWidth),16);
                    textureHeight = Mathf.Max(IntFieldAutoWidth("序列帧图片高度", textureHeight),16);
                    if (vfxEditor != null)
                    {
                        vfxEditor.UpdateEffectRTSize(textureWidth, textureHeight); // 传递新的宽度和高度
                    }
                    viewSize = FloatFieldAutoWidth("相机视图大小", viewSize);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("相机位置", GUILayout.Width(84));
                    cameraPosition = EditorGUILayout.Vector3Field("", cameraPosition);
                    EditorGUILayout.EndHorizontal();
                    captureflipZ = EditorGUILayout.Toggle("左右翻转", captureflipZ);
                    captureflipX = EditorGUILayout.Toggle("前后翻转", captureflipX);
                    if (modelPivot != null)
                    {
                        modelPivot.transform.localScale = new Vector3(
                            captureflipX ? -1 : 1,
                            modelPivot.transform.localScale.y,
                            captureflipZ ? 1 : -1
                        );
                        if(vfxEditor.modelPivot != null)
                        {
                            vfxEditor.modelPivot.transform.localScale = new Vector3(
                                captureflipX ? -1 : 1,
                                vfxEditor.modelPivot.transform.localScale.y,
                                captureflipZ ? 1 : -1
                            );
                        }

                    }
                    GUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("法线强度", "调整3D模型法线贴图对序列帧图影响程度"), GUILayout.Width(100));
                    normalIntensity = EditorGUILayout.Slider(normalIntensity, 0f, 1f);
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("序列帧输出路径", GUILayout.Width(100));
                    outputFolder = EditorGUILayout.TextField(outputFolder);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    GUILayout.Label("特效烘焙设置", EditorStyles.boldLabel);
                    GUILayout.Space(3);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Height(30));
                    {
                        // 是否半透明选项
                        // effectTransparency = EditorGUILayout.Toggle("特效是否半透明", effectTransparency);
                        // // 背景颜色设置
                        // effectBackgroundColor = EditorGUILayout.ColorField("特效背景颜色", effectBackgroundColor);
                        effectDoubleRes = EditorGUILayout.Toggle("特效是否双倍分辨率", effectDoubleRes);
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                // Assets
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Width(Screen.width / 2));
                {
                    GUILayout.Label("Assets", EditorStyles.boldLabel);
                    GameObject newTarget = (GameObject)EditorGUILayout.ObjectField("Prefab", target, typeof(GameObject), true);
                    if (newTarget != target)
                    {
                        target = newTarget;
                        MoveTargetToModelPivot();

                        // 将分配的 Prefab 传递给 VFXEditor
                        vfxEditor.SetModelPrefab(targetInstance);
                        animSelections.Clear(); // 清空选择列表
                        for (int i = 0; i < animPaths.Count; i++)
                        {
                            animSelections.Add(false);
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("3d动画文件路径(拖到此处)", GUILayout.Width(147));
                    Rect dropArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
                    GUI.Label(dropArea, folderPath, EditorStyles.textField);
                    EditorGUILayout.EndHorizontal();

                    HandleDragAndDrop(dropArea);

                    EditorGUILayout.Space();
                    Rect animsRect = EditorGUILayout.BeginVertical();
                    EditorGUI.DrawRect(animsRect, backgroundColor * 0.78f);
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                    if (animPaths.Count > 0)
                    {
                        GUILayout.Label("选择动画文件", EditorStyles.boldLabel);

                        int previousSelectedAnimIndex = selectedAnimIndex;
                        selectedAnimIndex = GUILayout.SelectionGrid(selectedAnimIndex, showAnimPaths.ToArray(), 1);
                        if (selectedAnimIndex >= 0 && selectedAnimIndex < animPaths.Count)
                        {
                            if (selectedAnimIndex != previousSelectedAnimIndex) // 只有在索引改变时才调用
                            {
                                for (int i = 0; i < animSelections.Count; i++)
                                {
                                    animSelections[i] = (i == selectedAnimIndex);
                                }

                                vfxEditor.SetAnimationClip(animPaths[selectedAnimIndex]);
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    vfxEditor.OnGUI();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            DrawRenderButton("character");

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            Repaint();
        }
        private void DrawStaticCaptureGUI()
        {
            EditorGUILayout.BeginHorizontal();

            float aspectRatio = (float)textureWidth / textureHeight; 
            float maxSize = 400f; // 最大宽度和高度
            float displayWidth = maxSize, displayHeight = maxSize;

            if (aspectRatio > 1)
            {
                // 宽度更大，限制宽度，高度按比例缩放
                displayWidth = maxSize;
                displayHeight = maxSize / aspectRatio;
            }
            else
            {
                // 高度更大，限制高度，宽度按比例缩放
                displayHeight = maxSize;
                displayWidth = maxSize * aspectRatio;
            }

            // 左侧：相机预览和烘焙设置
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width * 0.45f), GUILayout.ExpandHeight(true));
            {
                // 相机预览区域
                GUILayout.Label("相机预览", EditorStyles.boldLabel);
                Rect previewRect = GUILayoutUtility.GetRect(displayWidth, displayHeight);
                if (vfxEditor.effectRT != null)
                {
                    EditorGUI.DrawPreviewTexture(previewRect, vfxEditor.effectRT);
                }
                else
                {
                    GUILayout.Label("RenderTexture 未初始化");
                }
                GUILayout.FlexibleSpace();

                // 烘焙设置 HelpBox
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(position.height / 2));
                {
                    GUILayout.Label("烘焙设置", EditorStyles.boldLabel);
                    textureWidth = IntFieldAutoWidth("预览图片高度", textureWidth);
                    textureHeight = IntFieldAutoWidth("预览图片高度", textureHeight);
                    viewSize = FloatFieldAutoWidth("相机视图大小", viewSize);
                    cameraPosition = EditorGUILayout.Vector3Field("相机位置", cameraPosition);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("法线强度", "调整3D模型法线贴图对序列帧图影响程度"), GUILayout.Width(100));
                    normalIntensity = EditorGUILayout.Slider(normalIntensity, 0f, 1f);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Label(new GUIContent("模型Y轴旋转", "调整模型绕Y轴的旋转角度"), GUILayout.Width(100));
                    float newRotationY = EditorGUILayout.Slider(staticModelPivot != null ? staticModelPivot.transform.localEulerAngles.y : 0, 0f, 360f);

                    if (staticModelPivot != null && Mathf.Abs(newRotationY - staticModelPivot.transform.localEulerAngles.y) > 0.1f)
                    {
                        staticModelPivot.transform.localEulerAngles = new Vector3(
                            staticModelPivot.transform.localEulerAngles.x,
                            newRotationY,
                            staticModelPivot.transform.localEulerAngles.z
                        );
                    }

                    if (staticModelPivotEffect != null && Mathf.Abs(newRotationY - staticModelPivotEffect.transform.localEulerAngles.y) > 0.1f)
                    {
                        staticModelPivotEffect.transform.localEulerAngles = new Vector3(
                            staticModelPivotEffect.transform.localEulerAngles.x,
                            newRotationY,
                            staticModelPivotEffect.transform.localEulerAngles.z
                        );
                    }


                    EditorGUILayout.BeginHorizontal();
                    GameObject newTarget = (GameObject)EditorGUILayout.ObjectField("Prefab", staticTarget, typeof(GameObject), true);
                    if (newTarget != staticTarget)
                    {
                        staticTarget = newTarget;
                        MoveTargetToStaticModelPivot();
                    }
                    EditorGUILayout.EndHorizontal();

                    DrawRenderButton("staticCharacter");
                    
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            // 右侧：材质效果预览和光照设置
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                // 材质效果预览区域
                GUILayout.Label("材质效果预览", EditorStyles.boldLabel);
                Rect materialPreviewRect = GUILayoutUtility.GetRect(displayWidth, displayHeight);
                if (baseColorRT != null && normalRT != null && mixRT != null)
                {
                    Shader lightingShader = Shader.Find("CRLuo/SpritePreview");
                    Material lightingMaterial = new Material(lightingShader);
                    if(showStaticOutLine)
                    {
                        lightingMaterial.SetFloat("_EnableOutline", 1.0f);
                    }
                    else
                    {
                        lightingMaterial.SetFloat("_EnableOutline", 0);
                    }
                    //Material previewMaterial = staticTargetInstance.GetComponent<Renderer>().sharedMaterial;
                    string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                    resultTexture = MTPPreviewManager.DrawMaterialPreview(lightingMaterial, baseColorRT, normalRT, mixRT, textureWidth, textureHeight, scriptPath);
                    EditorGUI.DrawPreviewTexture(materialPreviewRect, resultTexture);
                }
                else
                {
                    // 默认显示纯黑背景
                    EditorGUI.DrawRect(materialPreviewRect, Color.black);
                }

                GUILayout.FlexibleSpace();
                // 光照设置 HelpBox
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(position.height / 2));
                {
                    GUILayout.Label("光照设置", EditorStyles.boldLabel);
                    lightAngleX = EditorGUILayout.Slider("X 轴", lightAngleX, 0f, 360f);
                    lightAngleY = EditorGUILayout.Slider("Y 轴", lightAngleY, 0f, 360f);
                    UpdateLightRotation();
                    //showStaticOutLine = EditorGUILayout.Toggle("显示描边", showStaticOutLine);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            Repaint();
        }

        private void DrawBulletCaptureGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);
                EditorGUILayout.BeginVertical( GUILayout.Width(300));
                {
                    GUILayout.Label("相机预览", EditorStyles.boldLabel);
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    Rect textureRect = GUILayoutUtility.GetAspectRect(1, GUILayout.Width(300), GUILayout.Height(300));
                    EditorGUI.DrawPreviewTexture(textureRect, vfxEditor.effectRT);
                    GUILayout.Space(5);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                GUILayout.Label("烘焙设置", EditorStyles.boldLabel);
                GUILayout.Space(3);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox,  GUILayout.ExpandHeight(true), GUILayout.Height(300));
                {
                    frameRate = IntFieldAutoWidth("帧率", frameRate);
                    textureWidth = IntFieldAutoWidth("序列帧图片宽度", textureWidth);
                    textureHeight = IntFieldAutoWidth("序列帧图片高度", textureHeight);
                    viewSize = FloatFieldAutoWidth("相机视图大小", viewSize);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("相机位置", GUILayout.Width(84));
                    cameraPosition = EditorGUILayout.Vector3Field("", cameraPosition);
                    EditorGUILayout.EndHorizontal();      
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("序列帧输出路径", GUILayout.Width(100));
                    bulletOutputFolder = EditorGUILayout.TextField(bulletOutputFolder);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
            {
                GUILayout.Label("特效设置", EditorStyles.boldLabel);
                // 特效Prefab选择
                GameObject newEffectPrefab = (GameObject)EditorGUILayout.ObjectField("特效Prefab", effectPrefab, typeof(GameObject), true);
                if (newEffectPrefab != effectPrefab)
                {
                    effectPrefab = newEffectPrefab;
                    MoveEffectToModelPivot();
                    MTPBulletEffect.LoadEffectPrefab(ref bulletInstanceEffect, bulletPivotEffect, effectPrefab);
                }
                effectScale = EditorGUILayout.Vector3Field("Scale", effectScale);
                effectRotation = EditorGUILayout.Vector3Field("Rotation", effectRotation);
                // 是否半透明选项
                effectTransparency = EditorGUILayout.Toggle("特效是否半透明", effectTransparency);   
            }
            EditorGUILayout.EndVertical();
            if (effectPrefab != null)
            {
                GUILayout.Label("动画控制", EditorStyles.boldLabel);
                float psLength = MTPBulletEffect.GetTotalFrame(bulletInstanceEffect);
                int totalFrames = 50;
                totalFrames = Mathf.FloorToInt(psLength * frameRate);
                bulletCurrentFrame = EditorGUILayout.IntSlider("当前显示帧数", bulletCurrentFrame, 0, totalFrames);

                Animator effectAnimator = bulletInstanceEffect.GetComponent<Animator>();

                if(effectAnimator != null)
                {
                    float effectNormalizedTime = (float)(bulletCurrentFrame) / (frameRate);
                    effectAnimator.Play(0, 0, effectNormalizedTime);
                    effectAnimator.speed = 0;
                }
                else
                {
                    //粒子特效的情况
                    ParticleSystem[] particleSystems = bulletInstanceEffect.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (ParticleSystem ps in particleSystems)
                    {
                        // 计算粒子系统的 normalizedTime
                        float particleSystemNormalizedTime = (float)(bulletCurrentFrame) / (frameRate);
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        ps.useAutoRandomSeed = false;
                        particleSystemNormalizedTime = Mathf.Clamp01(particleSystemNormalizedTime);
                        ps.Simulate(particleSystemNormalizedTime , true, true);
                        ps.Pause();
                    }
                }
                


                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            DrawRenderButton("bulletEffect");
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            Repaint();
        }
        private static Material transparentMaterial;

        // 初始化透明材质的方法
        private static Material GetTransparentMaterial()
        {
            if (transparentMaterial == null)
            {
                // 使用内置的透明 Shader
                Shader shader = Shader.Find("Custom/Checkerboard");
                transparentMaterial = new Material(shader);
            }
            return transparentMaterial;
        }
        private void DrawPreviewGUI()
        {
            selectedPreviewBulletAnimIndex = -1; 
            previewSprite_bulletEffect.enabled = false;
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            // 上半部分：动画预览和预览设置、文件选择
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);
                
                // 左上部分：动画预览
                EditorGUILayout.BeginVertical(GUILayout.Width(300));
                {
                    GUILayout.Label("序列帧预览", EditorStyles.boldLabel);
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    if (previewRenderTexture != null)
                    {
                        Rect previewRect = GUILayoutUtility.GetRect(300, 300);
                        EditorGUI.DrawPreviewTexture(previewRect, previewRenderTexture, GetTransparentMaterial(), ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Render Texture未找到或未创建", GUILayout.Height(300), GUILayout.Width(300));
                    }
                    GUILayout.Space(5);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("播放"))
                    {
                        MTPPreviewManager.PlayAnimation(previewSprite, previewSprite_effect);
                    }
                    if (GUILayout.Button("暂停"))
                    {
                        MTPPreviewManager.PauseAnimation(previewSprite, previewSprite_effect);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                // 右上部分：预览设置和文件选择
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label("预览设置", EditorStyles.boldLabel);
                    GUILayout.Space(3);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Height(110));
                    {
                        GUILayout.Label("动画设置", EditorStyles.boldLabel);

                        EditorApplication.QueuePlayerLoopUpdate();
                        SceneView.RepaintAll();

                        flipX = EditorGUILayout.Toggle("左右翻转", flipX);
                        showOutLine = EditorGUILayout.Toggle("显示描边", showOutLine);
                        Material material = previewSprite.sharedMaterial;
                        if(showOutLine)
                        {
                            material.SetFloat("_EnableOutline", 1.0f);
                        }
                        else
                        {
                            material.SetFloat("_EnableOutline", 0);
                        }

                        if (previewSprite != null)
                        {
                            previewSprite.flipX = flipX;
                        }
                        if (previewSprite_effect != null)
                        {
                            previewSprite_effect.flipX = flipX;
                        }
                        GUILayout.Space(5);
                        
                        // 光照角度调整
                        GUILayout.Label("光照角度", EditorStyles.boldLabel);
                        lightAngleX = EditorGUILayout.Slider("X 轴", lightAngleX, 0f, 360f);
                        lightAngleY = EditorGUILayout.Slider("Y 轴", lightAngleY, 0f, 360f);
                        UpdateLightRotation();
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(5);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Height(210));
                    {
                        GUILayout.Label("动画文件路径", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("2d动画文件路径(拖到此处)", GUILayout.Width(150));
                        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
                        string previousPreviewFolderPath = previewFolderPath;
                        GUI.Label(dropArea, previewFolderPath, EditorStyles.textField);
                        EditorGUILayout.EndHorizontal();


                        HandleDragAndDrop(dropArea, ref previewFolderPath, ref previewAnimPaths, ref showPreviewAnimPaths);

                        GUILayout.Space(5);
                        GUILayout.Label("选择预览动画", EditorStyles.boldLabel);

                        Rect bakingRect = EditorGUILayout.BeginVertical();
                        EditorGUI.DrawRect(bakingRect, backgroundColor * 0.78f);
                        {
                            GUILayout.Space(10);

                            previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition);

                            if (previewAnimPaths.Count > 0)
                            {
                                int newSelectedAnimIndex = GUILayout.SelectionGrid(selectedPreviewAnimIndex, showPreviewAnimPaths.ToArray(), 1);

                                if (newSelectedAnimIndex != selectedPreviewAnimIndex)
                                {
                                    selectedPreviewAnimIndex = newSelectedAnimIndex;
                                    selectedFrames.Clear();
                                    if (selectedPreviewAnimIndex >= 0 && selectedPreviewAnimIndex < previewAnimPaths.Count)
                                    {
                                        string selectedAnimPath = previewAnimPaths[selectedPreviewAnimIndex];
                                        if (!EditorApplication.isPlaying)
                                        {
                                            EditorUtility.DisplayDialog("Play Mode Required", "提示：请在play模式进行", "确认");
                                            return; // 提示用户手动进入播放模式
                                        }
                                        else
                                        {
                                            MTPPreviewManager.PreviewAnimation(selectedAnimPath, ref previewSprite, ref previewSprite_effect, ref previewClip, ref basePath, ref normalPath, ref effectPath, ref mixPath);
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            // 下半部分：帧预览
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawPreviewFrames();
            EditorGUILayout.EndVertical();

            // 底部：导出部分
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                // 左边：导出单帧贴图部分
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width * 0.5f));
                {
                    GUILayout.Label("导出单帧贴图", EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("合成序列帧导出路径:", GUILayout.Width(130));
                    exportPath = GUILayout.TextField(exportPath, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("编辑器用贴图导出路径:", GUILayout.Width(130));
                    editorExportPath = GUILayout.TextField(editorExportPath, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                    useCustomName = EditorGUILayout.Toggle("使用自定义名称", useCustomName);

                    // 如果启用了自定义名称，显示输入框
                    if (useCustomName)
                    {
                        customExportName = EditorGUILayout.TextField("自定义导出名称", customExportName);
                    }
                    //Debug.Log(useCustomName);
                    else if(!useCustomName)
                    {
                        customExportName = null;
                    }
                    GUILayout.Space(10);

                    GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 20,
                        fontStyle = FontStyle.Bold
                    };
                    if (GUILayout.Button("导出选中帧贴图", renderButtonStyle, GUILayout.Height(50)))
                    {
                        if (selectedFrames.Count > 0)
                        {
                            MTPExportManager.ExportSelectedFrames(selectedFrames, exportPath, previewAnimPaths[selectedPreviewAnimIndex], basePath, normalPath, effectPath, mixPath,customExportName);
                            MTPExportManager.ExportSelectedFramesForEditor(selectedFrames, editorExportPath, previewAnimPaths[selectedPreviewAnimIndex], basePath, normalPath, effectPath, mixPath, previewClip,customExportName);
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                // 右边：新增的动画导出序列帧功能
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
                {
                    GUILayout.Label("通过动画导出序列帧", EditorStyles.boldLabel);

                    selectedAnimClip = (AnimationClip)EditorGUILayout.ObjectField("选择动画文件", selectedAnimClip, typeof(AnimationClip), false);
                    int spaceValue = useCustomName ? 70 : 50;
                    GUILayout.Space(spaceValue);

                    GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 20,
                        fontStyle = FontStyle.Bold
                    };

                    if (GUILayout.Button("导出序列帧", renderButtonStyle, GUILayout.Height(50)))
                    {
                        MTPExportManager.ExportAnimationToSpriteSheet(selectedAnimClip, exportPath, frameRate);
                        //PerformRecovery(selectedAnimClip);
                    }
                
                }
                EditorGUILayout.EndVertical();
            }



            EditorGUILayout.EndHorizontal();

            Repaint();
        }

        private void DrawBulletPreviewGUI()
        {
            selectedPreviewAnimIndex = -1; 
            previewSprite.enabled = false;
            previewSprite_effect.enabled = false;
            

            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            // 上半部分：动画预览和预览设置、文件选择
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);
                
                // 左上部分：动画预览
                EditorGUILayout.BeginVertical(GUILayout.Width(300));
                {
                    GUILayout.Label("序列帧预览", EditorStyles.boldLabel);
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    if (previewRenderTexture != null)
                    {
                        Rect previewRect = GUILayoutUtility.GetRect(300, 300);
                        EditorGUI.DrawPreviewTexture(previewRect, previewRenderTexture);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Render Texture未找到或未创建", GUILayout.Height(300), GUILayout.Width(300));
                    }
                    GUILayout.Space(5);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("播放"))
                    {
                        MTPPreviewManager.PlayAnimation(previewSprite_bulletEffect, previewSprite_bulletEffect);
                    }
                    if (GUILayout.Button("暂停"))
                    {
                        MTPPreviewManager.PauseAnimation(previewSprite_bulletEffect, previewSprite_bulletEffect);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                // 右上部分：预览设置和文件选择
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label("预览设置", EditorStyles.boldLabel);
                    GUILayout.Space(3);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.Height(328));
                    {
                        GUILayout.Label("动画文件路径", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("2d动画文件路径(拖到此处)", GUILayout.Width(150));
                        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true));
                        GUI.Label(dropArea, previewBulletFolderPath, EditorStyles.textField);
                        EditorGUILayout.EndHorizontal();


                        HandleDragAndDropForBullet(dropArea, ref previewBulletFolderPath, ref previewBulletAnimPaths, ref showPreviewBulletAnimPaths);

                        GUILayout.Space(5);
                        GUILayout.Label("选择预览动画", EditorStyles.boldLabel);

                        Rect bakingRect = EditorGUILayout.BeginVertical();
                        EditorGUI.DrawRect(bakingRect, backgroundColor * 0.78f);
                        {
                            GUILayout.Space(10);

                            previewScrollPosition = EditorGUILayout.BeginScrollView(previewBulletScrollPosition);

                            if (previewBulletAnimPaths.Count > 0)
                            {
                                int newSelectedAnimIndex = GUILayout.SelectionGrid(selectedPreviewBulletAnimIndex, showPreviewBulletAnimPaths.ToArray(), 1);

                                if (newSelectedAnimIndex != selectedPreviewBulletAnimIndex)
                                {
                                    selectedPreviewBulletAnimIndex = newSelectedAnimIndex;
                                    selectedFrames.Clear();
                                    if (selectedPreviewBulletAnimIndex >= 0 && selectedPreviewBulletAnimIndex < previewBulletAnimPaths.Count)
                                    {
                                        string selectedAnimPath = previewBulletAnimPaths[selectedPreviewBulletAnimIndex];
                                        if (!EditorApplication.isPlaying)
                                        {
                                            EditorUtility.DisplayDialog("Play Mode Required", "提示：请在play模式进行", "确认");
                                            return; // 提示用户手动进入播放模式
                                        }
                                        else
                                        {
                                            MTPPreviewManager.PreviewBulletAnimation(selectedAnimPath, ref previewSprite_bulletEffect, ref previewClip, ref effectPath);
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            // 下半部分：帧预览
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            DrawPreviewFrames();
            EditorGUILayout.EndVertical();

            // 底部：导出部分
            GUILayout.Space(10);
 
            // 左边：导出单帧贴图部分
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            {
                GUILayout.Label("导出单帧贴图", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Label("合成序列帧导出路径:", GUILayout.Width(130));
                exportPath = GUILayout.TextField(exportPath, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                };
                if (GUILayout.Button("导出选中帧贴图", renderButtonStyle, GUILayout.Height(50)))
                {
                    if (selectedFrames.Count > 0)
                    {
                        MTPExportManager.ExportSelectedFramesBullet(selectedFrames, exportPath, previewBulletAnimPaths[selectedPreviewBulletAnimIndex], effectPath);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            





            Repaint();
        }

        public static void ReleaseRenderTextures()
        {
            foreach (var rt in previewRenderTextures)
            {
                if (rt != null)
                {
                    rt.Release();
                }
            }
            previewRenderTextures.Clear();
        }

        private void DrawPreviewFrames()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.Label("单帧预览", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (previewRenderTextures != null && previewRenderTextures.Count > 0)
            {
                int frameHeight = 200;
                int frameWidth = 200;
                int frameSpacing = 3;

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                int columns = Mathf.FloorToInt(position.width / (frameWidth + frameSpacing));
                int rows = Mathf.CeilToInt((float)previewRenderTextures.Count / columns);

                // 从上到下，从左到右，确保顺序正确
                for (int row = 0; row < rows; row++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int col = 0; col < columns; col++)
                    {
                        // 计算在图集中的正确索引
                        int index = row * columns + col;
                        if (index >= previewRenderTextures.Count)
                            break;

                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(frameWidth), GUILayout.Height(frameHeight));
                        Rect frameRect = GUILayoutUtility.GetRect(frameWidth, frameHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                        EditorGUI.DrawPreviewTexture(frameRect, previewRenderTextures[index]);

                        if (GUI.Button(frameRect, GUIContent.none, GUIStyle.none))
                        {
                            if (selectedFrames.Contains(index))
                            {
                                selectedFrames.Remove(index);
                            }
                            else
                            {
                                selectedFrames.Add(index);
                            }
                        }

                        if (selectedFrames.Contains(index))
                        {
                            Handles.color = Color.red;
                            Handles.DrawSolidRectangleWithOutline(new Rect(frameRect.x, frameRect.y, frameWidth, frameHeight + 20), new Color(1, 0, 0, 0.25f), Color.red);
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"第{index}帧", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter }, GUILayout.Height(20));
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        if (frameRect.Contains(Event.current.mousePosition))
                        {
                            Handles.color = Color.yellow;
                            Handles.DrawSolidRectangleWithOutline(new Rect(frameRect.x, frameRect.y, frameWidth, frameHeight + 20), new Color(1, 1, 0, 0.25f), Color.green);
                            Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("全选"))
                {
                    selectedFrames.Clear();
                    for (int i = 0; i < previewRenderTextures.Count; i++)
                    {
                        selectedFrames.Add(i);
                    }
                }
                if (GUILayout.Button("反选"))
                {
                    List<int> newSelectedFrames = new List<int>();
                    for (int i = 0; i < previewRenderTextures.Count; i++)
                    {
                        if (!selectedFrames.Contains(i))
                        {
                            newSelectedFrames.Add(i);
                        }
                    }
                    selectedFrames = newSelectedFrames;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void ResetTarget()
        {
            if(targetInstance)
            DestroyImmediate(targetInstance);
            if(staticTargetInstance)
            DestroyImmediate(staticTargetInstance);
            if(vfxEditor.targetInstance)
            DestroyImmediate(vfxEditor.targetInstance);
            target = null;
            staticTarget = null;
            vfxEditor.targetInstance = null;

            if(bulletInstance)
            DestroyImmediate(bulletInstance);
            if(bulletInstanceEffect)
            DestroyImmediate(bulletInstanceEffect);
            effectPrefab = null;
            bulletInstance = null;
        }

        private void UpdateLightRotation()
        {
            Vector3 lightDir = Quaternion.Euler(lightAngleX, lightAngleY, 0) * Vector3.forward;
            Shader.SetGlobalVector("_PreviewCharacterLightDir", lightDir);
        }

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == requiredSceneName)
            {
                if (captureCamera == null)
                captureCamera = GameObject.Find(captureCameraName)?.GetComponent<Camera>();

                if (effectRTCamera == null)
                effectRTCamera = GameObject.Find("EffectRTCamera")?.GetComponent<Camera>();
                
                if (previewRTCamera == null)
                    previewRTCamera = GameObject.Find(previewRTCameraName)?.GetComponent<Camera>();

                if (vfxEditor == null)
                {
                    vfxEditor = CreateInstance<VFXEditor>(); // 实例化VFX编辑器
                    vfxEditor.OnEnable(); // 调用VFX编辑器的OnEnable方法
                }
                if (previewSprite == null)
                {
                    previewSprite = GameObject.Find("PreviewSprite")?.GetComponent<SpriteRenderer>();
                }
                if (previewSprite_effect == null)
                {
                    previewSprite_effect = GameObject.Find("PreviewSprite_effect")?.GetComponent<SpriteRenderer>();
                }
                if (previewSprite_bulletEffect == null)
                {
                    previewSprite_bulletEffect = GameObject.Find("PreviewSprite_bulletEffect")?.GetComponent<SpriteRenderer>();
                }

                bulletPivot = FindInactiveObjectByName("BulletPivot");
                bulletPivotEffect = FindInactiveObjectByName("BulletPivotEffect");
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
            LoadSettings();
        }

        private void SaveSettings()
        {
            EditorPrefs.SetInt("FrameRate", frameRate);
            EditorPrefs.SetInt("TextureWidth", textureWidth);
            EditorPrefs.SetInt("TextureHeight", textureHeight);
            EditorPrefs.SetFloat("ViewSize", viewSize);
            EditorPrefs.SetString("OutputFolder", outputFolder);
            EditorPrefs.SetFloat("NormalIntensity", normalIntensity);
            EditorPrefs.SetString("ExportPath", exportPath);
            EditorPrefs.SetString("editorExportPath", editorExportPath);
            EditorPrefs.SetFloat("CameraPositionX",cameraPosition.x);
            EditorPrefs.SetFloat("CameraPositionY",cameraPosition.y);
            EditorPrefs.SetFloat("CameraPositionZ",cameraPosition.z);
            EditorPrefs.SetFloat("lightAngleX",lightAngleX);
            EditorPrefs.SetFloat("lightAngleY",lightAngleY);
        }

        private void LoadSettings()
        {
            frameRate = EditorPrefs.GetInt("FrameRate", frameRate); // 默认值为30
            textureWidth = EditorPrefs.GetInt("TextureWidth", textureWidth); // 默认值为128
            textureHeight = EditorPrefs.GetInt("TextureHeight", textureHeight); // 默认值为128
            viewSize = EditorPrefs.GetFloat("ViewSize", viewSize); // 默认值为2f
            outputFolder = EditorPrefs.GetString("OutputFolder", outputFolder);
            normalIntensity = EditorPrefs.GetFloat("NormalIntensity", normalIntensity); // 默认值为0
            exportPath = EditorPrefs.GetString("ExportPath", exportPath);
            editorExportPath = EditorPrefs.GetString("editorExportPath", editorExportPath);
            cameraPosition = new Vector3(EditorPrefs.GetFloat("CameraPositionX",cameraPosition.x),
            EditorPrefs.GetFloat("CameraPositionY",cameraPosition.y),
            EditorPrefs.GetFloat("CameraPositionZ",cameraPosition.z));
            EditorPrefs.GetFloat("lightAngleX",lightAngleX);
            EditorPrefs.GetFloat("lightAngleY",lightAngleY);
        }

        private void OnDisable()
        {
            if (SceneManager.GetActiveScene().name == requiredSceneName)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                selectedPreviewAnimIndex = -1;
                selectedAnimIndex = -1;
                ResetTarget();
                CleanupResources();
                ReleaseRenderTextures();
                if (vfxEditor != null)
                {
                    vfxEditor.OnDisable(); // 调用VFX编辑器的OnDisable方法
                    DestroyImmediate(vfxEditor);
                    vfxEditor = null;
                }
            }
            SaveSettings();
        }

        private void OnDestroy()
        {
            CleanupResources();
        }

        private void CleanupResources()
        {
            if (previewRenderTexture != null)
            {
                previewRenderTexture.Release();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ResetTarget();
                ReleaseRenderTextures();
                selectedPreviewAnimIndex = -1;
                selectedAnimIndex = -1;
            }
        }

        private void LoadAnimationsInFolder(string folderPath,List<string> animList, List<string> showAnimList, List<bool> selectionList)
        {
            animList.Clear();
            showAnimList.Clear();
            if (selectionList != null)
            {
                selectionList.Clear();
            }

            string[] animations = Directory.GetFiles(folderPath, "*.anim", SearchOption.AllDirectories);

            foreach (string anim in animations)
            {
                string animName = Path.GetFileName(anim);
                showAnimList.Add(animName);
                animList.Add(anim);
                if (selectionList != null)
                {
                    selectionList.Add(false);
                }
            }
        }
        private void LoadPreviewAnimations(string folderPath,List<string> animList, List<string> showAnimList)
        {
            animList.Clear();
            showAnimList.Clear();

            string[] animations = Directory.GetFiles(folderPath, "*.anim", SearchOption.AllDirectories);

            foreach (string anim in animations)
            {
                string animName = Path.GetFileName(anim);
                string animNameWithoutExtension = Path.GetFileNameWithoutExtension(anim);

                // 过滤掉后缀是 "effect" 的动画文件
                if (!animNameWithoutExtension.EndsWith("effect", System.StringComparison.OrdinalIgnoreCase))
                {
                    showAnimList.Add(animName);
                    animList.Add(anim);
                }
            }

        }
        private void LoadPreviewAnimationsForBullet(string folderPath,List<string> animList, List<string> showAnimList)
        {
            animList.Clear();
            showAnimList.Clear();

            string[] animations = Directory.GetFiles(folderPath, "*.anim", SearchOption.AllDirectories);

            foreach (string anim in animations)
            {
                string animName = Path.GetFileName(anim);
                string animNameWithoutExtension = Path.GetFileNameWithoutExtension(anim);

                // 过滤掉后缀是 "effect" 的动画文件
                if (animNameWithoutExtension.EndsWith("bulletEffect", System.StringComparison.OrdinalIgnoreCase))
                {
                    showAnimList.Add(animName);
                    animList.Add(anim);
                }
            }

        }

        private void MoveTargetToModelPivot()
        {
            // 删除现有的实例
            if (targetInstance != null)
            {
                DestroyImmediate(targetInstance);
            }

            if (target != null)
            {
                modelPivot = FindInactiveObjectByName("ModelPivot");
                modelPivotEffect = FindInactiveObjectByName("ModelPivotEffect");

                // 实例化新的Prefab
                targetInstance = Instantiate(target);
                targetInstance.transform.SetParent(modelPivot.transform, false);
                targetInstance.name = target.name;
                targetInstance.SetActive(true);

                string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                string scriptFolder = Path.GetDirectoryName(scriptPath);
                string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");

                RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{parentFolder}/AnimController/MTPController.controller");

                if (controller != null)
                {
                    Animator animator = targetInstance.GetComponent<Animator>();
                    if (animator == null)
                    {
                        animator = targetInstance.AddComponent<Animator>();
                    }
                    animator.runtimeAnimatorController = controller;
                    animator.speed = 0;

                    // 将分配的 Prefab 传递给 VFXEditor
                    vfxEditor.SetModelPrefab(targetInstance);
                }
                else
                {
                    Debug.LogError("MTPController.controller not found in the same folder as the target prefab.");
                }
            }
        }

        private void MoveTargetToStaticModelPivot()
        {
            // 删除现有的实例
            if (staticTargetInstance != null)
            {
                DestroyImmediate(staticTargetInstance);
            }
            if (staticTargetEffectInstance != null)
            {
                DestroyImmediate(staticTargetEffectInstance);
            }

            if (staticTarget != null)
            {
                staticModelPivot = FindInactiveObjectByName("StaticModelPivot");
                staticModelPivotEffect = FindInactiveObjectByName("StaticModelPivotEffect");

                // 实例化新的Prefab
                staticTargetInstance = Instantiate(staticTarget);
                staticTargetInstance.transform.SetParent(staticModelPivot.transform, false);
                staticTargetInstance.name = staticTarget.name;
                staticTargetInstance.SetActive(true);

                staticTargetEffectInstance = Instantiate(staticTarget);
                staticTargetEffectInstance.transform.SetParent(staticModelPivotEffect.transform, false);
                staticTargetEffectInstance.name = staticTarget.name;
                staticTargetEffectInstance.SetActive(true);
            }
        }

        GameObject FindInactiveObjectByName(string name)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
            return null;
        }

        private void MoveEffectToModelPivot()
        {
        // 删除现有的实例
            if (bulletInstance != null)
            {
                DestroyImmediate(bulletInstance);
            }

            if (effectPrefab != null)
            {
                // 实例化新的Prefab
                bulletInstance = Instantiate(effectPrefab);
                bulletInstance.transform.SetParent(bulletPivot.transform, false);
                bulletInstance.name = effectPrefab.name;
                bulletInstance.SetActive(true);
                ParticleSystem[] particleSystems = bulletInstance.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem ps in particleSystems)
                {
                    //ps.Simulate(0.1f , true, true);
                    ps.Pause();
                }
            }
        }

        private void StartStaticModelCapture()
        {
            captureScript = staticTargetInstance.GetComponent<MTPCapture>();
            if (captureScript == null)
            {
                captureScript = staticTargetInstance.AddComponent<MTPCapture>();
            }
            if (staticTargetInstance == null || captureScript == null)
            {
                Debug.LogError("未选择模型或未找到捕捉脚本！");
                return;
            }

            // 初始化捕捉脚本
            captureScript.captureCamera = captureCamera;
            captureScript.target = staticTargetInstance;
            captureScript.textureWidth = textureWidth;
            captureScript.textureHeight = textureHeight;
            captureScript.SetupCapture();

            if (baseColorRT != null) baseColorRT.Release();
            if (normalRT != null) normalRT.Release();
            if (mixRT != null) mixRT.Release();
            // 捕捉 BaseColor
            baseColorRT = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
            captureScript.CaptureFrameToRT(baseColorRT, MTPCapture.CaptureType.BaseColor);

            // 捕捉 Normal
            RenderTextureDescriptor normalDescriptor = new RenderTextureDescriptor(
                textureWidth,
                textureHeight,
                RenderTextureFormat.ARGB32,
                24)
            {
                sRGB = false // 禁用 sRGB
            };
            normalRT = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
            captureScript.CaptureFrameToRT(normalRT, MTPCapture.CaptureType.Normal);

            // // 捕捉 Emissive
            mixRT = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
            captureScript.CaptureFrameToRT(mixRT, MTPCapture.CaptureType.Emissive);
            

            Debug.Log("捕捉完成，临时贴图已更新。");
        }

        private void StartCapture()
        {
            captureScript = targetInstance.GetComponent<MTPCapture>();
            if (captureScript == null)
            {
                captureScript = targetInstance.AddComponent<MTPCapture>();
            }
            captureScript.target = targetInstance;
            captureScript.captureCamera = captureCamera;
            captureScript.previewRTCamera = previewRTCamera;
            captureScript.previewRenderTexture = previewRenderTexture;
            captureScript.outputFolder = outputFolder;
            captureScript.textureWidth = textureWidth;
            captureScript.textureHeight = textureHeight;
            captureScript.normalIntensity = normalIntensity;
            captureScript.effectBackgroundColor = effectBackgroundColor;
            captureScript.effectTransparency = effectTransparency;
            captureScript.effectDoubleRes = effectDoubleRes;
            captureScript.SetupCapture();

            animator = targetInstance.GetComponent<Animator>();
            List<AnimationClip> selectedClips = new List<AnimationClip>();
            for (int i = 0; i < animPaths.Count; i++)
            {
                if (animSelections[i])
                {
                    string animPath = animPaths[i];
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
                    selectedClips.Add(clip);
                }
            }

            // 捕捉基础颜色和法线
            captureScript.StartCoroutine(captureScript.CaptureBothAnimations(selectedClips, animator, frameRate));
        }
        private void StartCaptureForBullet()
        {
            captureScript = bulletInstance.GetComponent<MTPCapture>();
            if (captureScript == null)
            {
                captureScript = bulletInstance.AddComponent<MTPCapture>();
            }
            captureScript.captureCamera = captureCamera;
            captureScript.previewRTCamera = previewRTCamera;
            captureScript.previewRenderTexture = previewRenderTexture;
            captureScript.outputFolder = bulletOutputFolder;
            captureScript.textureWidth = textureWidth;
            captureScript.textureHeight = textureHeight;
            captureScript.effectBackgroundColor = effectBackgroundColor;
            captureScript.effectTransparency = effectTransparency;
            captureScript.SetupCaptureForBullet();

            // 捕捉基础颜色和法线
            captureScript.StartCoroutine(captureScript.CaptureAnimationsForBullet(bulletInstance, frameRate));
        }
    }
}