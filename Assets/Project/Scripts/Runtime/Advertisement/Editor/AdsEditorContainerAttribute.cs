using System;

namespace SBabchuk.Runtime.Advertisement
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AdsEditorContainerAttribute : Attribute
    {
        public Type ContainerType { get; }

        public AdsEditorContainerAttribute(Type containerType)
        {
            ContainerType = containerType;
        }
    }
}
