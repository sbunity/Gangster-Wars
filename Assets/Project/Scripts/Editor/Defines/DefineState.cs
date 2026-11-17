namespace SBabchuk.EditorTools.Defines
{
    public sealed class DefineState
    {
        public string Define { get; }
        public bool State { get; }

        public DefineState(string define, bool state)
        {
            Define = define;
            State = state;
        }
    }
}
