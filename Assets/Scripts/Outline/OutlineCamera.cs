using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
    public static OutlineCamera Instance;

    void Start()
    {
        Instance = this;
    }

    private float count = 0;

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
        {
            count += 0.6f;
        }

        if (count > 0)
        {
            count -= Time.deltaTime;
        }

        if (count > 10)
        {
            Wiggle = !Wiggle;
            count = 0;
        }
    }

    public Camera Camera => GetComponent<Camera>();

    public bool Wiggle { get; private set; } = false;
}
