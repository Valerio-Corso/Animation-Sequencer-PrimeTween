#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FillImageDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Image);
        public override string DisplayName => "Fill Amount";

        [SerializeField, Range(0, 1)]
        private float fillAmount;
        public float FillAmount
        {
            get => fillAmount;
            set => fillAmount = Mathf.Clamp01(value);
        }

        private Image image;
        private float previousFillAmount;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (image == null)
            {
                image = target.GetComponent<Image>();
                if (image == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return default;
                }
            }

            previousFillAmount = image.fillAmount;
            var (start, end) = PrimeTweenActionUtils.ResolveFloat(previousFillAmount, fillAmount, IsRelative, Direction);
            return Tween.UIFillAmount(image, start, end, duration, Ease.ToEasing());
        }

        public override void ResetToInitialState()
        {
            if (image == null)
                return;

            image.fillAmount = previousFillAmount;
        }
    }
}
#endif
