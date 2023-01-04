using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Assets.Scripts.EditorState
{
    public class SceneManagerState
    {
        private readonly Dictionary<Guid, SceneDirectoryState> sceneDirectoryStates = new Dictionary<Guid, SceneDirectoryState>();

        public Guid currentSceneIndex { get; private set; } = Guid.Empty;

        public IReadOnlyCollection<SceneDirectoryState> allSceneDirectoryStates => sceneDirectoryStates.Values;

        public void SetCurrentSceneIndex(Guid index)
        {
            currentSceneIndex = index;
        }

        public void AddSceneDirectoryState(SceneDirectoryState sceneDirectoryState)
        {
            sceneDirectoryStates.Add(sceneDirectoryState.id, sceneDirectoryState);
        }

        public bool Exists(Guid id)
        {
            return sceneDirectoryStates.ContainsKey(id);
        }

        [CanBeNull]
        public SceneDirectoryState GetDirectoryState(Guid index)
        {
            if (sceneDirectoryStates.TryGetValue(index, out var value))
            {
                return value;
            }

            return null;
        }

        [CanBeNull]
        public SceneDirectoryState GetCurrentDirectoryState()
        {
            if (sceneDirectoryStates.TryGetValue(currentSceneIndex, out var value))
            {
                return value;
            }

            return null;
        }
    }
}