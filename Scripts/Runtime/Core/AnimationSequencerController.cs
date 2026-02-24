#if PRIMETWEEN_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;
#endif
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Animation Sequencer Controller", 200)]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum PlayType
        {
            Forward,
            Backward
        }

        public enum AutoplayType
        {
            Awake,
            OnEnable,
            Nothing
        }

        [SerializeReference] 
        private AnimationStepBase[] animationSteps = Array.Empty<AnimationStepBase>();
        public AnimationStepBase[] AnimationSteps => animationSteps;

        [SerializeField] 
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField] 
        private bool timeScaleIndependent = false;
        [SerializeField] 
        private AutoplayType autoplayMode = AutoplayType.Awake;
        [SerializeField] 
        protected bool startPaused;
        [SerializeField] 
        private float playbackSpeed = 1f;
        public float PlaybackSpeed => playbackSpeed;
        [SerializeField] 
        protected PlayType playType = PlayType.Forward;
        [SerializeField] 
        private int loops = 0;
        [SerializeField] 
        private LoopType loopType = LoopType.Restart;
        [SerializeField] 
        private bool autoKill = true;

        [SerializeField] 
        private UnityEvent onStartEvent = new UnityEvent();

        public UnityEvent OnStartEvent
        {
            get => onStartEvent;
            protected set => onStartEvent = value;
        }

        [SerializeField] 
        private UnityEvent onFinishedEvent = new UnityEvent();

        public UnityEvent OnFinishedEvent
        {
            get => onFinishedEvent;
            protected set => onFinishedEvent = value;
        }

        [SerializeField] 
        private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;
        private PlayType playTypeInternal = PlayType.Forward;
#if UNITY_EDITOR
        private bool requiresReset = false;
