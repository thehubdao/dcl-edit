using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockSceneManagerSystem : ISceneManagerSystem
    {
        private DclScene currentScene = null;

        public void DiscoverScenes()
        {
        }

        public SceneDirectoryState GetCurrentDirectoryState()
        {
            throw new NotImplementedException();
        }

        public DclScene GetCurrentScene()
        {
            if (currentScene == null)
            {
                currentScene = new DclScene();
            }
            return currentScene;
        }

        public DclScene GetCurrentSceneOrNull()
        {
            if (currentScene == null)
            {
                currentScene = new DclScene();
            }
            return currentScene;
        }

        public DclScene GetScene(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SaveCurrentScene()
        {
        }

        public void SaveCurrentSceneAs()
        {
        }

        public void SaveScene(Guid id)
        {
        }

        public void SaveSceneAs(Guid id)
        {
        }

        public void SetCurrentScene(Guid id)
        {
        }

        public void SetCurrentScene(string path)
        {
        }

        public void SetDialogAsCurrentScene()
        {
        }

        public void SetFirstSceneAsCurrentScene()
        {
        }

        public void SetLastOpenedSceneAsCurrentScene()
        {
        }

        public void SetNewSceneAsCurrentScene()
        {
        }
    }
}