using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityGLTF;
using Zenject;
using Debug = UnityEngine.Debug;

public class BlenderExecutor
{
    // Dependencies
    private SettingsSystem settingsSystem;
    private ThreadManager threadManager;

    [Inject]
    private void Construct(
        SettingsSystem settingsSystem,
        ThreadManager threadManager)
    {
        this.settingsSystem = settingsSystem;
        this.threadManager = threadManager;
    }

    public bool CheckBlender()
    {
        try
        {
            var startInfo = new ProcessStartInfo(settingsSystem.blenderExecPath.Get(),
                "-v " +
                //"E:\\Data\\Decentraland\\sdk7-test-scene\\assets\\models\\TestCube.blend " +
                //"--python-expr \"import bpy; bpy.ops.export_scene.gltf(filepath='E:/Data/Decentraland/sdk7-test-scene/assets/models/TestCube.glb')\" " +
                "")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            Process.Start(startInfo);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void ConvertBlendToGlb(string blendPath, string glbPath, Action<string> then)
    {
        try
        {
            var directoryName = Path.GetDirectoryName(glbPath);
            Assert.IsNotNull(directoryName);
            Directory.CreateDirectory(directoryName!);

            glbPath = glbPath.Replace("\\", "/");

            var startInfo = new ProcessStartInfo(settingsSystem.blenderExecPath.Get(),
                $@"-b {blendPath} --python-expr ""
import bpy;
import sys;
try:
  bpy.ops.export_scene.gltf(filepath='{glbPath}',check_existing=False)
except:
  sys.exit(1)
""")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };


            var p = new Process();
            p.StartInfo = startInfo;
            p.EnableRaisingEvents = true;
            p!.Exited += (_, _) =>
            {
                if (p.ExitCode == 0)
                {
                    threadManager.DoOnNextUpdate(() => then(glbPath));
                }
                else
                {
                    Debug.Log($"Convert exit with code {p.ExitCode}");
                    Debug.Log(p.StandardOutput.ReadToEnd());
                    threadManager.DoOnNextUpdate(() => then(null));
                }
            };
            p.Start();
        }
        catch (Exception e)
        {
            then(null);
            Debug.LogException(e);
        }
    }
}
