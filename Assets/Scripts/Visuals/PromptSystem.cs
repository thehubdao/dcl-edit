using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PromptSystem
{
    private UnityState unityState;

    [Inject]
    private void Construct(UnityState unityState)
    {
        this.unityState = unityState;
    }

    public async Task<Action> CreateDialog(string dialogText = "", Action[] actions = null, Action notInWindowAction = null)
    {
        var promptVisual = unityState.PromptDialog.GetComponent<UiPromptVisuals>();
        promptVisual.ActivateDialog(dialogText, actions, notInWindowAction);
        return await promptVisual.data.taskCompleted.Task;
    }

    public async Task<Value<Color>> CreateColorPicker()
    {
        var promptVisual = unityState.PromptColorPicker.GetComponent<UiPromptVisuals>();
        promptVisual.ActivateColorPicker("Choose a color", unityState.ColorPicker);
        return (Value<Color>)await promptVisual.data.taskCompleted.Task;
    }

    public async Task<Value<System.Guid>> CreateAssetMenu()
    {
        var promptVisual = unityState.PromptAssetPicker.GetComponent<UiPromptVisuals>();
        promptVisual.ActivateAssetBrowser("Choose an asset.");
        return (Value<System.Guid>)await promptVisual.data.taskCompleted.Task;
    }

    public abstract class Action
    {
        public UiPromptVisuals.Data data;
        public System.Action action;
        public System.Action ToDo => action;
        public string name;
        public void Submit(GameObject go) 
        {
            action?.Invoke(); 
            data.window?.SetActive(false); 
            data.taskCompleted.SetResult(this);
        }
    }

    public class Yes : Action
    {
        public Yes() => name = "Yes";
        public Yes(System.Action action) 
        { 
            this.action = action; 
            name = "Yes"; 
        }
    }

    public class No : Action
    {
        public No() => name = "No";
        public No(System.Action action) 
        { 
            this.action = action;
            name = "No"; 
        }
    }

    public class Cancel : Action
    {
        public Cancel() => name = "Cancel";
        public Cancel(System.Action action) 
        {
            this.action = action; 
            name = "Cancel"; 
        }
    }

    public class OK : Action
    {
        public OK() => name = "OK";
        public OK(System.Action action) 
        { 
            this.action = action;
            name = "OK";
        }
    }

    public class Value<T> : Action
    {
        public T value;
        public Value() => name = "Value";
        public Value(System.Action action) 
        {
            this.action = action;
            name = "Value";
        }
    }

    public class NotInWindow: Action
    {
        public NotInWindow() => name = "NotInWindow";
        public NotInWindow(System.Action action) 
        { 
            this.action = action;
            name = "NotInWindow";
        }
    }
}