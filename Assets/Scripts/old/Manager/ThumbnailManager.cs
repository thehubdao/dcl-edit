using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;

public class ThumbnailManager : MonoBehaviour,ISerializedFieldToStatic
{

    [SerializeField]
    private Texture2D _defaultThumbnail;

    public static Texture2D DefaultThumbnail { private set; get; }

    public static ThumbnailManager Instance { private set; get; }

    public void SetupStatics()
    {
        DefaultThumbnail = _defaultThumbnail;
        Instance = this;
    }
    
    private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();


    public static void GetThumbnail(string path,Action<Texture2D> callback)
    {
        if (_cache.ContainsKey(path))
        {
            callback.Invoke(_cache[path]);
        }
        else if (path != "" && File.Exists(path))
        {
            Instance.GetThumbnailFromFileDispatcher(path, thumbnail =>
            {
                if (!_cache.ContainsKey(path))
                {
                    _cache.Add(path,thumbnail);
                }
                callback.Invoke(thumbnail);
            });
        }
        else
        {
            callback.Invoke(DefaultThumbnail);
        }
    }

    private void GetThumbnailFromFileDispatcher(string thumbnailPath,Action<Texture2D> callback)
    {
        StartCoroutine(GetThumbnailFromFile(thumbnailPath, callback));
    }

    private IEnumerator GetThumbnailFromFile(string thumbnailPath,Action<Texture2D> callback)
    {
        var request = UnityWebRequestTexture.GetTexture("file://" + thumbnailPath);
        yield return request.SendWebRequest();

        Texture2D thumbnail;
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            thumbnail = DefaultThumbnail;
        }
        else
        {
            thumbnail = DownloadHandlerTexture.GetContent(request);
        }

        callback.Invoke(thumbnail);
    }

}
