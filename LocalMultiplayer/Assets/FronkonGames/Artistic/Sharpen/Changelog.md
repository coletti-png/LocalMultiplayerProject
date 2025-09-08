# Changelog
All notable changes to this package will be documented in this file.

## [2.2.0] - 02-09-2025

# Added
- **Laplacian Sharpening Algorithm**: Uses Laplacian operator to detect and enhance edges with three kernel sizes (3x3, 5x5, 7x7).
- **Edge-Aware Sharpening Algorithm**: Applies sharpening only where edges are detected using Sobel, Canny, or Laplacian edge detection methods.
- **Bilateral Sharpening Algorithm**: Combines spatial and range filtering for edge-preserving sharpening that preserves texture while reducing noise.
- New parameters for each algorithm:
  - Laplacian: Kernel size selection (3x3, 5x5, 7x7)
  - Edge-Aware: Edge threshold, edge width, and edge detection method
  - Bilateral: Spatial sigma and range sigma for filtering control

# Fix
- Improved VR support for Unity 6.

## [2.1.1] - 22-04-2025

# Fix
- macOS Metal support (thanks to Sergey Akopkokhyants).

## [2.1.0] - 08-03-2025

# Added
- Support for effects in multiples Renderers.
- 'Affect the Scene View' added to Unity 6.

# Removed
- Removed Instance.settings, use .Instance.settings

# Fix
- Camera 'Post Processing' checkbox fixed.
- Errors when domain reload is disabled.
- Memory leak in Unity 2022.3.
- Documentation URL fixed.

## [2.0.0] - 11-11-2024

# Added
- Support for Unity 6.

# Changed
- Performance improvements.
- Removed support for Unity 2021.3.

# Fix
- Small fixes.

## [1.1.0] - 15-07-2024

# Changed
- Removed the AddRenderFeature() and RemoveRenderFeature() from the effect that damaged the configuration file.
- Performance improvements.

# Fix
- Small fixes.

## [1.0.2] - 02-06-2024

# Changed
- New website and documentation.

## [1.0.1] - 10-02-2024

# Fix
- 'Remap' function renamed to avoid collisions.

## [1.0.0] - 09-10-2023

- Initial release.