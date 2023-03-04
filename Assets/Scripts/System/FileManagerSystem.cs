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
        private PathState pathState;

        [Inject]
        private void Construct(PathState pathState)
        {
            this.pathState = pathState;
        }


        public IEnumerable<string> GetAllFilesWithExtension(params string[] extensions)
        {
            return extensions.SelectMany(extension => Directory.GetFiles(pathState.ProjectPath, $"*{extension}", SearchOption.AllDirectories));
        }
    }
}
