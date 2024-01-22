using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityGLTF.Loader;

public class FileLoader : IDataLoader2
{
    private readonly string _rootDirectoryPath;

    public FileLoader(string rootDirectoryPath)
    {
        _rootDirectoryPath = rootDirectoryPath;
    }

    public Task<Stream> LoadStreamAsync(string relativeFilePath)
    {
#if !WINDOWS_UWP && !UNITY_WEBGL
        // seems the Editor locks up in some cases when directly using Task.Run(() => {})
        if (UnityEngine.Application.isPlaying)
        {
            return Task.Run(() => LoadStream(relativeFilePath));
        }
#endif
        return Task.FromResult(LoadStream(relativeFilePath));
    }

    public Stream LoadStream(string relativeFilePath)
    {
        if (relativeFilePath == null)
        {
            throw new ArgumentNullException(nameof(relativeFilePath));
        }

        if (File.Exists(relativeFilePath))
            return GetStream(relativeFilePath);

        string pathToLoad = Path.Combine(_rootDirectoryPath, relativeFilePath);
        if (!File.Exists(pathToLoad))
        {
            pathToLoad = Path.Combine(_rootDirectoryPath, Uri.UnescapeDataString(relativeFilePath));
        }

        if (!File.Exists(pathToLoad))
        {
            if (relativeFilePath.ToLowerInvariant().EndsWith(".bin"))
                throw new FileNotFoundException("Buffer file " + relativeFilePath + " not found in " + _rootDirectoryPath + ", complete path: " + pathToLoad, relativeFilePath);

            UnityEngine.Debug.LogError("Buffer file " + relativeFilePath + " not found in " + _rootDirectoryPath + ", complete path: " + pathToLoad);
            return InvalidStream;
        }

        return GetStream(pathToLoad);
    }

    private Stream GetStream(string path)
    {
        var fileStream = File.OpenRead(path);
        var data = new byte[fileStream.Length];
        var readCount = fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        Assert.IsTrue(data.Length == readCount);

        var memoryStream = new MemoryStream(data, 0, data.Length, true, true);
        return memoryStream;
    }

    internal static readonly Stream InvalidStream = new MemoryStream();
}
