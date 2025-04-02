# SurfaceAirMissile

A classic arcade-style missile defense game built with Visual Basic .NET where players launch missiles to intercept enemy jets.


*![image](https://github.com/user-attachments/assets/67c868b1-89e5-469b-ae3f-c2b76dceb031)*

## Features

- **Multiple Missile Types**:
  - Standard: Balanced speed and gravity
  - Fast: Higher velocity but less control
  - Guided: Tracks and follows enemy aircraft

- **Dynamic Difficulty**: Enemy jets increase in number and speed as your score rises

- **Parallax Scrolling**: Multi-layered backgrounds create a sense of depth and immersion

- **Weather Effects**: Animated clouds drift across the sky

- **Visual Effects**:
  - Missile trails
  - Colorful explosions
  - Particle effects
  - Realistic jet designs with engine exhaust

## How to Play

### Controls
- **Arrow Keys**: Move your missile launcher left and right
- **Space**: Fire standard missile
- **Z**: Launch fast missile
- **X**: Deploy guided missile
- **P**: Pause/Resume game
- **Enter**: Start new game or restart after game over

### Objective
Shoot down as many enemy jets as possible! The game increases in difficulty as you progress. Can you reach the target score of 100 before the enemies overwhelm you?

## Technical Implementation

### Architecture
The game is built using object-oriented principles with separate classes for:
- `Form1`: Main game loop and rendering
- `SurfaceMissile`: Various missile types and physics
- `Jet`: Enemy aircraft movement and rendering
- `Explosion`: Visual explosion effects
- `BackgroundLayer`: Parallax scrolling background elements
- `Cloud`: Atmospheric cloud animations

### Graphics
- Custom vector graphics rendered with GDI+
- Smooth animations using double buffering
- Particle effects for explosions
- LinearGradientBrush for realistic coloring

## Requirements

- Windows operating system
- .NET 8.0 or higher
- Visual Studio 2022 (for development)

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/mars-rift/SurfaceAirMissile.git
   ```

2. Open the solution file in Visual Studio:
   ```
   SurfaceAirMissile.sln
   ```

3. Build and run the project (F5)

## Future Enhancements

- Add sound effects and background music
- Implement high score system
- Add power-ups and special weapons
- Create additional enemy types
- Add level progression with unique backgrounds

## Development Notes

The game utilizes several optimization techniques:
- Object pooling for frequently created/destroyed objects
- Graphics state management for efficient rendering
- Cached calculations to improve performance
- Collision detection optimizations

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Inspired by classic arcade missile defense games
- Built as a learning project for Visual Basic .NET and game development fundamentals

---

