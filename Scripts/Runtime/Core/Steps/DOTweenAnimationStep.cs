#if PRIMETWEEN_ENABLED
using System;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField]
        private int loopCount;
        public int LoopCount
        {
            get => loopCount;
            set => loopCount = value;
        }

        [SerializeField]
        private LoopType loopType;
        public LoopType LoopType
        {
            get => loopType;
            set => loopType = value;
        }

        [SerializeReference]
        private DOTweenActionBase[] actions;
        public DOTweenActionBase[] Actions
        {
            get => actions;
            set => actions = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            int cycles = PrimeTweenActionUtils.ToPrimeTweenCycles(loopCount);
            Sequence sequence = Sequence.Create(cycles: cycles,
                cycleMode: PrimeTweenActionUtils.ToSequenceCycleMode(loopType),
                sequenceEase: PrimeTween.Ease.Linear);

            if (Delay > 0)
                sequence.ChainDelay(Delay);

            bool hasTween = false;
            for (int i = 0; i < actions.Length; i++)
            {
                Tween tween = actions[i].GenerateTween(target, duration);
                if (!tween.isAlive)
                    continue;

                if (!hasTween)
                {
                    sequence.Chain(tween);
                    hasTween = true;
                }
                else
                {
                    sequence.Group(tween);
                }
            }

            if (FlowType == FlowType.Join)
                animationSequence.Group(sequence);
            else
                animationSequence.Chain(sequence);
        }

        public override void ResetToInitialState()
        {
            for (int i = actions.Length - 1; i >= 0; i--)
            {
                actions[i].ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;
            
            return $"{index}. {targetName}: {String.Join(", ", actions.Select(action => action.DisplayName)).Truncate(45)}";
        }

        public bool TryGetActionAtIndex<T>(int index, out T result) where T: DOTweenActionBase
        {
            if (index < 0 || index > actions.Length - 1)
            {
                result = null;
                return false;
            }

            result = actions[index] as T;
            return result != null;
        }
    }
}
#endif
