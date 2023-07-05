using System;
using UnityEngine;

namespace RedBjorn.ProtoTiles
{
    [Serializable]
    public class TileData
    {
        public Vector3Int TilePos;
        [Obsolete]
        public string Id;
        public int MovableArea;
        public float[] SideHeight = new float[6] { 0f, 0f, 0f, 0f, 0f, 0f };
    }
}