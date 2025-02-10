using UnityEngine;


namespace ModelToPixel
{
    public class MTPBulletEffect : MonoBehaviour
    {
        public static void LoadEffectPrefab(ref GameObject  effectInstance, GameObject effectPivot, GameObject effectPrefab)
        {
            if (effectInstance != null)
            {
                DestroyImmediate(effectInstance);
            }

            effectPivot = GameObject.Find("BulletPivotEffect");

            if (effectPivot != null && effectPrefab != null)
            {
                effectInstance = Instantiate(effectPrefab, effectPivot.transform, false);
                effectInstance.name = effectPrefab.name;
                ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Simulate(0.1f , true, true);
                    ps.Pause();
                }
            }
        }
        public static float GetTotalFrame(GameObject effectInstance)
        {
            Animator effectAnimator = effectInstance.GetComponent<Animator>();

            if(effectAnimator != null)
            {
                AnimationClip effectClip = effectAnimator.runtimeAnimatorController.animationClips[0];
                effectAnimator.Play(0, 0);
                effectAnimator.speed = 0;
                return effectClip.length;
            }

            float maxLifetime = 0f;
            if(effectInstance !=null)
            {
                ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                

                foreach (ParticleSystem ps in particleSystems)
                {
                    var startLifetime = ps.main.startLifetime;

                    if (startLifetime.mode == ParticleSystemCurveMode.Constant)
                    {
                        if (startLifetime.constant > maxLifetime)
                        {
                            maxLifetime = startLifetime.constant;
                        }
                    }
                    else if (startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                    {
                        if (startLifetime.constantMax > maxLifetime)
                        {
                            maxLifetime = startLifetime.constantMax;
                        }
                    }
                    else if (startLifetime.mode == ParticleSystemCurveMode.Curve || startLifetime.mode == ParticleSystemCurveMode.TwoCurves)
                    {
                        float evaluatedLifetime = startLifetime.Evaluate(1f); // Evaluate the curve at the highest point (time=1)
                        if (evaluatedLifetime > maxLifetime)
                        {
                            maxLifetime = evaluatedLifetime;
                        }
                    }
                }
            }


            return maxLifetime;
        }
    }
}
