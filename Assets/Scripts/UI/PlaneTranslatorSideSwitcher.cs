using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTranslatorSideSwitcher : MonoBehaviour
{

    enum Plane
    {
        X,Y,Z
    }

    [SerializeField]
    private Plane _plane;
    
    [SerializeField]
    private float _offset = 0.35f;
    

    // Update is called once per frame
    void Update()
    {
        var relativeCamPos =
            transform.parent.InverseTransformPoint(CameraManager.Position);

        var newLocalPos = Vector3.zero;

        if (_plane != Plane.X)
            newLocalPos.x = relativeCamPos.x < 0 ? -_offset : _offset;

        if (_plane != Plane.Y)
            newLocalPos.y = relativeCamPos.y < 0 ? -_offset : _offset;

        if (_plane != Plane.Z)
            newLocalPos.z = relativeCamPos.z < 0 ? -_offset : _offset;

        transform.localPosition = newLocalPos;
    }
}
