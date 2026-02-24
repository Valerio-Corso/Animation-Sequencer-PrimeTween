#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PunchPositionDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Punch Position";

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

        [SerializeField]
        private bool snapping;
        public bool Snapping
        {
            get => snapping;
            set => snapping = value;
        }

        private Transform previousTarget;
        private Vector3 previousPosition;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            previousPosition = target.transform.localPosition;

            if (snapping)
                Debug.LogWarning($"{DisplayName} doesn't support '{nameof(snapping)}' with PrimeTween.");

            var settings = new ShakeSettings(punch, duration, vibrato, asymmetryFactor: 1f - elasticity);
            return Tween.PunchLocalPosition(previousTarget, settings);
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            previousTarget.localPosition = previousPosition;
        }
    }
}
#endif
