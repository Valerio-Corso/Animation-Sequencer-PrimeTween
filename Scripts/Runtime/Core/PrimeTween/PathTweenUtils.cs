#if PRIMETWEEN_ENABLED
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    internal static class PathTweenUtils
    {
        public static Vector3 EvaluatePath(Vector3[] points, PathType pathType, float t)
        {
            if (points == null || points.Length == 0)
                return Vector3.zero;
            if (points.Length == 1)
                return points[0];

            t = Mathf.Clamp01(t);

            switch (pathType)
            {
                case PathType.CatmullRom:
                    return EvaluateCatmullRom(points, t);
                case PathType.Linear:
                default:
                    return EvaluateLinear(points, t);
            }
        }

        private static Vector3 EvaluateLinear(Vector3[] points, float t)
        {
            if (points.Length == 2)
                return Vector3.LerpUnclamped(points[0], points[1], t);

            float totalLength = 0f;
            float[] segmentLengths = new float[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                float len = Vector3.Distance(points[i], points[i + 1]);
                segmentLengths[i] = len;
                totalLength += len;
            }

            if (totalLength <= Mathf.Epsilon)
                return points[0];

            float targetDistance = t * totalLength;
            float accumulated = 0f;
            for (int i = 0; i < segmentLengths.Length; i++)
            {
                float segLen = segmentLengths[i];
                if (accumulated + segLen >= targetDistance)
                {
                    float localT = (targetDistance - accumulated) / Mathf.Max(segLen, Mathf.Epsilon);
                    return Vector3.LerpUnclamped(points[i], points[i + 1], localT);
                }
                accumulated += segLen;
            }

            return points[points.Length - 1];
        }

        private static Vector3 EvaluateCatmullRom(Vector3[] points, float t)
        {
            int segmentCount = points.Length - 1;
            float scaledT = t * segmentCount;
            int seg = Mathf.Clamp(Mathf.FloorToInt(scaledT), 0, segmentCount - 1);
            float localT = scaledT - seg;

            Vector3 p0 = points[Mathf.Max(seg - 1, 0)];
            Vector3 p1 = points[seg];
            Vector3 p2 = points[seg + 1];
            Vector3 p3 = points[Mathf.Min(seg + 2, points.Length - 1)];

            float tt = localT * localT;
            float ttt = tt * localT;

            return 0.5f * ((2f * p1) +
                           (-p0 + p2) * localT +
                           (2f * p0 - 5f * p1 + 4f * p2 - p3) * tt +
                           (-p0 + 3f * p1 - 3f * p2 + p3) * ttt);
        }
    }
}
#endif
