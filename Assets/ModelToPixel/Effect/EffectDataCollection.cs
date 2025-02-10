using System.Collections.Generic;
using UnityEngine;

namespace ModelToPixel
{
    [CreateAssetMenu(fileName = "EffectDataCollection", menuName = "ScriptableObjects/EffectDataCollection", order = 1)]
    public class EffectDataCollection : ScriptableObject
    {
        public List<EffectData> effects = new List<EffectData>();
    }

    [System.Serializable]
    public class EffectData
    {
        
        public AnimationClip animationClip;
        public GameObject effectPrefab;
        public string bonePath;
        public int startFrame;
        public int endFrame;
        public Vector3 offset;
        public Vector3 scale;
    }
}