#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class AnchoredPositionMoveDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(RectTransform);

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private RectTransform rectTransform;
        private Vector2 previousAnchorPosition;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (rectTransform == null)
            {
                rectTransform = target.transform as RectTransform;

                if (rectTransform == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return default;
                }
            }

            previousAnchorPosition = rectTransform.anchoredPosition;
            Vector2 endValue = PrimeTweenActionUtils.ApplyAxisConstraint(previousAnchorPosition, GetPosition(), axisConstraint, IsRelative);
            var (start, end) = PrimeTweenActionUtils.ResolveVector2(previousAnchorPosition, endValue, false, Direction);
            return Tween.UIAnchoredPosition(rectTransform, start, end, duration, Ease.ToEasing());
        }

        protected abstract Vector2 GetPosition();

        public override void ResetToInitialState()
        {
            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition = previousAnchorPosition;
        }
    }
}
#endif
