using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Interface3DManager : MonoBehaviour
{
    public Camera gizmoCamera;

    private Material _lastHovered = null;

    private EntityManipulator _activeManipulator;
    private Vector3? _lastMousePosition = null;
    private Plane _activeManipulatorPlane;

    // Update is called once per frame
    void Update()
    {
        var mouseRay = gizmoCamera.ViewportPointToRay(gizmoCamera.ScreenToViewportPoint(Input.mousePosition));
        Material hitMaterial = null;
        EntityManipulator hoveredManipulator = null;
        
        if (_activeManipulator != null)
        {
            hitMaterial = _activeManipulator.transform.GetComponent<MeshRenderer>().material;
        }
        else
        {
            if (!Input.GetMouseButton((int) MouseButton.RightMouse)&&Physics.Raycast(mouseRay, out RaycastHit hitInfo, 10000, LayerMask.GetMask("Gizmos")))
            {
                //Debug.Log("Hovering "+hitInfo.transform.gameObject);
                hitMaterial = hitInfo.transform.GetComponent<MeshRenderer>().material;
                hoveredManipulator = hitInfo.transform.GetComponent<EntityManipulator>();
            }
        }

        
        if (hitMaterial != _lastHovered)
        {
            hitMaterial?.SetFloat("hover", 1);
            _lastHovered?.SetFloat("hover", 0);
            _lastHovered = hitMaterial;
        }

        //Debug.Log("Manipulator: "+hoveredManipulator);
        if (hoveredManipulator != null && Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            _activeManipulator = hoveredManipulator;
            //_activeManipulatorDistance = Vector3.Distance(
            //            _activeManipulator.transform.position,
            //            gizmoCamera.transform.position);
            _activeManipulatorPlane = _activeManipulator.GetPlane(gizmoCamera);
        }

        if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse))
        {
            _activeManipulator = null;
        }


        if (_activeManipulator != null)
        {
            //// Distance from the camera to the active manipulator
            //var manipulatorDistance = _activeManipulatorDistance;
            //
            //// Distance from the camera to where the mouse points on a plane
            //var planeDistance = manipulatorDistance / Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(gizmoCamera.transform.forward, mouseRay.direction));
            //
            //// Point on the plane
            //var mousePositionOnPlane = mouseRay.GetPoint(planeDistance);
            //_lastMousePosition ??= mousePositionOnPlane;
            //
            //var mouseChange = mousePositionOnPlane - _lastMousePosition.Value;
            ////Debug.Log(currentMousePosition);
            //_activeManipulator?.Change(mouseChange);
            //
            //Debug.DrawLine(mousePositionOnPlane, _lastMousePosition.Value, Color.red, 10);
            //
            //_lastMousePosition = mousePositionOnPlane;

            _activeManipulatorPlane.Raycast(mouseRay,out var distanceOnPlane);
            var mousePositionOnPlane = mouseRay.GetPoint(distanceOnPlane);
            
            _lastMousePosition ??= mousePositionOnPlane;

            //Debug.DrawLine(mousePositionOnPlane, _lastMousePosition.Value, Color.red, 10);

            //_activeManipulatorPlane.DrawGizmo(_activeManipulator.GetOneRay());


            var mouseChange = mousePositionOnPlane - _lastMousePosition.Value;
            _activeManipulator.Change(mouseChange);
            
            _lastMousePosition = mousePositionOnPlane;
        }
        else
        {
            _lastMousePosition = null;
        }
    }
}

static class Util
{
    public static Vector3 randomVector3()
    {

        return new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    public static void DrawGizmo(this Plane p,Ray? startRay = null,int lines = 21,float spacing=1)
    {
        startRay ??= new Ray(Vector3.zero, Vector3.up);

        var startPoint = startRay.Value.origin;

        // Put startpoint on plane
        var startPointOnPlane = p.ClosestPointOnPlane(startPoint);

        // Move startpoint to left border
        var horizontalRay = new Ray(startPointOnPlane, Vector3.Cross(p.normal, startRay.Value.direction));
        startPointOnPlane = horizontalRay.GetPoint(-(lines-1) * spacing / 2);

        // Move startpoint to left bottom corner
        var verticalRay = new Ray(startPointOnPlane, Vector3.Cross(p.normal, horizontalRay.direction));

        // Draw Horizontal lines
        for (int i = 0; i < lines; i++)
        {
            var rayStartPoint = verticalRay.GetPoint((-(lines-1) * spacing / 2)+i*spacing);
            Debug.DrawRay(rayStartPoint,horizontalRay.direction.normalized*(lines-1) * spacing);
        }

        horizontalRay.origin = verticalRay.GetPoint(-(lines - 1) * spacing / 2);

        // Draw Vertical lines
        for (int i = 0; i < lines; i++)
        {
            var rayStartPoint = horizontalRay.GetPoint(/*(-(lines-1) * (-spacing/4))+*/i*spacing);
            Debug.DrawRay(rayStartPoint,verticalRay.direction.normalized*(lines-1) * spacing);
        }
        
        //Debug.DrawRay(p.normal*-p.distance,Vector3.Cross(p.normal,Util.randomVector3()),Color.green,10);
    }
}