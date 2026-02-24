#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlaySequenceAnimationStep : AnimationStepBase
    {
        public override string DisplayName => "Play Sequence";

        [SerializeField]
        private AnimationSequencerController sequencer;
        public AnimationSequencerController Sequencer
        {
            get => sequencer;
            set => sequencer = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = sequencer.GenerateSequence();
            if (Delay > 0)
                sequence.ChainDelay(Delay);
            if (FlowType == FlowType.Join)
                animationSequence.Group(sequence);
            else
                animationSequence.Chain(sequence);
        }

        public override void ResetToInitialState()
        {
            sequencer.ResetToInitialState();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (sequencer != null)
                display = sequencer.name;
            return $"{index}. Play {display} Sequence";
        }

        public void SetTarget(AnimationSequencerController newTarget)
        {
            sequencer = newTarget;
        }
    }
}
#endif
