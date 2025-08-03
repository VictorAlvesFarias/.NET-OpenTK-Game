using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;

namespace Client.Extensions
{
    public static class RectangleExtensions
    {
        public static void CalculatePlatformGeometry(this RenderObject rect,Vector_2 startWorldPos, Vector_2 currentWorldPos)
        {
            Vector_2 size = new Vector_2(
                Math.Abs(currentWorldPos.X - startWorldPos.X),
                Math.Abs(currentWorldPos.Y - startWorldPos.Y)
            );

            size.X = Math.Max(size.X, 0.01f);
            size.Y = Math.Max(size.Y, 0.01f);

            Vector_2 position = new Vector_2(
                Math.Min(startWorldPos.X, currentWorldPos.X),
                Math.Min(startWorldPos.Y, currentWorldPos.Y)
            );

            rect.Position = position;
            rect.Size = size;
        }
    }
}
