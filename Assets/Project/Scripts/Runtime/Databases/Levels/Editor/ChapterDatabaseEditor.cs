using UnityEditor;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.Levels
{
    [CustomEditor(typeof(ChapterDatabase))]
    public class ChapterDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            LevelDatabaseDrawer.Draw((ChapterDatabase)Database, SelectedMode);
        }
    }
}
