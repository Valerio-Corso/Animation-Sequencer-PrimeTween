#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ShakeRotationDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Shake Rotation";

        [SerializeField]
        private Vector3 strength;
        public Vector3 Strength
        {
            get => strength;
            set => strength = value;
        }

        [SerializeField]
        private int vibrato = 10;
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }

        [SerializeField]
        private float randomness = 90;
        public float Randomness
        {
            get => randomness;
            set => randomness = value;
        }

        [SerializeField]
        private bool fadeout = true;
        public bool Fadeout
        {
            get => fadeout;
            set => fadeout = value;
        }


        private Transform previousTarget;
        private Quaternion previousRotation;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousRotation = previousTarget.localRotation;
            
            if (Math.Abs(randomness - 90f) > 0.001f)
                Debug.LogWarning($"{DisplayName} doesn't support '{nameof(randomness)}' with PrimeTween.");

            var settings = new ShakeSettings(strength, duration, vibrato);
            if (fadeout)
            {
                settings.enableFalloff = true;
                settings.frequency *= 1.35f;
            }

            return Tween.ShakeLocalRotation(previousTarget, settings);
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            previousTarget.localRotation = previousRotation;
        }
    }
}
#endif
