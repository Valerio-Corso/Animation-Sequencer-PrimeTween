#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class SetTargetTransformPropertiesStep : AnimationStepBase
    {
        public override string DisplayName => "Set Target Transform Properties";
        [FormerlySerializedAs("targetGameObject")] [SerializeField]
        private Transform targetTransform;
        
        [SerializeField]
        private bool useLocal;
        [SerializeField]
        private Vector3 position;
        [SerializeField] 
        private Vector3 eulerAngles;
        [SerializeField] 
        private Vector3 scale = Vector3.one;

        private Vector3 originalPosition;
        private Vector3 originalEulerAngles;
        private Vector3 originalScale;
        
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence behaviourSequence = Sequence.Create();
            if (Delay > 0)
                behaviourSequence.ChainDelay(Delay);

            behaviourSequence.ChainCallback(() =>
            {
                if (useLocal)
                {
                    originalPosition = targetTransform.localPosition;
                    originalEulerAngles = targetTransform.localEulerAngles;
                    
                    targetTransform.localPosition = position;
                    targetTransform.localEulerAngles = eulerAngles;
                }
                else
                {
                    originalPosition = targetTransform.position;
                    originalEulerAngles = targetTransform.eulerAngles;
                    
                    targetTransform.position = position;
                    targetTransform.eulerAngles = eulerAngles;
                }

                originalScale = targetTransform.localScale; 
                targetTransform.localScale = scale;
            });
            if (FlowType == FlowType.Join)
                animationSequence.Group(behaviourSequence);
            else
                animationSequence.Chain(behaviourSequence);
        }

        public override void ResetToInitialState()
        {
            if (useLocal)
            {
                targetTransform.localPosition = originalPosition;
                targetTransform.localEulerAngles = originalEulerAngles;
            }
            else
            {
                targetTransform.position = originalPosition;
                targetTransform.eulerAngles = originalEulerAngles;
            }
            targetTransform.localScale = originalScale;
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (targetTransform != null)
                display = targetTransform.name;
            
            return $"{index}. Set {display} Transform Properties";
        }   
    }
}
#endif
