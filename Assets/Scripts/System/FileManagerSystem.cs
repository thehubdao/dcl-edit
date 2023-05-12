using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace Assets.Scripts.System
{
    public class FileManagerSystem
    {
        // Dependencies
        private IPathState pathState;

        [Inject]
        public void Construct(IPathState pathState)
        {
            this.pathState = pathState;
        }


        public IEnumerable<string> GetAllFilePathsWithExtension(params string[] extensions)
        {
            return extensions.SelectMany(extension => Directory.GetFiles(pathState.ProjectPath, $"*{extension}", SearchOption.AllDirectories));
        }

        public string GetFilePath(string path)
        {
            return Path.Combine(pathState.ProjectPath, path);
        }
    }
}
