using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Events;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

/*
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

public UiTitleAtom(string title)
public UiTextAtom(string text)
public UiSpacerAtom(float height)
public UiPanelAtom(string contentKey)
public UiPanelHeaderAtom(string title, [CanBeNull] Action onClose)
public UiHierarchyItemAtom(string name, int level, bool hasChildren, bool isExpanded, TextStyle textStyle, Action onArrowClick, Action onNameClick)
public UiStringPropertyInputAtom(string name, string placeholder, string currentContents, UiPropertyActions<string> actions)
public UiNumberPropertyInputAtom(string name, string placeholder, float currentContents, UiPropertyActions<float> actions)
public UiBooleanPropertyInputAtom(string name, bool currentContents, UiPropertyActions<bool> actions)
public UiVector3PropertyInputAtom(string name, string[] placeholder, Vector3 currentContents, UiPropertyActions<Vector3> actions)
*/


public class UiBuilderSystem
{
    public class UiBuilder
    {
        // Dependencies
        private UiBuilderState _builderState;

        [Inject]
        public void Construct(UiBuilderState builderState)
        {
            this._builderState = builderState;
        }

        public UiBuilder(EventDependentTypes.UiBuilderSetupKey key)
        {
            this.key = key;
        }

        public EventDependentTypes.UiBuilderSetupKey key { get; }

        private List<UiBuilderAtom> _atoms = new List<UiBuilderAtom>();

        public UiBuilder Title(string title)
        {
            _atoms.Add(new UiTitleAtom(title));

            return this;
        }

        public UiBuilder Text(string text)
        {
            _atoms.Add(new UiTextAtom(text));

            return this;
        }

        public UiBuilder Spacer(float height)
        {
            _atoms.Add(new UiSpacerAtom(height));

            return this;
        }

        public UiBuilder Panel(EventDependentTypes.UiBuilderSetupKey contentKey)
        {
            _atoms.Add(new UiPanelAtom(contentKey));

            return this;
        }

        public UiBuilder PanelHeader(string title, [CanBeNull] Action onClose)
        {
            _atoms.Add(new UiPanelHeaderAtom(title, onClose));

            return this;
        }

        public UiBuilder HierarchyItem(string name, int level, bool hasChildren, bool isExpanded, UiBuilderAtom.TextStyle textStyle, Action onArrowClick, Action onNameClick)
        {
            _atoms.Add(new UiHierarchyItemAtom(name, level, hasChildren, isExpanded, textStyle, onArrowClick, onNameClick));

            return this;
        }

        public UiBuilder StringPropertyInput(string name, string placeholder, string currentContents, UiBuilderAtom.UiPropertyActions<string> actions)
        {
            _atoms.Add(new UiStringPropertyInputAtom(name, placeholder, currentContents, actions));

            return this;
        }

        public UiBuilder NumberPropertyInput(string name, string placeholder, float currentContents, UiBuilderAtom.UiPropertyActions<float> actions)
        {
            _atoms.Add(new UiNumberPropertyInputAtom(name, placeholder, currentContents, actions));

            return this;
        }

        public UiBuilder BooleanPropertyInput(string name, bool currentContents, UiBuilderAtom.UiPropertyActions<bool> actions)
        {
            _atoms.Add(new UiBooleanPropertyInputAtom(name, currentContents, actions));

            return this;
        }

        public UiBuilder Vector3PropertyInput(string name, string[] placeholder, Vector3 currentContents, UiBuilderAtom.UiPropertyActions<Vector3> actions)
        {
            _atoms.Add(new UiVector3PropertyInputAtom(name, placeholder, currentContents, actions));

            return this;
        }


        public void Done()
        {
            _builderState.AddBuilderSetup(key, _atoms);
        }

        public class Factory : PlaceholderFactory<EventDependentTypes.UiBuilderSetupKey, UiBuilder>
        {
        }
    }
}
