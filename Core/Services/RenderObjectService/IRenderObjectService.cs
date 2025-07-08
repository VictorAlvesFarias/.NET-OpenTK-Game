using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Services.EntityGeometry
{
    public interface IRenderObjectService
    {
        float[] GetVertices(RenderObject renderObject);
        IEnumerable<Vector_2> GetAxes(List<Vector_2> vertices);
        PrimitiveType GetPrimitiveType();
        List<Vector_2> GetVerticesList(RenderObject renderObject);
        (float min, float max) Project(List<Vector_2> vertices, Vector_2 axis);
    }
}
