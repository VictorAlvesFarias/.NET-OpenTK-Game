using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Extensions;

namespace Kingdom_of_Creation.Physics
{
    public class Physics
    {
        public Dictionary<Guid,Action<RenderObject, CollisionManifold>> ColisionEvents { get; set; }

        private readonly float _positionTolerance;
        private readonly float _velocityTolerance;

        public Physics(float positionTolerance = 0.001f, float velocityTolerance = 0.001f) 
        {
            ColisionEvents = new Dictionary<Guid, Action<RenderObject, CollisionManifold>>();
            _positionTolerance = positionTolerance;
            _velocityTolerance = velocityTolerance;
        }

        public bool CheckCollision(RenderObject objA, RenderObject objB)
        {
            var vertsA = objA.GetVerticesList();
            var vertsB = objB.GetVerticesList();

            foreach (var axis in vertsA.GetAxes().Concat(vertsB.GetAxes()))
            {
                var (minA, maxA) = vertsA.Project(axis);
                var (minB, maxB) = vertsB.Project(axis);

                if (maxA < minB || maxB < minA)
                {
                    return false; 
                }
            }

            return true; 
        }
        public CollisionManifold CheckCollisionManifold(RenderObject objA, RenderObject objB, Vector_2 relativePositionObjA)
        {
            var vertsA = objA.GetVerticesList(relativePositionObjA);
            var vertsB = objB.GetVerticesList();

            float minPenetration = float.MaxValue;
            Vector_2 smallestAxis = new Vector_2();

            foreach (var axis in vertsA.GetAxes().Concat(vertsB.GetAxes()))
            {
                var (minA, maxA) = vertsA.Project(axis);
                var (minB, maxB) = vertsB.Project(axis);

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

            var direction = objB.Position - relativePositionObjA;
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
            var position = renderObject.Position;
            var applyGravity = true;
            var futurePosX = position + new Vector_2(velocity.X * deltaTime, 0);

            position = futurePosX;

            foreach (var other in allObjects)
            {
                if (other.Id == renderObject.Id) continue;

                var manifold = CheckCollisionManifold(renderObject, other, position);

                if (manifold.HasCollision && !renderObject.Static)
                {
                    position -= manifold.Normal * manifold.PenetrationDepth;

                    if (MathF.Abs(manifold.Normal.X) > 0.5f)
                        velocity.X = 0;
                }

                ColisionEvents.GetValueOrDefault(renderObject.Id)?.Invoke(other, manifold);
            }

            var futurePosY = position + new Vector_2(0, velocity.Y * deltaTime);

            position = futurePosY;

            foreach (var other in allObjects)
            {
                if (other.Id == renderObject.Id) continue;

                var manifold = CheckCollisionManifold(renderObject, other, position);

                if (manifold.HasCollision && !renderObject.Static)
                {
                    position -= manifold.Normal * manifold.PenetrationDepth;

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

            ApplyPositionWithTolerance(renderObject, position);
            ApplyVelocityWithTolerance(renderObject, velocity);
        }
        public void ApplyVelocity(RenderObject renderObject, Vector_2 newVelocity)
        {
            ApplyVelocityWithTolerance(renderObject, newVelocity);
        }

        private void ApplyPositionWithTolerance(RenderObject renderObject, Vector_2 newPosition)
        {
            if (IsPositionChangeSignificant(renderObject.Position, newPosition))
            {
                renderObject.Position = newPosition;
            }
            else
            {
                renderObject.UpdatePositionInternal(newPosition);
            }
        }

        private void ApplyVelocityWithTolerance(RenderObject renderObject, Vector_2 newVelocity)
        {
            if (IsVelocityChangeSignificant(renderObject.Velocity, newVelocity))
            {
                renderObject.Velocity = newVelocity;
            }
            else
            {
                renderObject.UpdateVelocityInternal(newVelocity);
            }
        }

        private bool IsPositionChangeSignificant(Vector_2 oldPosition, Vector_2 newPosition)
        {
            float deltaX = MathF.Abs(newPosition.X - oldPosition.X);
            float deltaY = MathF.Abs(newPosition.Y - oldPosition.Y);
            
            return deltaX >= _positionTolerance || deltaY >= _positionTolerance;
        }

        private bool IsVelocityChangeSignificant(Vector_2 oldVelocity, Vector_2 newVelocity)
        {
            float deltaX = MathF.Abs(newVelocity.X - oldVelocity.X);
            float deltaY = MathF.Abs(newVelocity.Y - oldVelocity.Y);
            
            return deltaX >= _velocityTolerance || deltaY >= _velocityTolerance;
        }
    }
}