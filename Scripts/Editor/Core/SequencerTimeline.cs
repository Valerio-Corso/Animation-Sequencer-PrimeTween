#if PRIMETWEEN_ENABLED
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    internal static class SequencerTimeline
    {
        public static (float start, float end)[] GetTimings(AnimationStepBase[] steps)
        {
            if (steps == null || steps.Length == 0)
                return null;

            float[] starts = new float[steps.Length];
            float[] ends = new float[steps.Length];

            float cursorEnd = 0f;
            float lastAppendStart = 0f;

            for (int i = 0; i < steps.Length; i++)
            {
                AnimationStepBase step = steps[i];
                if (step == null)
                    continue;

                float duration = GetStepDuration(step);
                float start = (step.FlowType == FlowType.Join ? lastAppendStart : cursorEnd) + step.Delay;
                float end = start + duration;

                starts[i] = start;
                ends[i] = end;

                if (step.FlowType == FlowType.Join)
                {
                    cursorEnd = Mathf.Max(cursorEnd, end);
                }
                else
                {
                    lastAppendStart = start;
                    cursorEnd = end;
                }
            }

            float totalDuration = Mathf.Max(cursorEnd, 0.0001f);
            var timings = new (float start, float end)[steps.Length];
            for (int i = 0; i < steps.Length; i++)
            {
                timings[i] = (starts[i] / totalDuration, ends[i] / totalDuration);
            }

            return timings;
        }

        public static float GetTotalDuration(AnimationStepBase[] steps)
        {
            if (steps == null || steps.Length == 0)
                return 0f;

            float cursorEnd = 0f;
            float lastAppendStart = 0f;

            for (int i = 0; i < steps.Length; i++)
            {
                AnimationStepBase step = steps[i];
                if (step == null)
                    continue;

                float duration = GetStepDuration(step);
                float start = (step.FlowType == FlowType.Join ? lastAppendStart : cursorEnd) + step.Delay;
                float end = start + duration;

                if (step.FlowType == FlowType.Join)
                    cursorEnd = Mathf.Max(cursorEnd, end);
                else
                {
                    lastAppendStart = start;
                    cursorEnd = end;
                }
            }

            return cursorEnd;
        }

        private static float GetStepDuration(AnimationStepBase step)
        {
            switch (step)
            {
                case DOTweenAnimationStep tweenStep:
                    {
                        float baseDuration = Mathf.Max(0f, tweenStep.Duration);
                        int loops = tweenStep.LoopCount;
                        int cycles = loops < 0 ? 10 : loops + 1;
                        return baseDuration * cycles;
                    }
                case WaitForIntervalStep waitStep:
                    return Mathf.Max(0f, waitStep.Interval);
                case PlayParticleSystemAnimationStep particleStep:
                    return Mathf.Max(0f, particleStep.Duration);
                case PlaySequenceAnimationStep playSequenceStep:
                    return playSequenceStep.Sequencer != null
                        ? GetTotalDuration(playSequenceStep.Sequencer.AnimationSteps)
                        : 0f;
                default:
                    return 0f;
            }
        }
    }
}
#endif
