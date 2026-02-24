#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class WaitForIntervalStep : AnimationStepBase
    {
        public override string DisplayName => "Wait for Interval";

        [SerializeField]
        private float interval;
        public float Interval
        {
            get => interval;
            set => interval = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = Sequence.Create();
            if (Delay > 0)
                sequence.ChainDelay(Delay);

            sequence.ChainDelay(interval);

            if (FlowType == FlowType.Join)
                animationSequence.Group(sequence);
            else
                animationSequence.Chain(sequence);
        }

        public override void ResetToInitialState()
        {
        }

        public override string GetDisplayNameForEditor(int index)
        {
            return $"{index}. Wait {interval} seconds";
        }
    }
}
#endif
