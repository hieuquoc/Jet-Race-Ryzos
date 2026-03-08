using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class EffectController : MonoBehaviour
    {
        public bool IsActive;
        public Collider ShipCollider;
        public ParticleSystem ShieldEffect;
        public List<EffectData> ActiveEffects = new List<EffectData>();
        public float SpeedBoostMultiplier = 1.5f;
        public float SlowMultiplier = 0.5f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!IsActive)
            {
                return;
            }
            if (ActiveEffects.Count > 0)
            {
                for (int i = ActiveEffects.Count - 1; i >= 0; i--)
                {
                    EffectData effect = ActiveEffects[i];
                    if (Time.time - effect.StartTime >= effect.Duration)
                    {
                        ActiveEffects.RemoveAt(i);
                        switch (effect.Type)
                        {
                            case EffectType.SpeedBoost:
                                MapController.Instance.SetSpeedMultiplier(1f);
                                break;
                            case EffectType.Shield:
                                ActiveShield(false);
                                break;
                            case EffectType.Slow:
                                MapController.Instance.SetSpeedMultiplier(1f);
                                break;
                        }
                    }
                }
            }
            else
            {
                IsActive = false;
            }
        }

        public void AddEffect(EffectType type, float duration)
        {
            EffectData existingEffect = ActiveEffects.Find(e => e.Type == type);
            if (existingEffect.Type != 0)
            {
                existingEffect.Duration = duration;
                existingEffect.StartTime = Time.time;
            }
            else
            {
                ActiveEffects.Add(new EffectData { Type = type, Duration = duration, StartTime = Time.time });
                switch (type)
                {
                    case EffectType.SpeedBoost:
                        MapController.Instance.SetSpeedMultiplier(SpeedBoostMultiplier);
                        break;
                    case EffectType.Shield:
                        ActiveShield(true);
                        break;
                    case EffectType.Slow:
                        MapController.Instance.SetSpeedMultiplier(SlowMultiplier);
                        // Handle slow logic
                        break;
                }
            }
        }

        public void ActiveShield(bool active)
        {
            ShipCollider.enabled = !active;
            ShieldEffect.gameObject.SetActive(active);
            if (ShieldEffect != null)
            {
                if (active)
                {
                    ShieldEffect.Play();
                }
                else
                {
                    ShieldEffect.Stop();
                }
            }
        }
    }

    public struct EffectData
    {
        public EffectType Type;
        public float Duration;
        public float StartTime;
    }

    public enum EffectType
    {
        SpeedBoost,
        Shield,
        Slow
    }
}

