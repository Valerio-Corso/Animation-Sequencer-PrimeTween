#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class RotateDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        public override string DisplayName => "Punch Scale";

        [SerializeField]
        private bool local;
        public bool Local
        {
            get => local;
            set => local = value;
        }

        [SerializeField]
        private RotateMode rotationMode = RotateMode.Fast;
        public RotateMode RotationMode
        {
            get => rotationMode;
            set => rotationMode = value;
        }


        private Transform previousTarget;
        private Quaternion previousRotation;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            if (local)
                previousRotation = target.transform.localRotation;
            else
                previousRotation = target.transform.rotation;

            Vector3 rotationValue = GetRotation();

            Vector3 currentEuler = local ? previousTarget.localEulerAngles : previousTarget.eulerAngles;
            Vector3 endValue = currentEuler + rotationValue;
            if (rotationMode == RotateMode.LocalAxisAdd || rotationMode == RotateMode.WorldAxisAdd)
            {
                var (startAdd, endAdd) = PrimeTweenActionUtils.ResolveVector3(currentEuler, endValue, false, Direction);
                return local
                    ? Tween.LocalRotation(previousTarget, startAdd, endAdd, duration, Ease.ToEasing())
                    : Tween.Rotation(previousTarget, startAdd, endAdd, duration, Ease.ToEasing());
            }

            endValue = IsRelative ? currentEuler + rotationValue : rotationValue;
            var (start, end) = PrimeTweenActionUtils.ResolveVector3(currentEuler, endValue, false, Direction);

            return local
                ? Tween.LocalRotation(previousTarget, start, end, duration, Ease.ToEasing())
                : Tween.Rotation(previousTarget, start, end, duration, Ease.ToEasing());
        }

        
        protected abstract Vector3 GetRotation();

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (!local)
                previousTarget.rotation = previousRotation;
            else
                previousTarget.localRotation = previousRotation;
        }
    }
}
#endif
