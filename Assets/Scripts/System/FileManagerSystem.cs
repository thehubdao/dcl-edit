using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.EditorState;
using Zenject;

namespace Assets.Scripts.System
{
    public class FileManagerSystem
    {
        // Dependencies
        private IPathState pathState;

        [Inject]
        public void Construct(IPathState pathState)
        {
            this.pathState = pathState;
        }


        public IEnumerable<string> GetAllFilePathsWithExtension(params string[] extensions)
        {
            return extensions.SelectMany(extension => Directory.GetFiles(pathState.ProjectPath, $"*{extension}", SearchOption.AllDirectories));
        }

        public enum FileWatcherEvent
        {
            Initial,
            Created,
            Changed,
            Deleted,
            Renamed
        }

        /// <summary>
        /// Sets up the FileSystemWatcher on a directory and its subdirectories
        /// </summary>
        /// <param name="path">the path to the directory to watch. If null, used the ProjectPath</param>
        /// <param name="filter">a filter to include only specific files. Use '*' as a wildcard</param>
        /// <param name="callback">a callback that gets the event, the path of the changed file, and path of the file before the change (file rename)</param>
        public void ForAllFilesByFilterNowAndOnChange(string path, string filter, Action<FileWatcherEvent, string, string> callback)
        {
            path ??= pathState.ProjectPath;

            var files = Directory.GetFiles(path, filter, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                callback(FileWatcherEvent.Initial, file, file);
            }

            var fsw = new FileSystemWatcher(path);
            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.FileName;

            fsw.Created += (_, e) => { callback(FileWatcherEvent.Created, e.FullPath, e.FullPath); };
            fsw.Changed += (_, e) => { callback(FileWatcherEvent.Changed, e.FullPath, e.FullPath); };
            fsw.Deleted += (_, e) => { callback(FileWatcherEvent.Deleted, e.FullPath, e.FullPath); };
            fsw.Renamed += (_, e) => { callback(FileWatcherEvent.Renamed, e.FullPath, e.OldFullPath); };

            fsw.Filter = filter;
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;
        }

        public string GetFilePath(string path)
        {
            return Path.Combine(pathState.ProjectPath, path);
        }
    }
}
