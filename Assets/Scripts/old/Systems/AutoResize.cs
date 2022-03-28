using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoResize : MonoBehaviour
{
    void Start()
    {
        //gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        var distance = Vector3.Distance(DclSceneManager.GizmoCamera.transform.position, transform.position);
        distance *= GizmoSizeManager.GizmoScale;
        transform.localScale = new Vector3(distance, distance, distance);
    }
}
