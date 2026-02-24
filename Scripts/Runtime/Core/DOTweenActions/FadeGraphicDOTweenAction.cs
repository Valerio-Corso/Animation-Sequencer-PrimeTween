#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeGraphicDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Fade Graphic";

        [SerializeField]
        private float alpha;
        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        private Graphic targetGraphic;
        private float previousAlpha;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetGraphic == null)
            {
                targetGraphic = target.GetComponent<Graphic>();
                if (targetGraphic == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return default;
                }
            }

            previousAlpha = targetGraphic.color.a;
            var (start, end) = PrimeTweenActionUtils.ResolveFloat(previousAlpha, alpha, IsRelative, Direction);
            Tween graphicTween = Tween.Alpha(targetGraphic, start, end, duration, Ease.ToEasing());
            
#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                graphicTween.OnUpdate(targetGraphic, (graphic, _) =>
                {
                    graphic.enabled = false;
                    graphic.enabled = true;
                });
            }
#endif
                
            return graphicTween;
        }

        public override void ResetToInitialState()
        {
            if (targetGraphic == null)
                return;

            Color color = targetGraphic.color;
            color.a = previousAlpha;
            targetGraphic.color = color;
        }
    }
}
#endif
