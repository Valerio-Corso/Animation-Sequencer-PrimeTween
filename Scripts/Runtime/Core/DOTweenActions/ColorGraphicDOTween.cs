#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class ColorGraphicDOTween : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Color Graphic";

        [SerializeField]
        private Color color;

        private Graphic targetGraphic;
        private Color previousColor;

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

            previousColor = targetGraphic.color;
            var (start, end) = PrimeTweenActionUtils.ResolveColor(previousColor, color, IsRelative, Direction);
            Tween graphicTween = Tween.Color(targetGraphic, start, end, duration, Ease.ToEasing());

#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                // Work around a Unity bug where updating the colour does not cause any visual change outside of PlayMode.
                // https://forum.unity.com/threads/editor-scripting-force-color-update.798663/
                graphicTween.OnUpdate(targetGraphic, (graphic, _) =>
                {
                    graphic.transform.localScale = new Vector3(1.001f, 1.001f, 1.001f);
                    graphic.transform.localScale = new Vector3(1, 1, 1);
                });
            }
#endif
            
            return graphicTween;
        }
        
        public override void ResetToInitialState()
        {
            if (targetGraphic == null)
                return;

            targetGraphic.color = previousColor;
        }
    }
}

#endif
