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
    }

    private IEnumerable<Entity> _entities;

    private Dictionary<Entity, TransformWrap> _beginningTransformations = new Dictionary<Entity, TransformWrap>();
    private Dictionary<Entity, TransformWrap> _endingTransformations = new Dictionary<Entity, TransformWrap>();

    public TransformUndo(IEnumerable<Entity> entities)
    {
        _entities = entities;
    }

    public void SaveBeginningState()
    {
        foreach (var entity in _entities)
        {
            _beginningTransformations.Add(entity, new TransformWrap(entity.transform));
        }
    }

    public void SaveEndingState()
    {
        foreach (var entity in _entities)
        {
            _endingTransformations.Add(entity, new TransformWrap(entity.transform));
        }
    }

    public void AddUndoItem()
    {
        var name = _beginningTransformations.Count > 1
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