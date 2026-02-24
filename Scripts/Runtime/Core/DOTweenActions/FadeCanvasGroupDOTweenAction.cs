#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeCanvasGroupDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(CanvasGroup);

        public override string DisplayName => "Fade Canvas Group";

        [SerializeField]
        private float alpha;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        private CanvasGroup canvasGroup;
        private float previousFade;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (canvasGroup == null)
            {
                canvasGroup = target.GetComponent<CanvasGroup>();

                if (canvasGroup == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return default;
                }
            }

            previousFade = canvasGroup.alpha;
            var (start, end) = PrimeTweenActionUtils.ResolveFloat(previousFade, alpha, IsRelative, Direction);
            return Tween.Alpha(canvasGroup, start, end, duration, Ease.ToEasing());
        }

        public override void ResetToInitialState()
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = previousFade;
        }
    }
}
#endif
