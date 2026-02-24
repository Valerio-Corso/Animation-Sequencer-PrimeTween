#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class MoveDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        private bool localMove;
        public bool LocalMove
        {
            get => localMove;
            set => localMove = value;
        }

        [SerializeField]
        private AxisConstraint axisConstraint;
        public AxisConstraint AxisConstraint
        {
            get => axisConstraint;
            set => axisConstraint = value;
        }

        private Vector3 previousPosition;
        private GameObject previousTarget;

        public override string DisplayName => "Move to Position";

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target;
            if (localMove)
            {
                previousPosition = target.transform.localPosition;
                Vector3 endValue = PrimeTweenActionUtils.ApplyAxisConstraint(previousPosition, GetPosition(), axisConstraint, IsRelative);
                var (start, end) = PrimeTweenActionUtils.ResolveVector3(previousPosition, endValue, false, Direction);
                return Tween.LocalPosition(target.transform, start, end, duration, Ease.ToEasing());
            }
            
            previousPosition = target.transform.position;
            Vector3 worldEndValue = PrimeTweenActionUtils.ApplyAxisConstraint(previousPosition, GetPosition(), axisConstraint, IsRelative);
            var (worldStart, worldEnd) = PrimeTweenActionUtils.ResolveVector3(previousPosition, worldEndValue, false, Direction);
            return Tween.Position(target.transform, worldStart, worldEnd, duration, Ease.ToEasing());
        }

        protected abstract Vector3 GetPosition();

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (localMove)
                previousTarget.transform.localPosition = previousPosition;
            else
                previousTarget.transform.position = previousPosition;
        }
    }
}
#endif
