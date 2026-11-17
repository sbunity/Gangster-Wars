namespace SBabchuk.Runtime.Advertisement
{
    [AdsEditorContainer(typeof(AdDummyContainer))]
    public sealed class EditorDummyContainer : EditorAdsContainer
    {
        protected override string ContainerDisplayName => "Dummy";

        protected override void SpecialButtons() { }
    }
}
