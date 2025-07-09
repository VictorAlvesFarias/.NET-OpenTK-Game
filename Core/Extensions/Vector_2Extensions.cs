using Kingdom_of_Creation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Extensions
{
    public static class Vector_2Extensions
    {
        public static IEnumerable<Vector_2> GetAxes(this List<Vector_2> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var p1 = vertices[i];
                var p2 = vertices[(i + 1) % vertices.Count];
                var edge = new Vector_2(p2.X - p1.X, p2.Y - p1.Y);
                var normal = new Vector_2(-edge.Y, edge.X);
                var length = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
                yield return new Vector_2(normal.X / length, normal.Y / length);
            }
        }
        public static (float min, float max) Project(this List<Vector_2> vertices, Vector_2 vert)
        {
            float min = vertices[0].Dot(vert);
            float max = min;

            foreach (var v in vertices)
            {
                float proj = v.Dot(vert);
                if (proj < min) min = proj;
                if (proj > max) max = proj;
            }
            return (min, max);
        }
    }
}
