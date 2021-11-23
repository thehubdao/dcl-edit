using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void Save()
    {
        SceneSaveSystem.Save(); 
        ScriptGenerator.MakeScript();
    }
}
