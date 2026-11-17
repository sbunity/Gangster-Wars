using SBabchuk.EditorTools;
using UnityEditor;

namespace SBabchuk.Runtime.Advertisement
{
    public abstract class EditorAdsContainer
    {
        protected SerializedProperty _containerProperty;

        protected abstract string ContainerDisplayName { get; }

        public virtual void Init(SerializedProperty containerProp)
        {
            _containerProperty = containerProp;
        }

        public virtual void DrawContainer()
        {
            _containerProperty.isExpanded = EditorBoxGroup.BeginExpandBoxGroup(ContainerDisplayName, _containerProperty.isExpanded);

            if (_containerProperty.isExpanded)
            {
                DrawChildren(_containerProperty);

                SpecialButtons();
            }

            EditorBoxGroup.EndBoxGroup();
        }

        protected abstract void SpecialButtons();

        private static void DrawChildren(SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            if (!iterator.NextVisible(true))
                return;

            do
            {
                if (SerializedProperty.EqualContents(iterator, end))
                    break;

                EditorGUILayout.PropertyField(iterator, true);
            }
            while (iterator.NextVisible(false));
        }
    }
}
