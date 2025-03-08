![CGMF](https://github.com/user-attachments/assets/edcf2cc2-c617-485c-8507-aa200f719e17)

---

## Description

**CustomGameModesFramework**

This project is a framework for creating and managing custom game modes in the game Slendytubbies 3. It provides a set of tools and APIs that allow developers to easily add new game modes, expanding the functionality of the game.

### Key Features:
- **Game Mode Registration**: Simple system for registering new game modes via the `GameModeInfo` attribute.
- **Modular Structure**: Each game mode can be implemented in a separate class, making it easy to maintain and extend.
- **Auto Loading**: Automatically detect and register game modes from other DLLs, allowing you to create mods independently of each other.
- **Flexible Configuration**: Ability to customize various aspects of the gameplay, such as round start events, player spawns, round end, etc.

### How it works:
1. **Creating a new game mode**: Extend your class from `CustomGameMode` and use the `GameModeInfo` attribute to describe it.
2. **Registering the mode**: The framework will automatically detect and register your game mode when the game starts.
3. **Using lifecycle methods**: Override methods like `OnAwake`, `OnRoundEnded`, `OnSpawnPlayer` and others to add your logic.

### Requirements:
- MelonLoader 0.7.0
