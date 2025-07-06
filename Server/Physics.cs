using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.RenderObjectService.Factories;

public static class Physics
{
    public readonly static RenderObjectFactory _renderObjectFactory = new RenderObjectFactory();

    public static bool CheckCollision(RenderObject objA, RenderObject objB)
    {
        var renderObjectService = _renderObjectFactory.GetRenderService(objA);
        var vertsA = renderObjectService.GetVerticesList(objA);
        var vertsB = renderObjectService.GetVerticesList(objB);

        foreach (var axis in GetAxes(vertsA).Concat(GetAxes(vertsB)))
        {
            var (minA, maxA) = Project(vertsA, axis);
            var (minB, maxB) = Project(vertsB, axis);

            if (maxA < minB || maxB < minA)
            {
                return false; // Separação encontrada → sem colisão
            }
        }

        return true; // nenhuma separação → colisão
    }
    private static IEnumerable<Vector_2> GetAxes(List<Vector_2> vertices)
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
    private static (float min, float max) Project(List<Vector_2> vertices, Vector_2 axis)
    {
        float min = Dot(vertices[0], axis);
        float max = min;

        foreach (var v in vertices)
        {
            float proj = Dot(v, axis);
            if (proj < min) min = proj;
            if (proj > max) max = proj;
        }
        return (min, max);
    }
    private static float Dot(Vector_2 a, Vector_2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    public static CollisionManifold CheckCollisionManifold(RenderObject objA, RenderObject objB)
    {
        var vertsA = _renderObjectFactory.GetRenderService(objA).GetVerticesList(objA);
        var vertsB = _renderObjectFactory.GetRenderService(objB).GetVerticesList(objB);

        float minPenetration = float.MaxValue;
        Vector_2 smallestAxis = new Vector_2();

        foreach (var axis in GetAxes(vertsA).Concat(GetAxes(vertsB)))
        {
            var (minA, maxA) = Project(vertsA, axis);
            var (minB, maxB) = Project(vertsB, axis);

            if (maxA < minB || maxB < minA)
            {
                return new CollisionManifold { HasCollision = false, Normal = smallestAxis };
            }

            float overlap = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
            if (overlap < minPenetration)
            {
                minPenetration = overlap;
                smallestAxis = axis;
            }
        }

        // Direção do vetor normal tem que apontar para fora do objA
        var direction = objB.Position - objA.Position;
        if (Dot(direction, smallestAxis) < 0)
            smallestAxis = new Vector_2(-smallestAxis.X, -smallestAxis.Y);

        return new CollisionManifold
        {
            HasCollision = true,
            Normal = smallestAxis,
            PenetrationDepth = minPenetration
        };
    }
    public static void ResolveColision(RenderObject renderObject, float deltaTime, IEnumerable<RenderObject> allObjects, float gravity)
    {
        var velocity = renderObject.Velocity;
        var applyGravity = true;

        // Primeiro tenta mover no eixo X
        var futurePosX = renderObject.Position + new Vector_2(velocity.X * deltaTime, 0);
        renderObject.Position = futurePosX;

        foreach (var other in allObjects)
        {
            if (other.Id == renderObject.Id) continue;

            var manifold = CheckCollisionManifold(renderObject, other);

            if (manifold.HasCollision && !renderObject.Static)
            {
                renderObject.Position -= manifold.Normal * manifold.PenetrationDepth;

                if (MathF.Abs(manifold.Normal.X) > 0.5f)
                    velocity.X = 0;
            }

            renderObject.ColisionEventAction.Invoke(other, manifold);
        }

        // Depois tenta mover no eixo Y
        var futurePosY = renderObject.Position + new Vector_2(0, velocity.Y * deltaTime);
        renderObject.Position = futurePosY;


        foreach (var other in allObjects)
        {
            if (other.Id == renderObject.Id) continue;

            var manifold = CheckCollisionManifold(renderObject, other);

            if (manifold.HasCollision && !renderObject.Static)
            {
                renderObject.Position -= manifold.Normal * manifold.PenetrationDepth;

                if (manifold.Normal.Y > 0.5f || manifold.Normal.Y < -0.5f)
                {
                    velocity.Y = 0;
                    applyGravity = false;
                }
            }

            renderObject.ColisionEventAction.Invoke(other, manifold);
        }

        if (applyGravity && !renderObject.Static)
            velocity.Y += gravity * deltaTime;

        renderObject.Velocity = velocity;
    }
}
