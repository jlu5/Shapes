# Shapes That Move

**Shapes That Move** is a simple platformer game written in half a year with the [Unity3D](https://unity3d.com/) engine. It employs realistic™ physics, moddable level loading, and extensive level objects.

<img src="shapes1.gif" width="360" height="240"> <img src="shapes2.gif" width="360" height="240">

## Compilation
Shapes That Move is built using Unity 5.6 (Personal) on Windows and Linux. Opening the project it is as simple as cloning the repository and opening it in the Unity Editor.

Single-level builds are supported using the level scene in the build - multi-level builds use the `MainScene.unity` scene in `Assets/`.

## Levels
Levels are stored in the `Assets/StreamingAssets/Levels` folder. Each level is basically a Unity scene file, but some considerations must be kept in mind:

1) The level should include the `GameState` prefab, and preferably the `ShapesDefaultCamera` and `GameBoundary` prefabs for consistency. It is therefore a good idea to copy `Assets/LevelTemplate.unity` to the levels folder and begin work there.

2) The level scene MUST have its asset bundle set to `levels`, or it will fail to import in multi-level mode (i.e. via MainScene).

3) *Safe* objects to add into the world in a level include everything in `WorldItems/` and `Resources/SimpleTextMesh` (for in-game textboxes).

* Script-wise, `AutoMaterial`, `AutoMover`, and `KillOnTouch` are safe to add to most objects. (`BindDisplay` works as well but that currently requires manual tweaking and a dummy gameobject)

### Level packs

Level packs are simple JSON files that tell Shapes' level selector which scenes to expose and in which order. All level packs named `Assets/StreamingAssets/Levels/*.levelpack` are automatically discovered on runtime, so there is no configuration needed to explicitly load them. For level creators, you will likely want to create a new `.levelpack` file or add your work to an existing one.

The level pack format is as follows: an object with a "levels" key pointing to a list of objects with a "name" and "path" key each. *Omit* file extensions when specifying the scene path.

```json
{
    "levels":
        [{
            "name": "How to Move",
            "path": "Tut1Movement"
        },
	{
            "name": "How to Hop",
            "path": "Tut2Jump"
        },
	    "..."
        ]
}
```

## License
GPLv2
