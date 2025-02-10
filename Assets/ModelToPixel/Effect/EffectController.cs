using UnityEngine;
using System.Collections.Generic;

public class EffectController : MonoBehaviour
{
    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();

    public void RegisterEffect(string name, GameObject effect)
    {
        if (!effects.ContainsKey(name))
        {
            effects.Add(name, effect);
        }
    }

    public void UnregisterEffect(string effectName)
    {
        if (effects.ContainsKey(effectName))
        {
            effects.Remove(effectName);
        }
    }

    public void ActivateEffect(string effectName)
    {
        if (effects.TryGetValue(effectName, out GameObject effect))
        {
            effect.SetActive(true);
        }
    }

    public void DeactivateEffect(string effectName)
    {
        if (effects.TryGetValue(effectName, out GameObject effect))
        {
            effect.SetActive(false);
        }
    }
}