#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchScaleDOTweenAction : DOTweenActionBase
    {
        public override string DisplayName => "Punch Scale";
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private Vector3 punch;
        public Vector3 Punch
        {
            get => punch;
            set => punch = value;
        }

        [SerializeField]
        private int vibrato = 10;
        public int Vibrato
        {
            get => vibrato;
            set => vibrato = value;
        }

        [SerializeField]
        private float elasticity = 1f;
        public float Elasticity
        {
            get => elasticity;
            set => elasticity = value;
        }

        private Transform previousTarget;
        private Vector3 previousScale;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousScale = previousTarget.localScale;
            
            var settings = new ShakeSettings(punch, duration, vibrato, asymmetryFactor: 1f - elasticity);
            return Tween.PunchScale(previousTarget, settings);
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            previousTarget.localScale = previousScale;
        }
    }
}
#endif
