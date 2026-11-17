namespace SBabchuk.EditorTools.Defines
{
    public sealed class RegisteredDefine
    {
        public string Define { get; }
        public string AssemblyType { get; }
        public string FilePath { get; }

        public RegisteredDefine(string define, string assemblyType, string filePath)
        {
            Define = define;
            AssemblyType = assemblyType;
            FilePath = filePath;
        }
    }
}
