using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ModelToPixel
{
    public class MTPAnimationHandler
    {
        private Animator baseAnimator;
        private Animator effectAnimator;
        private SpriteRenderer previewSprite;
        private SpriteRenderer previewSpriteEffect;
        private string normalPath;
        private string basePath;
        private string effectPath;
        private Dictionary<string, bool> originalLoopStates = new Dictionary<string, bool>();

        public MTPAnimationHandler(SpriteRenderer previewSprite, SpriteRenderer previewSpriteEffect)
        {
            this.previewSprite = previewSprite;
            this.previewSpriteEffect = previewSpriteEffect;
        }

        public void LoadAndPreviewAnimation(string animPath)
        {
            if (previewSprite == null || previewSpriteEffect == null)
            {
                Debug.LogError("Preview Sprites are not assigned.");
                return;
            }

            LoadBaseColorAnimation(animPath);
            LoadEffectAnimation(animPath);
        }

        private void LoadBaseColorAnimation(string animPath)
        {
            AnimationClip baseColorClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            if (baseColorClip == null)
            {
                Debug.LogError("Failed to load base color animation clip.");
                return;
            }

            baseAnimator = previewSprite.GetComponent<Animator>();
            if (baseAnimator == null)
            {
                baseAnimator = previewSprite.gameObject.AddComponent<Animator>();
            }

            AnimatorOverrideController baseAOC = new AnimatorOverrideController(baseAnimator.runtimeAnimatorController);
            var baseOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(baseAOC.overridesCount);
            baseAOC.GetOverrides(baseOverrides);
            foreach (var item in baseOverrides)
            {
                if (item.Key != null)
                {
                    baseAOC[item.Key] = baseColorClip;
                }
            }
            baseAnimator.runtimeAnimatorController = baseAOC;
            baseAnimator.speed = 0;

            normalPath = Path.Combine(Path.GetDirectoryName(animPath), Path.GetFileNameWithoutExtension(animPath) + "_N.png");
            basePath = Path.Combine(Path.GetDirectoryName(animPath), Path.GetFileNameWithoutExtension(animPath) + "_D.png");
            effectPath = Path.Combine(Path.GetDirectoryName(animPath), Path.GetFileNameWithoutExtension(animPath) + "_E.png");
            Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
            if (normalTex != null)
            {
                Material spriteMat = previewSprite.GetComponent<SpriteRenderer>().material;
                if (spriteMat != null)
                {
                    spriteMat.SetTexture("_NormalMap", normalTex);
                }
            }
        }

        private void LoadEffectAnimation(string animPath)
        {
            string effectAnimPath = animPath.Replace(".anim", "_effect.anim");
            AnimationClip effectClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(effectAnimPath);
            if (effectClip == null)
            {
                previewSpriteEffect.enabled = false;
                return;
            }

            previewSpriteEffect.enabled = true;

            effectAnimator = previewSpriteEffect.GetComponent<Animator>();
            if (effectAnimator == null)
            {
                effectAnimator = previewSpriteEffect.gameObject.AddComponent<Animator>();
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
            effectAnimator.speed = 0;
        }

        public void PlayAnimation()
        {
            if (baseAnimator != null)
            {
                baseAnimator.speed = 1;
            }
            if (effectAnimator != null)
            {
                effectAnimator.speed = 1;
            }
        }

        public void PauseAnimation()
        {
            if (baseAnimator != null)
            {
                baseAnimator.speed = 0;
            }
            if (effectAnimator != null)
            {
                effectAnimator.speed = 0;
            }
        }

        public void RestoreOriginalLoopStates()
        {
            foreach (var kvp in originalLoopStates)
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(kvp.Key);
                if (clip != null)
                {
                    var serializedClip = new SerializedObject(clip);
                    SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");
                    SerializedProperty loopTime = settings.FindPropertyRelative("m_LoopTime");
                    if (loopTime.boolValue != kvp.Value)
                        loopTime.boolValue = kvp.Value;
                    serializedClip.ApplyModifiedProperties();
                }
            }
        }
    }
}