#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class DOTweenActionBase
    {
        public enum AnimationDirection
        {
            To, 
            From
        }
        
        
        [SerializeField]
        protected AnimationDirection direction;
        public AnimationDirection Direction
        {
            get => direction;
            set => direction = value;
        }

        [SerializeField]
        protected CustomEase ease = CustomEase.InOutCirc;
        public CustomEase Ease
        {
            get => ease;
            set => ease = value;
        }

        [SerializeField]
        protected bool isRelative;
        public bool IsRelative
        {
            get => isRelative;
            set => isRelative = value;
        }

        public virtual Type TargetComponentType { get; }
        public abstract string DisplayName { get; }

        protected abstract Tween GenerateTween_Internal(GameObject target, float duration);

        public Tween GenerateTween(GameObject target, float duration)
        {
            return GenerateTween_Internal(target, duration);
        }

        public abstract void ResetToInitialState();
    }
}
#endif
