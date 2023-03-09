using System.IO;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class CheckVersionSystem
    {
        private IPathState pathState;

        [Inject]
        public void Construct(
            IPathState pathState)
        {
            this.pathState = pathState;
        }

        public bool CheckDclSceneExists()
        {
            return File.Exists(Path.Combine(pathState.ProjectPath, "scene.json"));
        }

        private string[] GetBetaProjectPaths()
        {
            string assetsDirPath = Path.Combine(pathState.ProjectPath, "assets");
            return Directory.GetDirectories(assetsDirPath, "*.dclscene", SearchOption.AllDirectories);
        }

        public bool TryGetBetaPaths(out string[] betaPaths)
        {
            betaPaths = GetBetaProjectPaths();
            return betaPaths.Length > 0;
        }

        private string GetAlphaProjectPath()
        {
            return pathState.ProjectPath;
        }

        public bool CheckForAlpha()
        {
            var alphaPath = GetAlphaProjectPath();
            return File.Exists(Path.Combine(alphaPath, "dcl-edit/saves/assets.json")) &&
                   File.Exists(Path.Combine(alphaPath, "dcl-edit/saves/save.json")) &&
                   File.Exists(Path.Combine(alphaPath, "dcl-edit/saves/project.json"));
        }
    }
}
