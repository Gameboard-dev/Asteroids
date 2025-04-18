using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AsteroidsMain.Content
{
    internal class Collision : AsteroidsMain
    {
        public static void Check(Asteroid ThisAsteroid)
        {
            if (ThisAsteroid.GlobalVertices == null) return;

            Console.WriteLine("Polygons : " + Polygons.Count);

            bool collisionDetected = false;

            foreach (Asteroid asteroid in Polygons)
            {
                    if(asteroid == ThisAsteroid || asteroid.GlobalVertices == null) continue;

                    bool isColliding = IsColliding(asteroid, ThisAsteroid);

                    if (isColliding)
                    {
                        Console.WriteLine("Collision Detected");
                        collisionDetected = true;
                        Bounce(asteroid, ThisAsteroid);
                    }

                }

            // DEBUG
            ThisAsteroid.AsteroidColor = collisionDetected ? Color.White : Color.Black;

        }

        private static void Bounce(Asteroid asteroidA, Asteroid asteroidB)
        {
            // Points from one asteroid's center to the other and is perpendicular to the tangent at the point of contact.
            Vector2 normal = asteroidB.Position - asteroidA.Position;
            normal.Normalize();

            // Determine the relative velocity between the two asteroids along the collision normal.
            Vector2 relativeVelocity = asteroidB.Trajectory - asteroidA.Trajectory;

            // Calculate velocity along the normal
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // If the asteroids are moving away from each other, no need to bounce
            if (velocityAlongNormal > 0)
            {
                return;
            }

            // Calculate restitution (bounciness)
            float restitution = 1.0f; // 1.0f for perfect bounce, less for less bounciness

            // Calculate impulse scalar
            float impulseScalar = -(1 + restitution) * velocityAlongNormal;
            impulseScalar /= 2; // Assuming equal mass. If masses are different, adjust accordingly.

            // Apply impulse to both asteroids
            Vector2 impulse = impulseScalar * normal;
            asteroidA.Trajectory -= impulse;
            asteroidB.Trajectory += impulse;
        }


        public static bool IsColliding(Asteroid asteroidA, Asteroid asteroidB)
        {
            Vector2[] axesA = GetAxes(asteroidA.GlobalVertices);
            Vector2[] axesB = GetAxes(asteroidB.GlobalVertices);

            // Check all axes of Polygon A
            for (int i = 0; i < axesA.Length; i++)
            {
                if (!IsOverlappingOnAxis(axesA[i], asteroidA, asteroidB))
                {
                    return false; // No collision
                }
            }

            // Check all axes of Polygon B
            for (int i = 0; i < axesB.Length; i++)
            {
                if (!IsOverlappingOnAxis(axesB[i], asteroidA, asteroidB))
                {
                    return false; // No collision
                }
            }

            // No separating axis found, collision detected
            return true;
        }


        private static Vector2[] GetAxes(List<Vector2> Vertices)
        {
            Vector2[] axes = new Vector2[Vertices.Count];
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector2 p1 = Vertices[i];
                Vector2 p2 = Vertices[(i + 1) % Vertices.Count];

                Vector2 edge = p2 - p1;
                axes[i] = new Vector2(-edge.Y, edge.X); // Perpendicular vector
                axes[i].Normalize();
            }
            return axes;
        }


        private static void ProjectAsteroid(Vector2 axis, Asteroid asteroid, out float min, out float max)
        {
            min = Vector2.Dot(axis, asteroid.GlobalVertices[0]);
            max = min;

            for (int i = 1; i < asteroid.GlobalVertices.Count; i++)
            {
                float projection = Vector2.Dot(axis, asteroid.GlobalVertices[i]);
                if (projection < min)
                {
                    min = projection;
                }
                if (projection > max)
                {
                    max = projection;
                }
            }
        }


        private static bool IsOverlappingOnAxis(Vector2 axis, Asteroid asteroidA, Asteroid asteroidB)
        {
            ProjectAsteroid(axis, asteroidA, out float minA, out float maxA);
            ProjectAsteroid(axis, asteroidB, out float minB, out float maxB);

            return !(maxA < minB || maxB < minA);
        }

    }
}
