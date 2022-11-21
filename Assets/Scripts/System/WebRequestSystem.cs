using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.System
{
    public interface IWebRequestSystem
    {
        void Get(string url, Action<UnityWebRequestAsyncOperation> then);
    }

    public class WebRequestSystem : IWebRequestSystem
    {
        public void Get(string url, Action<UnityWebRequestAsyncOperation> then)
        {
            void OnRequestComplete(AsyncOperation operation)
            {
                var request = operation as UnityWebRequestAsyncOperation;

                if (request == null)
                {
                    Debug.LogError("request is null");
                    return;
                }

                then(request);
            }

            var request = UnityWebRequest.Get(url).SendWebRequest();
            if (request.isDone)
            {
                OnRequestComplete(request);
            }
            else
            {
                request.completed += OnRequestComplete;
            }
        }
    }
}