using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.RenderObjectService.Factories;

namespace Kingdom_of_Creation.Physics
{
    public class Physics
    {
        private readonly RenderObjectServiceFactory _renderObjectFactory;
        public Dictionary<Guid,Action<RenderObject, CollisionManifold>> ColisionEvents { get; set; }

        public Physics() {
            _renderObjectFactory = new RenderObjectServiceFactory();
            ColisionEvents = new Dictionary<Guid, Action<RenderObject, CollisionManifold>>();
        }

        public bool CheckCollision(RenderObject objA, RenderObject objB)
        {
            var vertsA = _renderObjectFactory.GetRenderService(objA).GetVerticesList(objA);
            var vertsB = _renderObjectFactory.GetRenderService(objB).GetVerticesList(objB);

            foreach (var axis in _renderObjectFactory.GetRenderService(objA).GetAxes(vertsA).Concat(_renderObjectFactory.GetRenderService(objB).GetAxes(vertsB)))
            {
                var (minA, maxA) = _renderObjectFactory.GetRenderService(objB).Project(vertsA, axis);
                var (minB, maxB) = _renderObjectFactory.GetRenderService(objB).Project(vertsB, axis);

                if (maxA < minB || maxB < minA)
                {
                    return false; // Separação encontrada → sem colisão
                }
            }

            return true; // nenhuma separação → colisão
        }
        public CollisionManifold CheckCollisionManifold(RenderObject objA, RenderObject objB)
        {
            var vertsA = _renderObjectFactory.GetRenderService(objA).GetVerticesList(objA);
            var vertsB = _renderObjectFactory.GetRenderService(objB).GetVerticesList(objB);

            float minPenetration = float.MaxValue;
            Vector_2 smallestAxis = new Vector_2();

            foreach (var axis in _renderObjectFactory.GetRenderService(objA).GetAxes(vertsA).Concat(_renderObjectFactory.GetRenderService(objB).GetAxes(vertsB)))
            {
                var (minA, maxA) = _renderObjectFactory.GetRenderService(objA).Project(vertsA, axis);
                var (minB, maxB) = _renderObjectFactory.GetRenderService(objB).Project(vertsB, axis);

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
            if (direction.Dot(smallestAxis) < 0)
                smallestAxis = new Vector_2(-smallestAxis.X, -smallestAxis.Y);

            return new CollisionManifold
            {
                HasCollision = true,
                Normal = smallestAxis,
                PenetrationDepth = minPenetration
            };
        }
        public void ResolveColision(RenderObject renderObject, float deltaTime, IEnumerable<RenderObject> allObjects, float gravity)
        {
            var velocity = renderObject.Velocity;
            var applyGravity = true;
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

                ColisionEvents.GetValueOrDefault(renderObject.Id)?.Invoke(other, manifold);
            }

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

                ColisionEvents.GetValueOrDefault(renderObject.Id)?.Invoke(other, manifold);
            }

            if (applyGravity && !renderObject.Static)
                velocity.Y += gravity * deltaTime;

            renderObject.Velocity = velocity;
        }
        public void ApplyVelocity(RenderObject renderObject, Vector_2 newVelocity)
        {
            renderObject.Velocity = newVelocity;
        }
    }
}