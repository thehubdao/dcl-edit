using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.System.Utility
{
    public class FileIOUtility
    {   
        public bool CheckFileExists(string path)
        {
            return File.Exists(path);
        }
        
        public bool CheckFileExists(string path, string changeExtension)
        {
            return CheckFileExists(Path.ChangeExtension(path, changeExtension));
        }
        
        public (bool success, T result) ReadFileToJson<T>(string path)
        {
            if (!CheckFileExists(path))
            {
                Debug.LogError($"File {path} not found");
                return (false, default);
            }

            try
            {
                var fileContent = File.ReadAllText(path);
                var assetsJson = JsonConvert.DeserializeObject<T>(fileContent);

                return (true, assetsJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error serializing file to Json, file: {path}, message: {e.Message}");
                return (false, default);
            }
        }
        

        public (bool success, T result) ReadFileToJson<T>(string path, string changeExtension)
        {
            return ReadFileToJson<T>(Path.ChangeExtension(path, changeExtension));
        }

        public bool WriteToFile(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error trying to write file {path}, msg: {e.Message}");
                return false;
            }
        }

        public (bool success, byte[] result) ReadAllBytes(string path)
        {
            try
            {
                return (true, File.ReadAllBytes(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading bytes of file: {path}, msg: {e.Message}");
                return (false, null);
            }
        }

        public (bool success, string result) ChangeExtension(string path, string changeExtension)
        {
            try
            {
                return (true, Path.ChangeExtension(path, changeExtension));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading file: {path}, msg: {e.Message}");
                return (false, string.Empty);
            }
        }

        public (bool success, string result) GetFileName(string path)
        {
            try
            {
                return (true, Path.GetFileName(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading name of file: {path}, msg: {e.Message}");
                return (false, string.Empty);
            }
        }
    }
}
