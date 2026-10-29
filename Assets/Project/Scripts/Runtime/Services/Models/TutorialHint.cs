using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services.Models
{
    public sealed class TutorialHint
    {
        public TutorialHint(
            ITutorialAnchor anchor,
            string caption,
            TutorialGesture gesture,
            TutorialInputMode inputMode,
            bool dimBackground,
            ITutorialAnchor destination = null,
            ITutorialAnchor captionAnchor = null)
        {
            Anchor = anchor;
            Caption = caption;
            Gesture = gesture;
            InputMode = inputMode;
            DimBackground = dimBackground;
            Destination = destination;
            CaptionAnchor = captionAnchor;
        }

        public ITutorialAnchor Anchor { get; }
        public string Caption { get; }
        public TutorialGesture Gesture { get; }
        public TutorialInputMode InputMode { get; }
        public bool DimBackground { get; }
        public ITutorialAnchor Destination { get; }
        public ITutorialAnchor CaptionAnchor { get; }
    }
}
