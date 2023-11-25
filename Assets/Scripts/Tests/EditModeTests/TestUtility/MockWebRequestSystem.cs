/*
using System;
using System.IO;
using System.Threading.Tasks;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockWebRequestSystem : IWebRequestSystem
    {
        public void Get(string url, Action<UnityWebRequestAsyncOperation> then)
        {
            if (url == "https://builder-api.decentraland.org/v1/assetPacks?owner=default")
            {
                GetAssetPacks(then);
            }
            else if (url.StartsWith("https://builder-api.decentraland.org/v1/storage/contents/"))
            {
                GetContent(url.Substring("https://builder-api.decentraland.org/v1/storage/contents/".Length), then);
            }
            else
            {
                throw new Exception($"url: {url} is not recognized and has no mock data associated");
            }
        }

        public async Task<UnityWebRequestAsyncOperation> GetAsync(string url)
        {
            var promise = new TaskCompletionSource<UnityWebRequestAsyncOperation>();

            Get(url, result => promise.TrySetResult(result));

            return await promise.Task;
        }

        private void GetAssetPacks(Action<UnityWebRequestAsyncOperation> then)
        {
            void OnRequestComplete(AsyncOperation operation)
            {
                var request = operation as UnityWebRequestAsyncOperation;

                if (request == null)
                {
                    Debug.LogError($"Failed: request is no UnityWebRequestAsyncOperation");
                    return;
                }

                if (request.webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.webRequest.result);
                    Debug.Log($"Url: {request.webRequest.url}");
                    Debug.Log($"Error: {request.webRequest.error}");
                    return;
                }

                then(request);
            }


            var baseUri = new Uri(Path.GetFullPath("../dcl-edit-testing/builderAssets/AssetData.json"));

            var request = UnityWebRequest.Get(baseUri).SendWebRequest();
            if (request.isDone)
            {
                OnRequestComplete(request);
            }
            else
            {
                request.completed += OnRequestComplete;
            }
        }

        private void GetContent(string hash, Action<UnityWebRequestAsyncOperation> then)
        {
            void OnRequestComplete(AsyncOperation operation)
            {
                var request = operation as UnityWebRequestAsyncOperation;

                if (request == null)
                {
                    Debug.LogError($"Failed: request is no UnityWebRequestAsyncOperation");
                    return;
                }

                if (request.webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.webRequest.result);
                    Debug.Log($"Url: {request.webRequest.url}");
                    Debug.Log($"Error: {request.webRequest.error}");
                    return;
                }

                then(request);
            }


            var baseUri = new Uri(Path.GetFullPath($"../dcl-edit-testing/builderAssets/hashes/{hash}"));

            var request = UnityWebRequest.Get(baseUri).SendWebRequest();
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
*/
