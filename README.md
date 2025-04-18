# Asteroids MonoGame

A 2D asteroid simulation using the MonoGame framework. Uses procedurally generated asteroid shapes, collision detection, and physics-based bouncing.

## Features
- **Procedural Asteroid Generation**: Each asteroid is generated with a unique polygonal shape using random vertices.
- **Collision Detection**: Implements the Separating Axis Theorem (SAT) for accurate polygon collision detection.
- **Physics-Based Bouncing**: Asteroids bounce off each other upon collision, with velocity adjustments based on their trajectories.
- **Debugging Tools**: Includes visual debugging such as red dots to display asteroid vertices.


## Prerequisites
- Install [VS Code](https://code.visualstudio.com/)
- Download [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- Install [C# Dev](https://marketplace.visualstudio.com/items/?itemName=ms-dotnettools.csdevkit)
- Run `git clone https://github.com/Gameboard-dev/asteroids.git`
- Run 'Asteroid.cs' in VS Code

## Future Enhancements
- Add sound effects for collisions.
- Asteroid splitting upon collision.
- A scoring system and player-controlled spaceship.
- A physics-based grappling rope and alien rescue.