#endif

        public bool IsPlaying => playingSequence.isAlive && !playingSequence.isPaused;
        public bool IsPaused => playingSequence.isAlive && playingSequence.isPaused;

        [SerializeField, Range(0, 1)] 
        private float progress = -1;

        protected virtual void Awake()
        {
            progress = -1;
            if (autoplayMode != AutoplayType.Awake)
                return;

            Autoplay();
        }

        protected virtual void OnEnable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            Autoplay();
        }

        private void Autoplay()
        {
            Play();
            if (startPaused)
                playingSequence.isPaused = true;
        }

        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            if (!playingSequence.isAlive)
                return;

            ClearPlayingSequence();
            // Reset the object to its initial state so that if it is re-enabled the start values are correct for
            // regenerating the Sequence.
            ResetToInitialState();
        }

        protected virtual void OnDestroy()
        {
            ClearPlayingSequence();
        }

        public virtual void Play()
        {
            Play(null);
        }

        public virtual void Play(Action onCompleteCallback)
        {
            playTypeInternal = playType;

            ClearPlayingSequence();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            playingSequence = GenerateSequence();
            ApplyPlaybackDirection(playTypeInternal);
        }

        public virtual void PlayForward(bool resetFirst = true, Action onCompleteCallback = null)
        {
            if (!playingSequence.isAlive)
                Play();

            playTypeInternal = PlayType.Forward;

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            if (resetFirst)
                SetProgress(0, false);

            ApplyPlaybackDirection(PlayType.Forward);
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (!playingSequence.isAlive)
                Play();

            playTypeInternal = PlayType.Backward;

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            if (completeFirst)
                SetProgress(1, false);

            ApplyPlaybackDirection(PlayType.Backward);
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (!playingSequence.isAlive)
                Play();

            float clampedSeconds = Mathf.Clamp(seconds, 0f, playingSequence.durationTotal);
            playingSequence.elapsedTimeTotal = clampedSeconds;
            playingSequence.isPaused = !andPlay;
        }

        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            if (!playingSequence.isAlive)
                Play();
            
            targetProgress = Mathf.Clamp01(targetProgress);
            
            float duration = playingSequence.durationTotal;
            float finalTime = targetProgress * duration;
            SetTime(finalTime, andPlay);
        }

        public virtual void TogglePause()
        {
            if (!playingSequence.isAlive)
                return;

            playingSequence.isPaused = !playingSequence.isPaused;
        }

        public virtual void Pause()
        {
            if (!IsPlaying)
                return;

            playingSequence.isPaused = true;
        }

        public virtual void Resume()
        {
            if (!playingSequence.isAlive)
                return;

            playingSequence.isPaused = false;
        }


        public virtual void Complete(bool withCallbacks = true)
        {
            if (!playingSequence.isAlive)
                return;
            
            // Prepare for Complete().
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                animationStepBase.IsSkippingToEnd = true;
                if (animationStepBase is InvokeCallbackAnimationStep invokeCallbackStep)
                {
                    invokeCallbackStep.AllowCallbacks = withCallbacks;
                }
            }

            playingSequence.Complete();

            // Reset.
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                animationStepBase.IsSkippingToEnd = false;
                if (animationStepBase is InvokeCallbackAnimationStep invokeCallbackStep)
                {
                    invokeCallbackStep.AllowCallbacks = true;
                }
            }
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (!playingSequence.isAlive)
                return;

            playingSequence.elapsedTimeTotal = 0f;
            playingSequence.isPaused = true;
        }

        public virtual void Kill(bool complete = false)
        {
            if (!IsPlaying)
                return;

            if (complete)
                playingSequence.Complete();
            else
                playingSequence.Stop();
        }

        public virtual IEnumerator PlayEnumerator()
        {
            Play();
            while (playingSequence.isAlive)
                yield return null;
        }

        public virtual Sequence GenerateSequence()
        {
            int targetLoops = loops;
            if (!Application.isPlaying && loops == -1)
            {
                targetLoops = 10;
                Debug.LogWarning("Infinity sequences on editor can cause issues, using 10 loops while on editor.");
            }

            int cycles = PrimeTweenActionUtils.ToPrimeTweenCycles(targetLoops);
            Sequence sequence = Sequence.Create(cycles: cycles,
                cycleMode: PrimeTweenActionUtils.ToSequenceCycleMode(loopType),
                sequenceEase: PrimeTween.Ease.Linear,
                useUnscaledTime: timeScaleIndependent,
                updateType: PrimeTweenActionUtils.ToPrimeTweenUpdateType(updateType));

            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.ChainCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });

            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                animationStepBase.AddTweenToSequence(sequence);
            }
            // See comment above regarding bookending via AppendCallback.
            sequence.ChainCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onFinishedEvent.Invoke();
                }
                else
                {
                    onStartEvent.Invoke();
                }
            });

            sequence.timeScale = Mathf.Abs(playbackSpeed);
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            progress = -1.0f;
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                animationSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            if (playingSequence.isAlive)
                playingSequence.Stop();
            playingSequence = default;
        }

        public void SetAutoplayMode(AutoplayType autoplayType)
        {
            autoplayMode = autoplayType;
        }

        public void SetPlayOnAwake(bool targetPlayOnAwake)
        {
        }

        public void SetPauseOnAwake(bool targetPauseOnAwake)
        {
            startPaused = targetPauseOnAwake;
        }

        public void SetTimeScaleIndependent(bool targetTimeScaleIndependent)
        {
            timeScaleIndependent = targetTimeScaleIndependent;
        }

        public void SetPlayType(PlayType targetPlayType)
        {
            playType = targetPlayType;
        }

        public void SetUpdateType(UpdateType targetUpdateType)
        {
            updateType = targetUpdateType;
        }

        public void SetAutoKill(bool targetAutoKill)
        {
            autoKill = targetAutoKill;
        }

        public void SetLoops(int targetLoops)
        {
            loops = targetLoops;
        }

        public void SetTimeScale(float targetTimeScale)
        {
            if (!playingSequence.isAlive)
                return;
            
            playingSequence.timeScale = targetTimeScale;
        }

        private void Update()
        {
            if (playingSequence.isAlive && !playingSequence.isPaused)
                onProgressEvent.Invoke();

            if (progress == -1.0f)
                return;

            SetProgress(progress);
        }

        private void ApplyPlaybackDirection(PlayType type)
        {
            if (!playingSequence.isAlive)
                return;

            float baseScale = Mathf.Abs(playbackSpeed);
            playingSequence.timeScale = type == PlayType.Backward ? -baseScale : baseScale;
            playingSequence.isPaused = false;
        }

#if UNITY_EDITOR
        // Unity Event Function called when component is added or reset.
        private void Reset()
        {
            requiresReset = true;
        }

        // Used by the CustomEditor so it knows when to reset to the defaults.
        public bool IsResetRequired()
        {
            return requiresReset;
        }

        // Called by the CustomEditor once the reset has been completed 
        public void ResetComplete()
        {
            requiresReset = false;
        }
#endif
        public bool TryGetStepAtIndex<T>(int index, out T result) where T : AnimationStepBase
        {
            if (index < 0 || index > animationSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = animationSteps[index] as T;
            return result != null;
        }

        public void ReplaceTarget<T>(GameObject targetGameObject) where T : GameObjectAnimationStep
        {
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;

                if (animationStepBase is not T gameObjectAnimationStep)
                    continue;

                gameObjectAnimationStep.SetTarget(targetGameObject);
            }
        }

        public void ReplaceTargets(params (GameObject original, GameObject target)[] replacements)
        {
            for (int i = 0; i < replacements.Length; i++)
            {
                (GameObject original, GameObject target) replacement = replacements[i];
                ReplaceTargets(replacement.original, replacement.target);
            }
        }

        public void ReplaceTargets(GameObject originalTarget, GameObject newTarget)
        {
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;
                
                if(animationStepBase is not GameObjectAnimationStep gameObjectAnimationStep)
                    continue;

                if (gameObjectAnimationStep.Target == originalTarget)
                    gameObjectAnimationStep.SetTarget(newTarget);
            }
        }

#if UNITASK_ENABLED
        public async UniTask PlayAsync()
        {
            await PlayEnumerator().ToUniTask(this);
        }
#endif
    }
}
#endif
