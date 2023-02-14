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
            //check for duplicate directoryPaths (not null)
            if (sceneDirectoryState.directoryPath != null &&
                TryGetDirectoryState(sceneDirectoryState.directoryPath, out SceneDirectoryState _))
            {
                throw new ArgumentException($"Scenes are not allowed be added twice to sceneDirectoryStates: \"{sceneDirectoryState.directoryPath}\"");
            }

            sceneDirectoryStates.Add(sceneDirectoryState.id, sceneDirectoryState);
        }

        public void RemoveSceneDirectoryState(SceneDirectoryState sceneDirectoryState)
        {
            if (!sceneDirectoryStates.Remove(sceneDirectoryState.id))
            {
                throw new ArgumentException($"There is no SceneDirectoryState with the ID \"{sceneDirectoryState.id.Shortened()}\"");
            }
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
            return sceneDirectoryStates
                .First(t => (t.Value.directoryPath != null) && (Path.GetFullPath(t.Value.directoryPath) == Path.GetFullPath(path)))
                .Value;
        }

        public bool TryGetDirectoryState(string path, out SceneDirectoryState sceneDirectoryState)
        {
            sceneDirectoryState = sceneDirectoryStates
                .FirstOrDefault(t => (t.Value.directoryPath != null) && (Path.GetFullPath(t.Value.directoryPath) == Path.GetFullPath(path)))
                .Value;

            return sceneDirectoryState != null;
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