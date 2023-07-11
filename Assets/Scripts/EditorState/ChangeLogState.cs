﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class ChangeLogState
    {
        [CanBeNull]
        private List<ChangeLogStructure> changeLog;

        public List<ChangeLogStructure> ChangeLog
        {
            get
            {
                if (changeLog != null) return changeLog;

                var changeLogFile = Resources.Load<TextAsset>("changeLog");
                if (changeLogFile != null)
                {
                    var data = JsonUtility.FromJson<Log>(changeLogFile.text);
                    changeLog = data.changeLog;
                }

                return changeLog;
            }
        }
        
        [Serializable]
        public class ChangeLogStructure
        {
            public int order { get; set; }
            public string version { get; set; }
            public string details { get; set; }
        }

        [Serializable]
        public class Log
        {
            public List<ChangeLogStructure> changeLog;
        }
    }
}