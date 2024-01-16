using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RedBjorn.ProtoTiles;

public class AccessibleTile : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Color affectedColor;
    private Material defaultMaterial;
    private Color defaultColor;
    private Tween anim;
    private TileEntity entity;

    private void Awake()
    {
        defaultColor = mesh.material.color;
    }

    public void Setup(TileEntity entity)
    {
        this.entity = entity;
    }

    public void PlayAnimation()
    {
        StopAnimation();
        anim = mesh.material.DOColor(affectedColor, 0.3f).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimation()
    {
        anim?.Kill();
        mesh.material.color = defaultColor;
    }

    public TileEntity GetEntity()
    {
        return entity;
    }
}
