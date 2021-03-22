using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Hexagonal
{
    public static class Offset
    {
        public static Vector3Int RToCube(Vector2Int h, OffsetCoord offset = OffsetCoord.Odd)
        {
            int q = h.x - (int)((h.y + (int)offset * (h.y & 1)) / 2);
            int r = h.y;
            int s = -q - r;

            return new Vector3Int(q, r, s);
        }

        public static Vector3Int QToCube(Vector2Int h, OffsetCoord offset = OffsetCoord.Odd)
        {
            int q = h.x;
            int r = h.y - (int)((h.x + (int)offset * (h.x & 1)) / 2);
            int s = -q - r;

            return new Vector3Int(q, r, s);
        }

        // q=x r=y
        public static Vector2Int RFromCube(Vector3Int h, OffsetCoord offset = OffsetCoord.Odd)
        {
            int col = h.x + (int)((h.y + (int)offset * (h.y & 1)) / 2);
            int row = h.y;

            return new Vector2Int(col, row);
        }

        public static Vector2Int QFromCube(Vector3Int h, OffsetCoord offset = OffsetCoord.Odd)
        {
            int col = h.x;
            int row = h.y + (int)((h.x + (int)offset * (h.x & 1)) / 2);

            return new Vector2Int(col, row);
        }
    }

    public static class Axial
    {
        public static Vector3 ToCube(Vector2 hex)
        {
            var x = hex.x;
            var z = hex.y;
            var y = -x - z;
            return new Vector3(x, y, z);
        }
    }

    public static class Cube
    {
        public static List<Vector3Int> directions = new List<Vector3Int> {
            new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
            new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1)
        };
        public static List<Vector3Int> diagonals = new List<Vector3Int> {
            new Vector3Int(2, -1, -1), new Vector3Int(1, -2, 1), new Vector3Int(-1, -1, 2),
            new Vector3Int(-2, 1, 1), new Vector3Int(-1, 2, -1), new Vector3Int(1, 1, -2)
        };

        private static Matrix4x4 OrientationFlat = new Matrix4x4(
                new Vector4(3f / 2, Mathf.Sqrt(3f) / 2),
                new Vector4(0, Mathf.Sqrt(3f)),
                Vector4.zero,
                Vector4.zero
            );

        private static Matrix4x4 OrientationPointy = new Matrix4x4(
                new Vector4(Mathf.Sqrt(3f), 0),
                new Vector4(Mathf.Sqrt(3f) / 2, 3f / 2),
                Vector4.zero,
                Vector4.zero
            );

        public static IEnumerable<Vector3Int> GetNeighbour(Vector3Int index)
            => directions.Select(v => index + v);

        public static IEnumerable<Vector3Int> GetDiagonals(Vector3Int index)
            => diagonals.Select(v => index + v);

        public static int Length(Vector3Int index)
            => (Mathf.Abs(index.x) + Mathf.Abs(index.y) + Mathf.Abs(index.z)) / 2;

        public static int Distance(Vector3Int start, Vector3Int end)
            => Length(end - start);

        public static Vector3 HexToPixel(Vector3Int index, Vector2 size)
        {
            var t = OrientationFlat * (Vector3)index;
            t.Scale(size);

            return new Vector3(t.x, 0, t.y);
        }

        public static Vector3Int PixelToHex(Vector3 localPosition, Vector2 size)
        {
            float sqrt3 = Mathf.Sqrt(3f);

            //TODO: OrientationFlat.inverse == матрице ниже?

            Matrix4x4 OrientationFlat = new Matrix4x4(
                new Vector4(2f / 3, -1f / 3),
                new Vector4(0, sqrt3 / 3),
                Vector4.zero,
                Vector4.zero
            );
            /*
            Matrix4x4 OrientationPointy = new Matrix4x4(
                new Vector4(sqrt3/3, 0),
                new Vector4(-1f/3, 2f/3),
                Vector4.zero,
                Vector4.zero
            );
            */

            var pos = new Vector3(localPosition.x / size.x, -localPosition.z / size.y, 0);
            var hex = OrientationFlat * pos;

            return HexRound(Axial.ToCube(hex));
        }

        //https://www.redblobgames.com/grids/hexagons/codegen/output/lib.cs
        private static Vector3Int HexRound(Vector3 hex)
        {
            int qi = (int)(Mathf.Round(hex.x));
            int ri = (int)(Mathf.Round(hex.y));
            int si = (int)(Mathf.Round(hex.z));
            double q_diff = Mathf.Abs(qi - hex.x);
            double r_diff = Mathf.Abs(ri - hex.y);
            double s_diff = Mathf.Abs(si - hex.z);

            if (q_diff > r_diff && q_diff > s_diff)
                qi = -ri - si;
            else if (r_diff > s_diff)
                ri = -qi - si;
            else
                si = -qi - ri;
            return new Vector3Int(qi, ri, si);
        }

    }


}
