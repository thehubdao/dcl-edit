using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Assets
{
    public interface IWebRequestSystem
    {
        void Get(string url, Action<UnityWebRequestAsyncOperation> then);
        Task<UnityWebRequestAsyncOperation> GetAsync(string url);
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

        public async Task<UnityWebRequestAsyncOperation> GetAsync(string url)
        {
            var request = UnityWebRequest.Get(url).SendWebRequest();

            while (!request.isDone)
            {
                await Task.Delay(1);
            }

            return request;
        }
    }
}