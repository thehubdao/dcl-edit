using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListAvailableComponentsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _listItemTemplate;

    void OnEnable()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        var entity = SceneManager.SelectedEntity;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var c in ComponentRepresentationList.AllComponentTypes)
        {
            var componentName = c.Key;
            var componentType = c.Value;

            if (entity.gameObject.TryGetComponent(componentType, out var _))
            {
                continue;
            }

            var newItemObject = Instantiate(_listItemTemplate, transform);
            var newListItem = newItemObject.GetComponent<ComponentListItem>();
            newListItem.componentName = componentName;
            newListItem.componentType = componentType;
            newListItem.UpdateVisuals();
        }
    }
}
