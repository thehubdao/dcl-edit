using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Highlighter : Interface3DHover
{
    //Settings
    //The UV-Channel to store the smooth normals.
    //Must be the same UV-Channel as in the Shader.
    public const int UVChannel = 3;

    [SerializeField]
    private Highlight highlightPrefab = default;


    private bool highlightEnabled = false;
    private Highlight[] highlights;


    public override void StartHover()
    {
        if (!highlightEnabled)
        {
            Hilightable[] targets = GetComponentsInChildren<Hilightable>();
            highlights = new Highlight[targets.Length];
            InstantiateHighlights(targets);

            highlightEnabled = true;
        }
    }


    public override void EndHover()
    {
        if (highlightEnabled)
        {
            highlightEnabled = false;

            foreach (Highlight highlight in highlights)
            {
                highlight.DestroyHighlight();
            }
            
        }
    }


    private void InstantiateHighlights(Hilightable[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            highlights[i] = highlightPrefab.HighlightTarget(targets[i]);
        }
    }
}
