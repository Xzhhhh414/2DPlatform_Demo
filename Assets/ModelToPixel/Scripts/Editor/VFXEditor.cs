using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModelToPixel
{
    public class VFXEditor : ScriptableObject
    {
        private GameObject effectPrefab;
        private GameObject modelPrefab;
        private Transform bone;
        private int startFrame;
        private int endFrame;
        private int lastStartFrame;
        private int lastEndFrame;
        private Vector3 offset;
        private Vector3 scale = Vector3.one; 
        private AnimationClip animationClip;
        private Animator animator;
        public GameObject targetInstance;
        private int currentFrame = 0;
        private int lastCurrentFrame = 0;
        private int totalFrames = 0;
        private GameObject effectInstance;
        private Animator effectAnimator;

        private Vector3 lastOffset;
        private EffectDataCollection effectDataCollection;
        private string effectDataPath;

        public RenderTexture effectRT;
        private Dictionary<ParticleSystem, ParticleSystem.Particle[]> particleCache = new Dictionary<ParticleSystem, ParticleSystem.Particle[]>();
        public GameObject modelPivot;
        public GameObject effectPivot;
        private Vector2 scrollPos;
        private int selectedBoneIndex = -1;
        private Color backgroundColor;
        public int frameRate = 30;


        public void OnGUI()
        {
            UpdateEffectOffset();

            backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.76f, 0.76f, 0.76f);

            // 右上部分：特效挂点工具设置
            EditorGUILayout.BeginVertical();

            GUILayout.Label("特效配置信息", EditorStyles.boldLabel);
            effectPrefab = (GameObject)EditorGUILayout.ObjectField("特效Prefab", effectPrefab, typeof(GameObject), false);

            // 骨骼选择部分，直接从 modelPivot 的子骨骼中筛选

            if (modelPivot != null)
            {
                Rect animsRect = EditorGUILayout.BeginVertical( GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(animsRect, backgroundColor * 0.78f);
                EditorGUILayout.Space(3);
                GUILayout.Label("选择挂接骨骼:", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));  // 限定高度为150，添加滚动条
                Transform[] bones = modelPivot.GetComponentsInChildren<Transform>(true);
                List<string> boneNames = new List<string>();

                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i] != modelPivot.transform)  // 排除modelPivot本身
                    {
                        boneNames.Add(bones[i].name);
                    }
                }

                selectedBoneIndex = GUILayout.SelectionGrid(selectedBoneIndex, boneNames.ToArray(), 1);

                if (selectedBoneIndex >= 0 && selectedBoneIndex < bones.Length)
                {
                    bone = bones[selectedBoneIndex + 1];
                }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(3);
            EditorGUILayout.EndVertical();
                
            }


            GUILayout.FlexibleSpace(); 
            startFrame = EditorGUILayout.IntField("起始帧数", startFrame);
            endFrame = EditorGUILayout.IntField("结束帧数", endFrame);
            offset = EditorGUILayout.Vector3Field("Offset", offset);
            scale = EditorGUILayout.Vector3Field("Scale", scale);

            if (lastStartFrame != startFrame || lastEndFrame != endFrame || lastCurrentFrame != currentFrame)
            {
                UpdateAnimationFrame();
                lastStartFrame = startFrame;
                lastEndFrame = endFrame;
                lastCurrentFrame = currentFrame;
            }

            GUILayout.Space(10);
            GUIStyle renderButtonStyle = new GUIStyle(GUI.skin.button);
            renderButtonStyle.fontSize = 20;
            renderButtonStyle.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("挂接特效", renderButtonStyle, GUILayout.Height(40)))
            {
                AttachEffect();
            }
            
            if (animationClip != null)
            {
                GUILayout.Label("动画控制", EditorStyles.boldLabel);
                totalFrames = Mathf.FloorToInt(animationClip.length * frameRate);
                currentFrame = EditorGUILayout.IntSlider("当前显示帧数", currentFrame, 0, totalFrames);

                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }

            //GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存特效挂点信息", renderButtonStyle, GUILayout.Height(40)))
            {
                SaveEffectData();
            }
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            // EditorGUILayout.EndHorizontal();
        }

        private void UpdateAnimationFrame()
        {
            if (animator != null)
            {

                float normalizedTime = (float)currentFrame / (totalFrames);
                animator.Play(0, 0, normalizedTime);
                animator.speed = 0;
                // 手动处理特效激活和停用
                if (effectInstance != null)
                {
                    float startTime = (float)startFrame / totalFrames;
                    float endTime = (float)endFrame / totalFrames;

                    if (normalizedTime >= startTime && normalizedTime <= endTime)
                    {
                        if (!effectInstance.activeSelf)
                        {
                            effectInstance.SetActive(true);
                        }
                        if(effectAnimator != null)
                        {
                            AnimationClip effectClip = effectAnimator.runtimeAnimatorController.animationClips[0];
                            int effectTotalFrames = Mathf.FloorToInt(effectClip.length * frameRate);
                            float effectNormalizedTime = (float)(currentFrame - startFrame) / effectTotalFrames;
                            effectAnimator.Play(0, 0, effectNormalizedTime);
                            effectAnimator.speed = 0;
                        }
                        // 处理粒子系统
                        ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                        foreach (ParticleSystem ps in particleSystems)
                        {
                            if (!particleCache.ContainsKey(ps))
                            {
                                particleCache[ps] = new ParticleSystem.Particle[ps.main.maxParticles];
                            }
                            // 计算粒子系统的 normalizedTime
                            float particleSystemNormalizedTime = (float)(currentFrame - startFrame) / (frameRate);

                            particleSystemNormalizedTime = Mathf.Clamp01(particleSystemNormalizedTime);
                            ps.randomSeed = 12345;
                            ps.Simulate(particleSystemNormalizedTime , true, true);
                            ps.Pause();
                        }
                    }
                    else
                    {
                        if (effectInstance.activeSelf)
                        {
                            effectInstance.SetActive(false);
                        }
                    }
                }
            }
        }

        private void UpdateEffectOffset()
        {
            if (effectInstance != null && lastOffset != offset)
            {
                effectInstance.transform.localPosition = offset;
                lastOffset = offset;
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

        private void LoadModelPrefab()
        {
            if (targetInstance != null)
            {
                DestroyImmediate(targetInstance);
            }

            modelPivot = FindInactiveObjectByName("ModelPivotEffect");
            Debug.Log(modelPivot);

            if (modelPivot != null && modelPrefab != null)
            {
                targetInstance = Instantiate(modelPrefab, modelPivot.transform, false);
                targetInstance.name = modelPrefab.name;

                string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                string scriptFolder = Path.GetDirectoryName(scriptPath);
                string parentFolder = FindParentFolder(scriptPath, "ModelToPixel");

                RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>($"{parentFolder}/AnimController/EffectController.controller");

                if (controller != null)
                {
                    animator = targetInstance.GetComponent<Animator>();
                    if (animator == null)
                    {
                        animator = targetInstance.AddComponent<Animator>();
                    }
                    animator.runtimeAnimatorController = controller;
                }
            }
        }

        private void AttachEffect()
        {
            if (effectPrefab == null || modelPrefab == null || bone == null || animationClip == null)
            {
                Debug.LogError("请分配所有必需的字段。");
                return;
            }
            // 确保EffectController附加在Animator的GameObject上
            EffectController effectController = animator.gameObject.GetComponent<EffectController>();
            if (effectController == null)
            {
                effectController = animator.gameObject.AddComponent<EffectController>();
            }

            // 删除现有特效实例
            if (effectInstance != null)
            {
                effectController.UnregisterEffect(effectInstance.name);
                DestroyImmediate(effectInstance);
            }

            effectInstance = Instantiate(effectPrefab, bone);
            effectInstance.transform.localPosition = offset;
            effectInstance.transform.localScale = scale; // 设置默认缩放比例
            effectInstance.SetActive(false); // 确保特效初始激活

            effectAnimator = effectInstance.GetComponent<Animator>();

            // 为特效实例添加唯一名称
            string effectName = effectInstance.name;

            // 将特效实例注册到effectController
            effectController.RegisterEffect(effectName, effectInstance);
        }

        string GetBonePath(Transform bone, Transform parentPrefabTransform)
        {
            if (bone == null || parentPrefabTransform == null)
                return string.Empty;

            StringBuilder path = new StringBuilder();
            Transform current = bone;
            if(current == parentPrefabTransform)
            {
                return string.Empty;
            }

            while (current != null && current.parent != parentPrefabTransform)
            {
                path.Insert(0, current.name);
                path.Insert(0, "/");
                current = current.parent;
            }

            if (current.parent == parentPrefabTransform)
            {
                path.Insert(0, current.name);
            }

            return path.ToString();
        }

        private void SaveEffectData()
        {
            // 尝试加载现有的 EffectDataCollection 资产
            effectDataCollection = AssetDatabase.LoadAssetAtPath<EffectDataCollection>(effectDataPath);
            if (effectDataCollection == null)
            {
                // 如果不存在则创建新的
                effectDataCollection = ScriptableObject.CreateInstance<EffectDataCollection>();
                AssetDatabase.CreateAsset(effectDataCollection, effectDataPath);
            }

            // 查找是否已有相同 animationClip 的数据
            EffectData existingEffectData = effectDataCollection.effects.Find(effect => effect.animationClip == animationClip);

            string bonePath = GetBonePath(bone, targetInstance.transform);

            if (existingEffectData != null)
            {
                // 更新现有数据
                existingEffectData.effectPrefab = effectPrefab;
                existingEffectData.bonePath = bonePath;
                existingEffectData.startFrame = startFrame;
                existingEffectData.endFrame = endFrame;
                existingEffectData.offset = offset;
                existingEffectData.scale = scale;
            }
            else
            {
                // 添加新数据
                EffectData effectData = new EffectData
                {
                    effectPrefab = effectPrefab,
                    bonePath = bonePath,
                    startFrame = startFrame,
                    endFrame = endFrame,
                    offset = offset,
                    scale = scale,
                    animationClip = animationClip
                };

                effectDataCollection.effects.Add(effectData);
            }

            EditorUtility.SetDirty(effectDataCollection);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            Debug.Log("特效数据已保存为 ScriptableObject 资产。");
        }

        private void ResetTarget()
        {
            modelPrefab = null;
            effectPrefab = null; // 强制刷新EditorWindow
        }

        public void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            effectRT = AssetDatabase.LoadAssetAtPath<RenderTexture>("Assets/ModelToPixel/RenderTexture/EffectRT.renderTexture");

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            effectDataPath = $"{scriptFolder}/EffectDataCollection.asset";
        }

        public void UpdateEffectRTSize(int newWidth, int newHeight)
        {
            if (effectRT != null)
            {
                if (effectRT.width != newWidth || effectRT.height != newHeight)
                {
                    //Debug.Log($"更新 EffectRT 大小: 宽度 {effectRT.width} -> {newWidth}, 高度 {effectRT.height} -> {newHeight}");

                    effectRT.Release(); // 释放当前 RenderTexture
                    effectRT.width = newWidth * 1;  // 更新宽度
                    effectRT.height = newHeight * 1; // 更新高度
                    effectRT.Create(); // 重新创建 RenderTexture
                }
            }
        }

        public void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            if (targetInstance != null)
            {
                DestroyImmediate(targetInstance);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ResetTarget();
            }
        }
        public void SetModelPrefab(GameObject modelPrefab)
        {
            this.modelPrefab = modelPrefab;
            LoadModelPrefab();
        }


        public void SetAnimationClip(string animPath)
        {
            animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);

            if(animator != null)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
                overrideController.GetOverrides(overrides);

                foreach (var item in overrides)
                {
                    if (item.Key != null)
                    {
                        overrideController[item.Key] = animationClip;
                    }
                }
                animator.runtimeAnimatorController = overrideController;
                animator.Play(0, 0, 0);
            }
        }
    }
}