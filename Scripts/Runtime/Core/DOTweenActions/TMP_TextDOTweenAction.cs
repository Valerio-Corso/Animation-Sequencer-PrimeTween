#if PRIMETWEEN_ENABLED
#if TMP_ENABLED

using System;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{

    [Serializable]
    public sealed class TMP_TextDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(TMP_Text);
        public override string DisplayName => "TMP Text";

        [SerializeField]
        private string text;
        public string Text
        {
            get => text;
            set => text = value;
        }

        [SerializeField]
        private bool richText;
        public bool RichText
        {
            get => richText;
            set => richText = value;
        }

        [SerializeField]
        private ScrambleMode scrambleMode = ScrambleMode.None;
        public ScrambleMode ScrambleMode
        {
            get => scrambleMode;
            set => scrambleMode = value;
        }
        
        private TMP_Text tmpTextComponent;
        
        private string previousText;
        private int previousMaxVisibleCharacters;
        private TMP_Text previousTarget;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (tmpTextComponent == null)
            {
                tmpTextComponent = target.GetComponent<TMP_Text>();
                if (tmpTextComponent == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return default;
                }
            }

            previousText = tmpTextComponent.text;
            previousMaxVisibleCharacters = tmpTextComponent.maxVisibleCharacters;
            previousTarget = tmpTextComponent;

            if (scrambleMode != ScrambleMode.None)
                Debug.LogWarning($"{DisplayName} ignores '{nameof(scrambleMode)}' with PrimeTween migration.");

            tmpTextComponent.text = text ?? string.Empty;
            tmpTextComponent.ForceMeshUpdate();
            int totalCharacters = tmpTextComponent.textInfo.characterCount;

            int start = Direction == AnimationDirection.From ? totalCharacters : 0;
            int end = Direction == AnimationDirection.From ? 0 : totalCharacters;

            return Tween.TextMaxVisibleCharacters(tmpTextComponent, start, end, duration, Ease.ToEasing());
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null)
                return;
            
            if (string.IsNullOrEmpty(previousText))
                return;

            previousTarget.text = previousText;
            previousTarget.maxVisibleCharacters = previousMaxVisibleCharacters;
        }
    }
}
#endif
#endif
