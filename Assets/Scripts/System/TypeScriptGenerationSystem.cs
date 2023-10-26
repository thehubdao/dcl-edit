using System;
using System.IO;
using System.Threading.Tasks;
using Assets.Scripts.EditorState;
using Zenject;
using Debug = UnityEngine.Debug;

// Warning 0162 warns about unreachable code.
// The variable _obfuscate is currently constantly false.
// This variable is then used in an if statement, that leads to the unreachable code.
// This will be changed in the future.
#pragma warning disable 0162

namespace Assets.Scripts.System
{
    public class TypeScriptGenerationSystem
    {
        // Dependencies
        private ExposeEntitySystem exposeEntitySystem;
        private IPathState pathState;
        private SceneManagerState sceneManagerState;
        private SceneManagerSystem sceneManagerSystem;
        private AssetManagerSystem assetManagerSystem;
        private AvailableComponentsState availableComponentsState;

        private SceneJsonReaderSystem sceneJsonReaderSystem;
        private Ecs7GenerationSystem ecs7GenerationSystem;

        [Inject]
        private void Construct(
            ExposeEntitySystem exposeEntitySystem,
            IPathState pathState,
            SceneManagerState sceneManagerState,
            SceneManagerSystem sceneManagerSystem,
            AssetManagerSystem assetManagerSystem,
            AvailableComponentsState availableComponentsState,
            SceneJsonReaderSystem sceneJsonReaderSystem,
            Ecs7GenerationSystem ecs7GenerationSystem)
        {
            this.exposeEntitySystem = exposeEntitySystem;
            this.pathState = pathState;
            this.sceneManagerState = sceneManagerState;
            this.sceneManagerSystem = sceneManagerSystem;
            this.assetManagerSystem = assetManagerSystem;
            this.availableComponentsState = availableComponentsState;
            this.sceneJsonReaderSystem = sceneJsonReaderSystem;
            this.ecs7GenerationSystem = ecs7GenerationSystem;
        }


        public async Task<bool> GenerateTypeScript()
        {
            try
            {
                string script;
                if (sceneJsonReaderSystem.IsEcs7())
                {
                    script = await ecs7GenerationSystem.GenerateScript();
                }
                else
                {
                    script = "ecs6 generation here";
                }


                var scriptsFolderPath = pathState.ProjectPath + "/dcl-edit/build/scripts/";

                Directory.CreateDirectory(scriptsFolderPath);

                File.WriteAllText(scriptsFolderPath + "scenes.ts", script);

                Debug.Log("Script generation done");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}

#pragma warning restore 0162
