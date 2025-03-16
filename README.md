![CGMF](https://github.com/user-attachments/assets/edcf2cc2-c617-485c-8507-aa200f719e17)

---

# **CustomGameModesFramework**

The **Custom Game Modes Framework** is a powerful modding tool designed to allow the creation of custom game modes in **Slendytubbies 3**.

## Features

- **Custom Game Mode Support**: Easily create and integrate your own game modes.
- **Highly Modular**: The framework is built with flexibility in mind, allowing developers to customize various aspects of the game.
- **Event System**: Hook into key game events such as room creation, player spawning, round ending, and other thing to implement custom behavior.
- **Game Mode Manager**: A centralized system that handles actions for different phases of the game (e.g., Start, SetupRoom, StartGame, OnGUI, FixedUpdate, etc.).

## Known Issues & Nuances

1. **Compatibility**:
   - The framework may not work well alongside other mods, cheats, or third-party tools due to potential conflicts in patching and hooking core game methods. Use with caution when combining with other modifications.

2. **Edge Cases**:
   - Certain "bugs" or unexpected behaviors can occur during gameplay. For example:
     - If a player leaves the room unexpectedly, some hooks may fail to execute properly.
     - These issues are challenging to address as they depend on unpredictable user actions and network states.
   - While these scenarios are not traditional bugs, they can lead to inconsistent behavior in rare cases.

3. **Dependencies**:
   - This framework requires **MelonLoader v0.7.0**. Ensure you have this exact version installed before using the mod.

4. **Documentation Required**:
   - Some advanced features and troubleshooting steps will require additional documentation to fully understand and utilize the framework's capabilities.

## Getting Started

To get started with the **Custom Game Modes Framework**, follow these steps:

1. Install **MelonLoader v0.7.0** if you haven't already.
2. Place the compiled DLL file of this framework into your `Mods` folder.
3. Create your custom game mode scripts by extending the provided classes and implementing the necessary hooks(more details in the documentation).
4. Test your game modes locally or with friends to ensure everything works as intended.

For detailed instructions, refer to the [official documentation](#) (coming soon).

## Contributing

Contributions are welcome! If you'd like to help improve the framework, fix bugs, or add new features, feel free to submit pull requests or open issues on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

### Notes for Developers

If you're planning to develop custom game modes using this framework, keep the following in mind:

- Always test your code thoroughly to avoid runtime errors.
- Be mindful of how your custom logic interacts with the base game and other mods.
- Document any quirks or limitations you encounter so future developers can benefit from your experience.
