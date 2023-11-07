using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Zenject;
using HSVPicker;

namespace Assets.Scripts.System
{
    public class PromptSystem
    {
        private UnityState unityState;

        public PromptData promptData;

        [Inject]
        private void Construct(UnityState unityState)
        {
            this.unityState = unityState;
        }

        public void CreatePromptData(GameObject window, string dialogText, PromptAction[] actions, PromptAction notInWindowAction, PromptData.PromptType type)
        {
            promptData = new(window, dialogText, actions, notInWindowAction, type);
            actions ??= new PromptAction[0];
            foreach (var action in actions)
                action.data = promptData;
            if(notInWindowAction != null)
                notInWindowAction.data = promptData;
        }

        public async Task<PromptAction> CreateDialog(string dialogText = "", PromptAction[] actions = null, PromptAction notInWindowAction = null)
        {
            CreatePromptData(unityState.PromptDialog, dialogText, actions, notInWindowAction, PromptData.PromptType.Dialog);
            unityState.PromptDialog.SetActive(true);
            return await promptData.taskCompleted.Task;
        }

        public async Task<Value<Color>> CreateColorPicker(Color initColor)
        {
            unityState.ColorPicker.GetComponent<ColorPicker>().CurrentColor = initColor;
            CreatePromptData(unityState.PromptColorPicker, "Choose a color", null, null, PromptData.PromptType.ColorPicker);
            unityState.PromptColorPicker.SetActive(true);
            return (Value<Color>)await promptData.taskCompleted.Task;
        }

        public async Task<Value<Guid>> CreateAssetMenu()
        {
            CreatePromptData(unityState.PromptAssetPicker, "Choose an asset", null, null, PromptData.PromptType.AssetPicker);
            unityState.PromptAssetPicker.SetActive(true);
            return (Value<Guid>)await promptData.taskCompleted.Task;
        }

        public abstract class PromptAction
        {
            public Action action;
            public PromptData data;
            public Action ToDo => action;
            public string name;
            public void Submit(GameObject go = null)
            {
                action?.Invoke();
                data.window?.SetActive(false);
                data.taskCompleted.SetResult(this);
            }

            public PromptAction(Action action = null) => this.action = action;
        }

        public class Yes : PromptAction
        {
            public Yes(Action action = null) : base(action) => name = "Yes";
        }

        public class No : PromptAction
        {
            public No(Action action = null) : base(action) => name = "No";
        }

        public class Cancel : PromptAction
        {
            public Cancel(Action action = null) : base(action) => name = "Cancel";
        }

        public class OK : PromptAction
        {
            public OK(Action action = null) : base(action) => name = "Ok";
        }

        public class Value<T> : PromptAction
        {
            public T value;
            public Value(Action action = null) : base(action) => name = "Value";
        }

        public class NotInWindow : PromptAction
        {
            public NotInWindow(Action action = null) : base(action) => name = "NotInWindow";
        }

        public class PromptData
        {
            public string dialogText;
            public GameObject window;
            public PromptAction[] actions;
            public PromptAction notInWindowAction;
            public PromptType promptType;
            public TaskCompletionSource<PromptAction> taskCompleted = new();

            public PromptData(GameObject window, string dialogText, PromptAction[] actions, PromptAction notInWindowAction, PromptType promptType)
            {
                this.window = window;
                this.dialogText = dialogText ?? "";
                this.actions = actions ?? new PromptAction[0];
                this.notInWindowAction = notInWindowAction;
                this.promptType = promptType;
            }

            public enum PromptType
            {
                Dialog,
                ColorPicker,
                AssetPicker
            }
        }
    }
}