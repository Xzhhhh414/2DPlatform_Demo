using System.Collections.Generic;
using UnityEngine;

namespace ModelToPixel
{
    [CreateAssetMenu(fileName = "AnimationConfig", menuName = "MTP/2DAnimation Config")]
    public class AnimationConfigSO : ScriptableObject
    {
        public string characterName;
        public List<AnimationInfo> animations = new List<AnimationInfo>();
    }

    [System.Serializable]
    public class AnimationInfo
    {
        public string animationName;
        //public string oldName;        // 旧的动画名称（如果有变化）
        public int rows;
        public int columns;
        public int totalFrames;
        public float scaleX;
        public float scaleY;
        public bool hasEffect;
        public List<int> selectedFrameIndices; 
    }
}