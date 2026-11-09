# Isometric Dash

`Isometric Dash` is a Unity 3D rehabilitation runner prototype designed around wrist rehabilitation training for stroke patients. It combines continuous forward movement, staying on a path, small steering adjustments, and result feedback into a lightweight training experience.

![Isometric Dash Home](docs/images/hero-home.png)

## Download and Play

- **Windows build:** [IsometricDash-Windows.zip](https://github.com/painfulmoments/Isometric/releases/latest/download/IsometricDash-Windows.zip)
- **Release page:** [Isometric Dash Playable Build](https://github.com/painfulmoments/Isometric/releases/tag/playable-v1.0.0)

Download the Windows ZIP from Releases, extract the entire archive, and launch `IsometricDash.exe`. Keep the executable, `UnityPlayer.dll`, `MonoBleedingEdge`, and `IsometricDash_Data` together. Unity and Unity Hub are only needed to open or modify the source project.

## Project Overview

- **Full project name:** `Isometric Dash`
- **Project type:** Unity 3D rehabilitation runner / rehabilitation interaction prototype
- **Intended users:** People undergoing wrist rehabilitation after a stroke
- **Unity version:** `2022.3.52f1c1`, as recorded in `ProjectSettings/ProjectVersion.txt`
- **Render pipeline:** Universal Render Pipeline `14.0.11`
- **Target platform:** Windows
- **Main scenes:** `Assets/Scenes/Start.unity` and `Assets/Scenes/SampleScene.unity`

## Gameplay and Controls

The current prototype uses automatic forward movement with mouse steering:

- The character moves forward continuously.
- Move the mouse left or right to adjust the direction of travel.
- Press `Esc` to pause or resume.
- Falling off the platforms ends the run.
- Your score increases over time. Reaching the target score wins the run.

The current controls use continuous mouse movement rather than separate actions assigned to the left and right mouse buttons.

## Features

- Runner gameplay provides clear feedback for the rehabilitation training rhythm.
- Includes a main menu, settings, pause, win and loss screens, and a return-to-menu flow.
- Retains `PEAK PCAN`, `UDP`, and `MySQL` integration modules for further rehabilitation-system development.
- Provides a foundation for continued work as a research prototype or course project.

## Source Project Requirements

To open and edit the project:

- Unity Hub
- Unity `2022.3.52f1c1`
- Windows

Hardware integration additionally requires the appropriate components for your setup:

- `PEAK PCAN` drivers and runtime libraries
- Encoder / CAN hardware
- An optional `Qt + UDP` communication application
- An optional `MySQL` database environment

The default gameplay input is mouse steering; the retained hardware modules are separate from the player controller.

## Open the Source Project

1. Add the repository root folder as a project in Unity Hub.
2. Open it with Unity `2022.3.52f1c1`.
3. Review `config/config.ini` and update it for your local integration environment where needed.
4. Open `Assets/Scenes/Start.unity`.
5. Press **Play** to enter the main menu, then start the game.

## Screenshots

Home screen:

![Isometric Dash Hero](docs/images/hero-home.png)

Gameplay:

![Gameplay 01](docs/images/gameplay-01.png)

![Gameplay 02](docs/images/gameplay-02.png)

![Gameplay 03](docs/images/gameplay-03.png)

Win and loss screens:

![Win Screen](docs/images/win-screen.png)

![Fail Screen](docs/images/fail-screen.png)

## Repository Structure

```text
Isometric/
├─ Assets/
│  ├─ Plugins/                  # Third-party DLLs, including MathNet and MySql.Data
│  ├─ Resources/                # Models, prefabs, backgrounds, and UI assets
│  ├─ Scenes/
│  │  ├─ Start.unity
│  │  └─ SampleScene.unity
│  ├─ Scripts/
│  │  ├─ Entity/
│  │  ├─ QtMysqlUDP/
│  │  ├─ ScriptsUDP/
│  │  └─ *.cs
│  └─ Sounds/
├─ config/
│  └─ config.ini
├─ docs/
│  └─ images/
├─ Packages/
├─ ProjectSettings/
└─ guide.md
```

## Key Scripts

- `Assets/Scripts/PlayerMovement.cs`: Character movement, scoring, win/loss conditions, and restarting
- `Assets/Scripts/PlatformSpawner.cs`: Straight-path platform generation and replenishment
- `Assets/Scripts/FallingPlatform.cs`: Platform falling and cleanup after the player leaves
- `Assets/Scripts/CameraFollow.cs`: Main camera follow behavior
- `Assets/Scripts/MainMenuController.cs`: Main menu, settings entry, and background-music switching
- `Assets/Scripts/PauseController.cs`: Pause panel and return-to-menu behavior
- `Assets/Scripts/QtMysqlUDP/ReadConfig.cs`: Loading `config/config.ini`
- `Assets/Scripts/ScriptsUDP/EncoderManager.cs`: Encoder / CAN data acquisition and angle mapping

## Current Status

- A playable runner prototype with a single gameplay scene is available.
- The main gameplay uses mouse controls and does not use external hardware as its player input.
- Rehabilitation hardware, logging, database, and UDP integration code remains in the source project.
- The project is intended as a research prototype for further iteration.

## Detailed Guide

For a fuller analysis of the project structure, script responsibilities, scenes, integration flow, and technical debt, see [guide.md](guide.md) (in Chinese).

## Acknowledgments

- Special thanks for artwork: Han Yuhe
- Special thanks for modeling: Ju Yiming
