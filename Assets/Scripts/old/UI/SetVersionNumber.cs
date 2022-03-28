using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetVersionNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _versionNumberText;

    
    // Start is called before the first frame update
    void Start()
    {
        _versionNumberText.text = Application.version;
    }
}
