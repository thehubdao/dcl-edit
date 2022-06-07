namespace Assets.Scripts.ProjectState
{
    public class DclGltfAsset : DclAsset
    {
        public override string TypeName => "GLTF";


        public DclGltfAsset(string name, string path)
        {
            _name = name;
            _path = path;
        }

        // Name
        private string _name;
        public override string Name => _name;

        // Gltf Path
        private string _path;
        public string Path => _path;


    }
}
