# ShrinkShot

A hyper-casual mobile game made with Unity. A ball rolls along a path towards a door, but obstacles block the way. Hold the screen to charge a shot out of the ball's own volume, then release to fire. The hit obstacle gets infected, and the infection spreads to its neighbours: dense clusters burn through easily, while lonely obstacles stop the chain. Once the path is clear, the ball hops through the door.

Volume is conserved: the bigger the shot, the smaller the ball gets (and the path narrows with it). If the ball shrinks to a critical size, the level is lost.

## Controls

- **Hold** (touch or left mouse button): charge a shot
- **Release**: fire

## Tech

- Unity **6000.3.21f1**
- Universal Render Pipeline (URP)
- Input System

## Structure

```
Assets/
  Sources/
    Config/     GameConfig: ScriptableObject with gameplay parameters
    Core/       Pure logic without MonoBehaviours: BallMath (volume <-> radius), LevelRules (blocking, shot hits, infection spread)
    Gameplay/   GameController (level loop), balls, obstacles, door, camera, input
    UI/         GameHud
    GameBootstrapper.cs: entry point
  Levels/       Level prefabs (Level01)
  Scenes/       ShrinkShot.unity: main scene
build/          Ready-made Android build (shrinkshot.apk)
```

## Getting started

1. Open the project in Unity 6000.3.21f1.
2. Open `Assets/Scenes/ShrinkShot.unity` and press Play.

For Android, you can install `build/shrinkshot.apk` directly.
