namespace Assets.Scripts.EditorState
{
    public interface IPathState
    {
        string ProjectPath { get; set; }
        string BuildPath { get; }
        string AssetPath { get; }
    }
}
