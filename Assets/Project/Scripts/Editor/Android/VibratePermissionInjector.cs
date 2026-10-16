#if UNITY_ANDROID
using System.IO;
using System.Xml;
using UnityEditor.Android;

namespace SBabchuk.Editor.Android
{
    public sealed class VibratePermissionInjector : IPostGenerateGradleAndroidProject
    {
        private const string ManifestRelativePath = "src/main/AndroidManifest.xml";
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";
        private const string PermissionElement = "uses-permission";
        private const string VibratePermission = "android.permission.VIBRATE";

        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var manifestPath = Path.Combine(path, ManifestRelativePath);
            if (!File.Exists(manifestPath))
                return;

            var document = new XmlDocument();
            document.Load(manifestPath);

            var manifest = document.DocumentElement;
            if (manifest == null || HasPermission(manifest))
                return;

            var permission = document.CreateElement(PermissionElement);
            permission.SetAttribute("name", AndroidNamespace, VibratePermission);
            manifest.PrependChild(permission);
            document.Save(manifestPath);
        }

        private bool HasPermission(XmlElement manifest)
        {
            foreach (XmlNode node in manifest.GetElementsByTagName(PermissionElement))
            {
                if (node is XmlElement element && element.GetAttribute("name", AndroidNamespace) == VibratePermission)
                    return true;
            }

            return false;
        }
    }
}
#endif
