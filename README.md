# RewardOpeningSequence

---

## Implementation Details

### Core Script: `Unboxing.cs`

The script manages the reward sequence with the following features:

- **Intro Setup**: Initializes the box and buttons, hides unnecessary elements.
- **Opening Animation**: Triggers sprite animations, reveals prizes, and logs events.
- **Reset Functionality**: Hides prizes and reinitializes the screen for replay.
- **Sprite Animation**: Dynamically creates frames from a sprite sheet.

### Key Methods

- `OnOpenBoxClicked()`: Handles the box opening sequence.
- `OnResetClicked()`: Resets the sequence to its initial state.
- `CreateSpriteSheetFrames()`: Extracts frames from a sprite sheet.
- `FadeOutButton()`: Fades out the open button smoothly using DOTween.

### Notes

- All assets (textures, shaders, VFX, etc.) are original creations as per the constraints.
- DOTween library is used for smooth UI and animation effects.

---

## Optimization

To improve performance and reduce draw calls, the following optimizations were applied:

- **Mesh Optimization**: The original meshes have been optimized by reducing polygon counts, merging redundant vertices, and simplifying complex geometry to ensure better performance, particularly on mobile devices.
  
- **Texture Atlas Creation**: Multiple textures were combined into a single atlas to minimize the number of texture swaps during rendering, which reduces draw calls when using one material instance. This improves performance and optimizes memory usage by efficiently packing multiple textures into one.

---

## Deliverables

The repository includes:

1. **Source Code**: Complete Unity project with organized file structure.
2. **Unity Package**: Exported `.unitypackage` for importing into other projects.
3. **Android Build**: An `.apk` file for testing on mobile devices.
4. **Art Source Files**: An `.psd` and `.aep` file with source animation and layered button.
5. **Notes**: This `README.md` highlights the project details and implementation.

---

## Notes for Reviewers

- Focus on the overall polish and "juicy" feel of the sequence.
- Evaluate the visual and animation quality, prioritization, and user interaction.
- Verify performance on mobile platforms, especially after the mesh optimization and texture atlas integration.

---

## Tools and Libraries

- **Unity 2021.3 LTS**
- **URP** (Universal Render Pipeline)
- **DOTween** for animations
- **UI ToolKit** for UI
- **Mesh Optimization Tools**: For reducing polygon counts
- **Texture Atlas Creation**: Combining multiple textures into a single atlas to reduce draw calls

---

## Future Improvements

- Add particle effects for box opening.
- Integrate actual reward data and HUD elements.
- Optimize textures and meshes further for better memory and performance savings.

---
