using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Highlight : MonoBehaviour
{
	//Settings
	[SerializeField]
	private List<MeshFilter> meshFilters = default;
	[SerializeField]


	public Highlight HighlightTarget(Hilightable target)
	{
		Highlight instance = Instantiate(gameObject, target.transform).GetComponent<Highlight>();

		Mesh targetMesh = target.OwnMeshFilter.sharedMesh;
		foreach (MeshFilter meshFilter in instance.meshFilters)
		{
			meshFilter.mesh = targetMesh;
		}

		return instance;
	}


	public void DestroyHighlight()
	{
		if(gameObject!=null && gameObject)
		    Destroy(gameObject);
	}
}
