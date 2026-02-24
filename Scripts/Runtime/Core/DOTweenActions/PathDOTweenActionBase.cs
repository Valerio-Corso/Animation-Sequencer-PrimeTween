#if PRIMETWEEN_ENABLED
using System;
using PrimeTween;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class PathDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        protected bool isLocal;
        public bool IsLocal
        {
            get => isLocal;
            set => isLocal = value;
        }

        [SerializeField]
        private Color gizmoColor;
        public Color GizmoColor
        {
            get => gizmoColor;
            set => gizmoColor = value;
        }

        [SerializeField]
        private int resolution = 10;
        public int Resolution
        {
            get => resolution;
            set => resolution = value;
        }

        [SerializeField]
        private PathMode pathMode = PathMode.Full3D;
        public PathMode PathMode
        {
            get => pathMode;
            set => pathMode = value;
        }

        [SerializeField]
        private PathType pathType = PathType.CatmullRom;
        public PathType PathType
        {
            get => pathType;
            set => pathType = value;
        }

        private Transform previousTarget;
        private Vector3 previousPosition;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            previousTarget = target.transform;
            Vector3[] path = GetPathPositions();
            if (path == null || path.Length == 0)
            {
                Debug.LogWarning($"{DisplayName} has no path points.");
                return Tween.Delay(duration);
            }

            previousPosition = isLocal ? target.transform.localPosition : target.transform.position;

            if (IsRelative)
            {
                for (int i = 0; i < path.Length; i++)
                    path[i] += previousPosition;
            }

            if (pathMode != PathMode.Full3D)
                Debug.LogWarning($"{DisplayName} ignores PathMode '{pathMode}' in PrimeTween migration.");

            float startT = Direction == AnimationDirection.From ? 1f : 0f;
            float endT = Direction == AnimationDirection.From ? 0f : 1f;

            return Tween.Custom(startT, endT, duration, t =>
            {
                Vector3 pos = PathTweenUtils.EvaluatePath(path, pathType, t);
                if (isLocal)
                    target.transform.localPosition = pos;
                else
                    target.transform.position = pos;
            }, Ease.ToEasing());
        }


        protected abstract Vector3[] GetPathPositions();
        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (isLocal)
            {
                previousTarget.transform.localPosition = previousPosition;
            }
            else
            {
                previousTarget.transform.position = previousPosition;
            }
        }
        
    }
}
#endif
