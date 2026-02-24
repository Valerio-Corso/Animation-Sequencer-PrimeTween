#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class PlayParticleSystemAnimationStep : AnimationStepBase
    {
        [SerializeField]
        private ParticleSystem particleSystem;
        public ParticleSystem ParticleSystem
        {
            get => particleSystem;
            set => particleSystem = value;
        }

        [SerializeField]
        private float duration = 1;
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        [SerializeField]
        private bool stopEmittingWhenOver;
        public bool StopEmittingWhenOver
        {
            get => stopEmittingWhenOver;
            set => stopEmittingWhenOver = value;
        }

        public override string DisplayName => "Play Particle System";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = Sequence.Create();
            if (Delay > 0)
                sequence.ChainDelay(Delay);

            sequence.ChainCallback(() => particleSystem.Play());
            sequence.ChainDelay(duration);
            sequence.ChainCallback(FinishParticles);

            if (FlowType == FlowType.Join)
                animationSequence.Group(sequence);
            else
                animationSequence.Chain(sequence);
        }

        public override void ResetToInitialState()
        {
        }

        private void FinishParticles()
        {
            if (stopEmittingWhenOver)
            {
                particleSystem.Stop();
            }
        }

        public void SetTarget(ParticleSystem newTarget)
        {
            particleSystem = newTarget;
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string display = "NULL";
            if (particleSystem != null)
                display = particleSystem.name;
            return $"{index}. Play {display} particle system";
        }

    }
}
#endif
