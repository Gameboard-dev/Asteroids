using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsteroidsMain.Content
{
    public class Asteroid
    {

        private Texture2D PolygonTextureMap;
        private float PolygonMaxRadius;

        private Vector2 TextureCentre;
        private int TextureWidth;
        private float LineThickness;
        private float RotationDegrees = 0f;
        private List<Vector2> LocalVertices;
        private static Texture2D debugTexture;

        public Vector2 Trajectory;
        public float Spin;
        public float TextureScale = 1f;
        public List<Vector2> GlobalVertices;
        public Color AsteroidColor;
        public Vector2 Position;


        internal Asteroid(float radius, float thickness, Vector2 position, Color color, Vector2 trajectory, float spin)
        {
            PolygonMaxRadius = radius;
            LineThickness = thickness;
            Position = position;
            AsteroidColor = color;
            Trajectory = trajectory;
            Spin = spin;
        }

        public static void AddAsteroid(GraphicsDevice graphicsDevice, Vector2 MousePosition, Vector2 Trajectory, float Spin)
        {
            var asteroid = new Asteroid(50f, 2f, MousePosition, Color.Black, Trajectory, Spin);
            asteroid.GeneratePolygonTexture(graphicsDevice);
            AsteroidsMain.Polygons.Add(asteroid);

            if(debugTexture == null)
            {
                debugTexture = new Texture2D(graphicsDevice, 2, 2);
                debugTexture.SetData(Enumerable.Repeat(Color.White, 4).ToArray());
            }

        }

        public void GeneratePolygonTexture(GraphicsDevice graphicsDevice)
        {
            TextureWidth = (int)(PolygonMaxRadius * 1.3f + LineThickness) * 2;
            // buffer TextureWidth to avoid cropping pixels drawn outside the radius.

            PolygonTextureMap = new Texture2D(graphicsDevice, TextureWidth, TextureWidth);

            // Stores ColorData of each pixel in 2D array:
            Color[] ColorData = new Color[TextureWidth * TextureWidth];
            
            // Centre vertex coordinates of TextureMap
            TextureCentre = new Vector2(TextureWidth / 2f, TextureWidth / 2f);

            // Number of corners / coordinates of the corners of the Polygon
            LocalVertices = new List<Vector2>();
            int NumberOfVertices = AsteroidsMain.NewRandom<int>(5, 8);

            Random random = new Random();

            for (int i = 0; i < NumberOfVertices; i++)
            {
                // Calculate next random angle
                float baseAngle = MathHelper.TwoPi * i / NumberOfVertices;
                float angleOffset = (float)(random.NextDouble() * MathHelper.Pi / 4 - MathHelper.Pi / 8);
                float angle = baseAngle + angleOffset;

                // Calculate next random radius
                float radiusOffset = (float)(random.NextDouble() * PolygonMaxRadius * 0.6f - PolygonMaxRadius * 0.3f);
                float randomRadius = PolygonMaxRadius + radiusOffset;

                // Calculate this vertex position
                float x = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                Vector2 vertex = TextureCentre + new Vector2(x, y) * randomRadius;

                // Add the vertex to the list
                LocalVertices.Add(vertex);
            }

            // Loop through each pixel coordinate in the Texture and its corresponding ColorData
            for (int y = 0; y < TextureWidth; y++)
            {
                for (int x = 0; x < TextureWidth; x++)
                {
                    Vector2 pixel = new Vector2(x, y);

                    // Determine whether pixel is inside the polygon
                    bool isInside = IsPointInPolygon(LocalVertices, pixel);

                    // Calculate the distance from the pixel to the polygon edge
                    float distance = DistanceToPolygon(LocalVertices, pixel) - LineThickness;

                    // Set the pixel color based on its distance to the polygon edge
                    ColorData[y * TextureWidth + x] = (isInside && Math.Abs(distance) <= LineThickness) ? Color.White : Color.Transparent;
                }
            }

            // Apply Color Data
            PolygonTextureMap.SetData(ColorData);
        }



        // Use a raycasting algorithm to determine whether points are inside the polygon
        private bool IsPointInPolygon(List<Vector2> polygonVertices, Vector2 Point)
        {
            bool isInside = false;
            int previousVertexIndex = polygonVertices.Count - 1;

            // Loop through each edge of the polygon
            for (int currentVertexIndex = 0; currentVertexIndex < polygonVertices.Count; previousVertexIndex = currentVertexIndex++)
            {
                // Get the current and previous vertices
                Vector2 currentVertex = polygonVertices[currentVertexIndex];
                Vector2 previousVertex = polygonVertices[previousVertexIndex];

                // Check if the point is within the Y-bounds of the edge
                bool pointIsBetweenYs = (currentVertex.Y > Point.Y) != (previousVertex.Y > Point.Y);

                if (pointIsBetweenYs)
                {
                    // Calculate x-coordinate where this point's Y intersects the edge
                    float intersectionX = previousVertex.X + (Point.Y - previousVertex.Y) * (currentVertex.X - previousVertex.X) / (currentVertex.Y - previousVertex.Y);

                    // If the point is to the left of this intersection, toggle the "inside" status
                    if (Point.X < intersectionX)
                    {
                        isInside = !isInside;
                    }
                }
            }
            return isInside;
        }

        // Calculate the minimum distance to the edges of the Polygon
        private float DistanceToPolygon(List<Vector2> polygonVertices, Vector2 Point)
        {
            float minDistance = float.MaxValue;

            for (int i = 0; i < polygonVertices.Count; i++)
            {
                Vector2 v1 = polygonVertices[i];
                Vector2 v2 = polygonVertices[(i + 1) % polygonVertices.Count]; 
                // Use Modulo % to ensure the index wraps around to the first vertex when reaching the last vertex

                float distance = DistanceToLine(v1, v2, Point);
                minDistance = Math.Min(minDistance, distance); 
            }
            return minDistance; 
        }

        private float DistanceToLine(Vector2 startPoint, Vector2 endPoint, Vector2 targetPoint)
        {
            Vector2 lineVector = endPoint - startPoint;            // Vector representing the direction and length of the line
            Vector2 pointVector = targetPoint - startPoint;        // Vector representing the direction and distance from startPoint to targetPoint

            float projectionLength = Vector2.Dot(pointVector, lineVector);
            /* Projection = Seeing where the targetPoint would "land" if you measured its position along the direction of the line.
             * Indicates where the targetPoint lies along the extended or extrapolated line, beyond just the segment between startPoint and endPoint.
             * Determines the closest point on the actual line segment (startPoint to endPoint) to the targetPoint. */

            if (projectionLength <= 0)                             
                return Vector2.Distance(targetPoint, startPoint);
            // If the projection falls before startPoint, closest is startPoint

            float LineLengthSquared = Vector2.Dot(lineVector, lineVector);
            // A vector describes the direction and distance from startPoint to endPoint, but its magnitude gives us the actual length.
            // LineLengthSquared calculates the squared length of the line segment to avoid the cost of a square root.

            if (LineLengthSquared <= projectionLength)
                return Vector2.Distance(targetPoint, endPoint);
            // Checks if the projected point lies beyond the end of the segment.

            float distanceRatio = projectionLength / LineLengthSquared;
            // Calculates how far along the line segment the projection falls as a ratio of the segment's length.

            Vector2 closestPoint = startPoint + distanceRatio * lineVector;
            // distanceRatio is between 0 and 1
            // Any value in-between places it somewhere along the segment.

            return Vector2.Distance(targetPoint, closestPoint);

        }



        public void Update()
        {
            // Update Texture Rotation
            RotationDegrees += Spin;

            // Update based on Trajectory
            Position += Trajectory;


        }


        public void DrawAtPosition(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                PolygonTextureMap,                                                                   // Texture2D
                Position,                                                                                     // Vector2D Coordinates
                null,                                                                                           // Source rectangle
                AsteroidColor,                                                                          // Color.White (colour tints)
                MathHelper.ToRadians(RotationDegrees),                            // Rotation in Radians
                TextureCentre,                                                                      // Centre of Origin
                TextureScale,                                                                       // Scale
                SpriteEffects.None,                                                             // Effects (I.E., Flipped)
                0f                                                                                        // Depth or Layer
            );

            // DEBUG
            DrawRedDots(spriteBatch);

        }

        public void DrawRedDots(SpriteBatch spriteBatch)
        {
            List<Vector2> positions = GetGlobalVertices();

            foreach (Vector2 position in positions)
            {
                spriteBatch.Draw(debugTexture, position, Color.Red);
            }
        }

        public List<Vector2> GetGlobalVertices()
        {
            GlobalVertices = new List<Vector2>();

            // Precompute the rotation matrix
            Matrix rotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(RotationDegrees));

            // Iterate over each vertex and apply transformations
            foreach (Vector2 localVertex in LocalVertices)
            {
                // Move the vertex relative to the texture center (i.e., as if the center is the origin)
                Vector2 centeredVertex = localVertex - TextureCentre;

                // Apply scaling
                Vector2 scaledVertex = centeredVertex * TextureScale;

                // Apply rotation around the new origin (centered vertex)
                Vector2 rotatedVertex = Vector2.Transform(scaledVertex, rotationMatrix);

                // Translate back to the actual global position
                Vector2 globalVertex = rotatedVertex + Position;

                GlobalVertices.Add(globalVertex);
            }

            return GlobalVertices;
        }

    }
}
