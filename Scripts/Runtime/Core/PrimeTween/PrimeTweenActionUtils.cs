#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    internal static class PrimeTweenActionUtils
    {
        public static int ToPrimeTweenCycles(int loops)
        {
            if (loops < 0)
                return -1;

            return loops + 1;
        }

        public static Sequence.SequenceCycleMode ToSequenceCycleMode(LoopType loopType)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    return Sequence.SequenceCycleMode.Yoyo;
                case LoopType.Incremental:
                    Debug.LogWarning("PrimeTween Sequence doesn't support Incremental cycle mode. Falling back to Restart.");
                    return Sequence.SequenceCycleMode.Restart;
                default:
                    return Sequence.SequenceCycleMode.Restart;
            }
        }

        public static CycleMode ToCycleMode(LoopType loopType)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    return CycleMode.Yoyo;
                case LoopType.Incremental:
                    return CycleMode.Incremental;
                default:
                    return CycleMode.Restart;
            }
        }

        public static PrimeTween.UpdateType ToPrimeTweenUpdateType(BrunoMikoski.AnimationSequencer.UpdateType updateType)
        {
            switch (updateType)
            {
                case BrunoMikoski.AnimationSequencer.UpdateType.Late:
                    return PrimeTween.UpdateType.LateUpdate;
                case BrunoMikoski.AnimationSequencer.UpdateType.Fixed:
                    return PrimeTween.UpdateType.FixedUpdate;
                case BrunoMikoski.AnimationSequencer.UpdateType.Manual:
                    Debug.LogWarning("PrimeTween Manual update type isn't enabled. Falling back to Update.");
                    return PrimeTween.UpdateType.Update;
                default:
                    return PrimeTween.UpdateType.Update;
            }
        }

        public static (float start, float end) ResolveFloat(float current, float value, bool isRelative, DOTweenActionBase.AnimationDirection direction)
        {
            float end = isRelative ? current + value : value;
            return direction == DOTweenActionBase.AnimationDirection.From
                ? (end, current)
                : (current, end);
        }

        public static (Vector2 start, Vector2 end) ResolveVector2(Vector2 current, Vector2 value, bool isRelative, DOTweenActionBase.AnimationDirection direction)
        {
            Vector2 end = isRelative ? current + value : value;
            return direction == DOTweenActionBase.AnimationDirection.From
                ? (end, current)
                : (current, end);
        }

        public static (Vector3 start, Vector3 end) ResolveVector3(Vector3 current, Vector3 value, bool isRelative, DOTweenActionBase.AnimationDirection direction)
        {
            Vector3 end = isRelative ? current + value : value;
            return direction == DOTweenActionBase.AnimationDirection.From
                ? (end, current)
                : (current, end);
        }

        public static (Color start, Color end) ResolveColor(Color current, Color value, bool isRelative, DOTweenActionBase.AnimationDirection direction)
        {
            Color end = isRelative ? current + value : value;
            end.r = Mathf.Clamp01(end.r);
            end.g = Mathf.Clamp01(end.g);
            end.b = Mathf.Clamp01(end.b);
            end.a = Mathf.Clamp01(end.a);
            return direction == DOTweenActionBase.AnimationDirection.From
                ? (end, current)
                : (current, end);
        }

        public static Vector3 ApplyAxisConstraint(Vector3 current, Vector3 value, AxisConstraint axisConstraint, bool isRelative)
        {
            if (axisConstraint == AxisConstraint.None)
                return isRelative ? current + value : value;

            Vector3 end = current;
            if ((axisConstraint & AxisConstraint.X) != 0)
                end.x = isRelative ? current.x + value.x : value.x;
            if ((axisConstraint & AxisConstraint.Y) != 0)
                end.y = isRelative ? current.y + value.y : value.y;
            if ((axisConstraint & AxisConstraint.Z) != 0)
                end.z = isRelative ? current.z + value.z : value.z;

            return end;
        }

        public static Vector2 ApplyAxisConstraint(Vector2 current, Vector2 value, AxisConstraint axisConstraint, bool isRelative)
        {
            if (axisConstraint == AxisConstraint.None)
                return isRelative ? current + value : value;

            Vector2 end = current;
            if ((axisConstraint & AxisConstraint.X) != 0)
                end.x = isRelative ? current.x + value.x : value.x;
            if ((axisConstraint & AxisConstraint.Y) != 0)
                end.y = isRelative ? current.y + value.y : value.y;

            return end;
        }

    }
}
#endif
