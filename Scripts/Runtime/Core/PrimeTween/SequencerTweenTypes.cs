#if PRIMETWEEN_ENABLED
using System;

namespace BrunoMikoski.AnimationSequencer
{
    public enum Ease : sbyte
    {
        Custom = -1,
        Default = 0,
        Linear = 1,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce
    }

    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }

    public enum UpdateType
    {
        Normal,
        Late,
        Fixed,
        Manual
    }

    [Flags]
    public enum AxisConstraint
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    public enum RotateMode
    {
        Fast,
        FastBeyond360,
        WorldAxisAdd,
        LocalAxisAdd
    }

    public enum PathType
    {
        Linear,
        CatmullRom
    }

    public enum PathMode
    {
        Full3D,
        TopDown2D,
        SideScroller2D
    }

    public enum ScrambleMode
    {
        None,
        All,
        Uppercase,
        Lowercase,
        Numerals
    }
}
#endif
