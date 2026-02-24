#if PRIMETWEEN_ENABLED
using System;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public partial class CustomEase : IEquatable<CustomEase>
    {
        [SerializeField]
        private Ease ease;
        public Ease Ease => ease;
        [SerializeField]
        private AnimationCurve curve;

        public bool UseCustomCurve => ease == Ease.Custom;

        public CustomEase(AnimationCurve curve)
        {
            this.curve = curve;
            ease = Ease.Custom;
        }

        public CustomEase(Ease ease)
        {
            this.ease = ease;
            curve = null;
        }

        public CustomEase()
        {
            ease = Ease.InOutCirc;
        }
        
        public float Lerp(float from, float to, float fraction)
        {
            return Mathf.Lerp(from, to, Evaluate(fraction));
        }

        public float LerpUnclamped(float from, float to, float fraction)
        {
            return Mathf.LerpUnclamped(from, to, Evaluate(fraction));
        }

        [Pure]
        public float Evaluate(float time, float duration = 1f,
            float overshootOrAmplitude = 1.70158f)
        {
            if (UseCustomCurve)
                return curve == null ? time : curve.Evaluate(Mathf.Clamp01(time / Mathf.Max(duration, 0.0001f)));

            float t = Mathf.Clamp01(time / Mathf.Max(duration, 0.0001f));
            return Easing.Evaluate(t, ToPrimeTweenEase(ease));
        }

        public Easing ToEasing()
        {
            if (UseCustomCurve)
                return Easing.Curve(curve);

            return Easing.Standard(ToPrimeTweenEase(ease));
        }

        private static PrimeTween.Ease ToPrimeTweenEase(Ease ease)
        {
            return (PrimeTween.Ease) (int) ease;
        }

        public bool Equals(CustomEase other)
        {
            return ease == other.ease && (ease != Ease.Custom || Equals(curve, other.curve));
        }

        public override bool Equals(object obj)
        {
            return obj is CustomEase other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)ease * 397) ^ ((ease == Ease.Custom && curve != null) ? curve.GetHashCode() : 0);
            }
        }
    }
}
#endif
