using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities;

namespace Server
{
    public static class Physics
    {
        public static bool CheckCollision(Vector_2 pos1, Vector_2 size1, Vector_2 pos2, Vector_2 size2)
        {
            return pos1.X < pos2.X + size2.X &&
                   pos1.X + size1.X > pos2.X &&
                   pos1.Y < pos2.Y + size2.Y &&
                   pos1.Y + size1.Y > pos2.Y;
        }

        public static void ResolveColision(Vector_2 position, Player player, float deltaTime, List<Rectangle> _mapObjects)
        {
            var futurePosition = player.Position + (player.Velocity * deltaTime);

            bool collisionX = false;
            bool collisionY = false;

            player.IsGrounded = false;

            foreach (var mapObject in _mapObjects)
            {
                var mapObjectPos = mapObject.Position;
                var mapObjectSize = mapObject.Size;
                var checkY = new Vector_2(player.Position.X, futurePosition.Y);

                if (Physics.CheckCollision(checkY, player.Size, mapObjectPos, mapObjectSize))
                {
                    collisionY = true;

                    if (player.Velocity.Y > 0)
                    {
                        futurePosition.Y = mapObjectPos.Y - player.Size.Y;
                    }
                    else
                    {
                        futurePosition.Y = mapObjectPos.Y + mapObjectSize.Y;
                        player.IsGrounded = true;
                    }
                    player.Velocity = new Vector_2(player.Velocity.X, 0);
                }

                var checkX = new Vector_2(futurePosition.X, player.Position.Y);

                if (Physics.CheckCollision(checkX, player.Size, mapObjectPos, mapObjectSize))
                {
                    collisionX = true;

                    if (player.Velocity.X > 0)
                    {
                        futurePosition.X = mapObjectPos.X - player.Size.X;
                    }
                    else
                    {
                        futurePosition.X = mapObjectPos.X + mapObjectSize.X;
                    }
                    player.Velocity = new Vector_2(0, player.Velocity.Y);
                }
            }

            player.Position = futurePosition;
        }
    }
}
