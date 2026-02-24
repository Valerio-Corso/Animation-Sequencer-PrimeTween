#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetCanvasGroupPropertiesStep : AnimationStepBase
    {
        [SerializeField]
        private CanvasGroup targetCanvasGroup;

        [SerializeField] 
        private float targetAlpha = 1f;

        private float originalAlpha;
        
        public override string DisplayName => "Set Target Canvas Group Properties";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = Sequence.Create();
            if (Delay > 0)
                behaviourSequence.ChainDelay(Delay);

            behaviourSequence.ChainCallback(() =>
            {
                originalAlpha = targetCanvasGroup.alpha; 
                targetCanvasGroup.alpha = targetAlpha;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Group(behaviourSequence);
            else
                animationSequence.Chain(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            targetCanvasGroup.alpha = originalAlpha;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetCanvasGroup != null)
                display = targetCanvasGroup.name;
            
            return $"{index}. Set {display} Properties";
        } 
    }
}
#endif
