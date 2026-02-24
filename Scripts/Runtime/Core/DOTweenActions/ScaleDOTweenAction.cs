#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ScaleDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Scale to Size";

        [SerializeField]
        private Vector3 scale;
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private Vector3? previousState;
        private GameObject previousTarget;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousState = target.transform.localScale;
            previousTarget = target;
            
            Vector3 endValue = PrimeTweenActionUtils.ApplyAxisConstraint(previousState.Value, scale, axisConstraint, IsRelative);
            var (start, end) = PrimeTweenActionUtils.ResolveVector3(previousState.Value, endValue, false, Direction);
            return Tween.Scale(target.transform, start, end, duration, Ease.ToEasing());
        }

        public override void ResetToInitialState()
        {
            if (!previousState.HasValue)
                return;

            previousTarget.transform.localScale = previousState.Value;
        }
    }
}
#endif
