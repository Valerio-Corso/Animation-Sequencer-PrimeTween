#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class RectTransformSizeDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(RectTransform);
        public override string DisplayName => "RectTransform Size";

        [SerializeField]
        private Vector2 sizeDelta;
        public Vector2 SizeDelta
        {
            get => sizeDelta;
            set => sizeDelta = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }


        private RectTransform previousTarget;
        private Vector2 previousSize;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform as RectTransform;
            previousSize = previousTarget.sizeDelta;
            Vector2 endValue = PrimeTweenActionUtils.ApplyAxisConstraint(previousSize, sizeDelta, axisConstraint, IsRelative);
            var (start, end) = PrimeTweenActionUtils.ResolveVector2(previousSize, endValue, false, Direction);
            return Tween.UISizeDelta(previousTarget, start, end, duration, Ease.ToEasing());
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;

            previousTarget.sizeDelta = previousSize;
        }
    }
}
#endif
