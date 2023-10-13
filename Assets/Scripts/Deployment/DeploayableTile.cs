using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploayableTile : MonoBehaviour
{
    public int playerId;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private MeshRenderer mesh;

    public void Highlight(bool value)
    {
        if (value)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }
}
