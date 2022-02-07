using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformUndo
{
    private struct TransformWrap
    {
        private Vector3 pos, scale;
        private Quaternion rot;

        public TransformWrap(Transform t)
        {
            pos = t.position;
            scale = t.localScale;
            rot = t.rotation;
        }

        public void ApplyTo(Transform t)
        {
            t.position = pos;
            t.localScale = scale;
            t.rotation = rot;
        }

        public override bool Equals(object obj)
        {
            if (obj is TransformWrap other)
            { 
                return pos == other.pos && 
                       scale == other.scale && 
                       rot == other.rot;
            }

            return false;
        }

        public override int GetHashCode() // Overriding to shut up the compiler
        {
            return base.GetHashCode();
        }
    }

    private IEnumerable<Entity> _entities;

    private Dictionary<Entity, TransformWrap> _beginningTransformations = null;
    private Dictionary<Entity, TransformWrap> _endingTransformations = null;

    public TransformUndo(IEnumerable<Entity> entities)
    {
        _entities = entities;
    }

    public void SaveBeginningState()
    {
        _beginningTransformations = new Dictionary<Entity, TransformWrap>();
        foreach (var entity in _entities)
        {
            _beginningTransformations.Add(entity, new TransformWrap(entity.transform));
        }
    }

    public void SaveEndingState()
    {
        _endingTransformations = new Dictionary<Entity, TransformWrap>();
        foreach (var entity in _entities)
        {
            _endingTransformations.Add(entity, new TransformWrap(entity.transform));
        }
    }

    public bool HasChanged()
    {
        if (_beginningTransformations == null || _endingTransformations == null)
            throw new Exception("Beginning and end state has to be saved");

        return _beginningTransformations.Any(pair => !pair.Value.Equals(_endingTransformations[pair.Key]));
    }

    public void AddUndoItem()
    {
        if(!HasChanged())
        {
            Debug.Log("not Changed");
            
            return;
        }

        var name = "Transforming ";
        name += _beginningTransformations.Count > 1
            ? "Entities"
            : _beginningTransformations.First().Key.TryGetShownName();

        UndoManager.RecordUndoItem(
            name,
            () =>
            {
                foreach (var pair in _beginningTransformations)
                {
                    pair.Value.ApplyTo(pair.Key.transform);
                    SceneManager.OnUpdateSelection.Invoke();
                }
            },
            () =>
            {
                foreach (var pair in _endingTransformations)
                {
                    pair.Value.ApplyTo(pair.Key.transform);
                    SceneManager.OnUpdateSelection.Invoke();
                }
            });
    }
}