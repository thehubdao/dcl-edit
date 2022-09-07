using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DclPropertyIdentifier
{
    private Guid _entity;
    private string _component;
    private string _property;

    public Guid Entity => _entity;
    public string Component => _component;
    public string Property => _property;

    public DclPropertyIdentifier(Guid entity, string component, string property)
    {
        _entity = entity;
        _component = component;
        _property = property;
    }
}
