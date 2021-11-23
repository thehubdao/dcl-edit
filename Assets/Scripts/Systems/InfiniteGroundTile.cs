using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;

public class InfiniteGroundTile : MonoBehaviour
{
    public GameObject DefaultGrass;


    private bool _showDefaultGrass = true;

    public bool ShowDefaultGrass
    {
        get => _showDefaultGrass;
        set
        {
            _showDefaultGrass = value;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        DefaultGrass.SetActive(ShowDefaultGrass);
    }
    

}
