using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Assets.Scripts.Utility;

namespace Assets.Scripts.EditorState
{
    public class SceneManagerState
    {
        private readonly Dictionary<Guid, SceneDirectoryState> sceneDirectoryStates = new Dictionary<Guid, SceneDirectoryState>();

        public Guid currentSceneIndex { get; private set; } = Guid.Empty;

        public IReadOnlyCollection<SceneDirectoryState> allSceneDirectoryStates => sceneDirectoryStates.Values;

        public void SetCurrentSceneIndex(Guid index)
        {
            if (!Exists(index))
            {
                throw new ArgumentOutOfRangeException($"There is no SceneDirectoryState with the ID \"{index.Shortened()}\"");
            }

            currentSceneIndex = index;
        }

        public void AddSceneDirectoryState(SceneDirectoryState sceneDirectoryState)
        {
            sceneDirectoryStates.Add(sceneDirectoryState.id, sceneDirectoryState);
        }

        private bool Exists(Guid id)
        {
            return sceneDirectoryStates.ContainsKey(id);
        }

        public SceneDirectoryState GetDirectoryState(Guid index)
        {
            if (!Exists(index))
            {
                throw new ArgumentOutOfRangeException($"There is no SceneDirectoryState with the ID \"{index.Shortened()}\"");
            }

            return sceneDirectoryStates[index];
        }

        public SceneDirectoryState GetDirectoryState(string path)
        {
            return sceneDirectoryStates.First(t => Path.GetFullPath(t.Value.directoryPath) == Path.GetFullPath(path)).Value;
        }

        [CanBeNull]
        public SceneDirectoryState GetCurrentDirectoryState()
        {
            if (currentSceneIndex == Guid.Empty)
            {
                return null;
            }

            return sceneDirectoryStates[currentSceneIndex];
        }
    }
}