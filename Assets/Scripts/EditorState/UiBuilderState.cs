using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Events;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class UiBuilderState
{
    // Dependencies
    private EditorEvents _events;

    [Inject]
    public void Constuct(EditorEvents events)
    {
        _events = events;
    }


    private Dictionary<EventDependentTypes.UiBuilderSetupKey, UiBuilderSetup> _uiBuilderSetups = new Dictionary<EventDependentTypes.UiBuilderSetupKey, UiBuilderSetup>();

    public void AddBuilderSetup(EventDependentTypes.UiBuilderSetupKey key, List<UiBuilderAtom> atoms)
    {
        if (_uiBuilderSetups.ContainsKey(key))
        {
            _uiBuilderSetups[key] = new UiBuilderSetup(atoms);
        }
        else
        {
            _uiBuilderSetups.Add(key, new UiBuilderSetup(atoms));
        }

        _events.InvokeUiBuilderChangedEvent(key);
    }

    public UiBuilderSetup GetBuilderSetup(EventDependentTypes.UiBuilderSetupKey key)
    {
        return _uiBuilderSetups.TryGetValue(key, out var builderSetup) ?
            builderSetup :
            new UiBuilderSetup();
    }
}

public class UiBuilderSetup
{
    private readonly List<UiBuilderAtom> _uiBuilderAtoms;

    public UiBuilderSetup(List<UiBuilderAtom> uiBuilderAtoms)
    {
        _uiBuilderAtoms = uiBuilderAtoms;
    }

    public UiBuilderSetup()
    {
        _uiBuilderAtoms = new List<UiBuilderAtom>();
    }

    public IEnumerable<UiBuilderAtom> atoms => _uiBuilderAtoms;
}

public abstract class UiBuilderAtom
{
    public enum AtomType
    {
        Title,
        Text,
        Spacer,
        Panel,
        PanelHeader,
        HierarchyItem,
        StringPropertyInput,
        NumberPropertyInput,
        BooleanPropertyInput,
        Vector3PropertyInput
    }

    public enum TextStyle
    {
        Normal,
        PrimarySelection,
        SecondarySelection,
    }

    public struct UiPropertyActions<T>
    {
        public Action<T> OnChange;
        public Action<T> OnSubmit;
        public Action<T> OnAbort;
    }

    public abstract AtomType type { get; }
}

public class UiTitleAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.Title;

    public UiTitleAtom(string title)
    {
        this.title = title;
    }

    public string title { get; }
}

public class UiTextAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.Text;

    public UiTextAtom(string text)
    {
        this.text = text;
    }

    public string text { get; }
}

public class UiSpacerAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.Spacer;

    public UiSpacerAtom(float height)
    {
        this.height = height;
    }

    public float height { get; }
}

public class UiPanelAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.Panel;

    public UiPanelAtom(EventDependentTypes.UiBuilderSetupKey contentKey)
    {
        this.contentKey = contentKey;
    }

    public EventDependentTypes.UiBuilderSetupKey contentKey { get; }
}

public class UiPanelHeaderAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.PanelHeader;

    public UiPanelHeaderAtom(string title, [CanBeNull] Action onClose)
    {
        this.title = title;
        this.onClose = onClose;
    }

    public string title { get; }

    [CanBeNull]
    public Action onClose { get; }
}

public class UiHierarchyItemAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.HierarchyItem;

    public UiHierarchyItemAtom(string name, int level, bool hasChildren, bool isExpanded, TextStyle textStyle, Action onArrowClick, Action onNameClick)
    {
        this.name = name;
        this.level = level;
        this.hasChildren = hasChildren;
        this.isExpanded = isExpanded;
        this.textStyle = textStyle;
        this.onArrowClick = onArrowClick;
        this.onNameClick = onNameClick;
    }

    public string name { get; }
    public int level { get; }
    public bool hasChildren { get; }
    public bool isExpanded { get; }
    public TextStyle textStyle { get; }
    public Action onArrowClick { get; }
    public Action onNameClick { get; }
}

public class UiStringPropertyInputAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.StringPropertyInput;

    public UiStringPropertyInputAtom(string name, string placeholder, string currentContents, UiPropertyActions<string> actions)
    {
        this.name = name;
        this.placeholder = placeholder;
        this.currentContents = currentContents;
        this.actions = actions;
    }

    public string name { get; }
    public string placeholder { get; }
    public string currentContents { get; }
    public UiPropertyActions<string> actions { get; }
}

public class UiNumberPropertyInputAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.NumberPropertyInput;

    public UiNumberPropertyInputAtom(string name, string placeholder, float currentContents, UiPropertyActions<float> actions)
    {
        this.name = name;
        this.placeholder = placeholder;
        this.currentContents = currentContents;
        this.actions = actions;
    }

    public string name { get; }
    public string placeholder { get; }
    public float currentContents { get; }
    public UiPropertyActions<float> actions { get; }
}

public class UiBooleanPropertyInputAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.BooleanPropertyInput;

    public UiBooleanPropertyInputAtom(string name, bool currentContents, UiPropertyActions<bool> actions)
    {
        this.name = name;
        this.currentContents = currentContents;
        this.actions = actions;
    }

    public string name { get; }
    public bool currentContents { get; }
    public UiPropertyActions<bool> actions { get; }
}

public class UiVector3PropertyInputAtom : UiBuilderAtom
{
    public override AtomType type => AtomType.Vector3PropertyInput;

    public UiVector3PropertyInputAtom(string name, string[] placeholder, Vector3 currentContents, UiPropertyActions<Vector3> actions)
    {
        this.name = name;
        this.placeholder = placeholder;
        this.currentContents = currentContents;
        this.actions = actions;
    }

    public string name { get; }
    public string[] placeholder { get; }
    public Vector3 currentContents { get; }
    public UiPropertyActions<Vector3> actions { get; }
}