using UnityEngine;
using UnityEngine.Tilemaps;

namespace GamePackages.Core
{
    public static class TilemapWrapper
    {
        public static Bounds GetWorldBounds(this Tilemap tilemap)
        {
            Vector3 cellSize = tilemap.cellSize;
            Vector3 size = tilemap.cellBounds.size;
            size.x *= cellSize.x;
            size.y *= cellSize.y;
            size.z *= cellSize.z;

            Vector3 center = tilemap.transform.position;
            Vector3 offset = tilemap.cellBounds.center;
            offset.x *= cellSize.x;
            offset.y *= cellSize.y;
            offset.z *= cellSize.z;

            return new Bounds(center + offset, size);
        }
    }
}
