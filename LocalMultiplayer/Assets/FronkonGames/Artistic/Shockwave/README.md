# Artistic: Shockwave

Create stunning shockwave effects with customizable distortion, chromatic aberration, flares, and edge detection. Perfect for explosions, time jumps, portals, and other dramatic visual effects in your Universal Render Pipeline projects.

## Features

- **Customizable Shockwave Ring**: Control radius, center position, strength, width, and asymmetric inner/outer ring dimensions
- **Chromatic Aberration**: Per-channel color separation for realistic lens distortion effects
- **Dynamic Flares**: Animated radial flares with customizable frequency, speed, threshold, and color blending
- **Procedural Noise**: Add organic variation to the shockwave with scale and speed controls
- **Edge Detection & Enhancement**: Sobel edge detection with optional noise and plasma effects
- **Advanced Color Blending**: 23 blend modes including Additive, Multiply, Screen, Overlay, Hue, Saturation, and more
- **Hue Variation**: Angular and radial hue shifts for rainbow or prismatic effects
- **Real-time Animation**: All parameters can be animated via scripts for dynamic effects
- **Performance Optimized**: Efficient GPU shaders with WebGL support
- **URP Integration**: Seamless Universal Render Pipeline renderer feature

## Requisites

### Unity 6

* Unity 6000.0.23f1 or higher.
* Universal RP 17.0.3 or higher. 

### Unity 2022 LTS

* Unity 2022.3.61f1 or higher.
* Universal RP 12.1.15 or higher. 

## Getting Started

1. Read the documentation at folder '_FronkonGames/Artistic/Shockwave/Documentation_' or visit the [online version](https://fronkongames.github.io/store/artistic.html).
2. Open the scene at folder '_FronkonGames/Artistic/Shockwave/Demo/_'.

## Settings' Parameters

### Common Settings
- **Intensity** `[0, 1]`: Controls the overall effect intensity. Set to 0 to disable the effect entirely. Default: 1.0

### Shockwave Settings
- **Radius** `[0, 1]`: Controls the shockwave ring radius (0 = center, 1 = outside screen). Default: 0.0
- **Center** `Vector2`: Screen-space center position of the shockwave (0.5, 0.5 = screen center). Default: (0.5, 0.5)
- **Strength** `[0, 5]`: Intensity of the distortion effect. Default: 1.0
- **Width** `[0.01, 0.75]`: Base width of the shockwave ring. Default: 0.25
- **Ring Width Inner** `[0.01, 1.0]`: Inner ring boundary width. Default: 0.5
- **Ring Width Outer** `[0.01, 1.0]`: Outer ring boundary width. Default: 0.5
- **Ring Sharpness** `[1.0, 32.0]`: Falloff sharpness exponent. Default: 8.0
- **Ring Skew** `[-1.0, 1.0]`: Pushes ring thickness inward/outward. Default: 0.0
- **Color Strength** `Vector3`: Per-channel color intensity multiplier. Default: (1, 1, 1)
- **Shockwave Color Blend**: Color blending mode for the shockwave effect. Default: Solid
- **Chromatic Aberration** `Vector3`: Per-channel offset for color separation. Default: (-1, 2, 5)
- **Inside Tint** `Color`: Color tint applied to the inner shockwave area. Default: Light cyan

### Flares Settings
- **Flares** `[0, 1]`: Intensity of radial flare effects. Default: 0.2
- **Flares Color Blend**: Blending mode for flare colors. Default: Solid
- **Flares Color** `Color`: Base color for flares. Default: Light cyan
- **Flares Frequency** `[0, 64]`: Number of flare rays around the ring. Default: 24
- **Flares Speed** `[0, 5]`: Animation speed of flares. Default: 1.5
- **Flares Threshold** `[0, 1]`: Noise threshold for flare visibility. Default: 0.35
- **Flares Softness** `[0, 10]`: Softness factor for flare edges. Default: 6.0

### Noise Settings
- **Noise** `[0, 1]`: Amount of procedural noise variation. Default: 0.2
- **Noise Scale** `[0.1, 64]`: Spatial frequency of noise pattern. Default: 8.0
- **Noise Speed** `[0, 5]`: Animation speed of noise. Default: 0.2

### Edge Detection Settings
- **Edge** `[0, 1]`: Intensity of edge enhancement effect. Default: 1.0
- **Edge Color Blend**: Blending mode for edge colors. Default: Hue
- **Edge Color** `Color`: Color applied to detected edges. Default: Cyan
- **Edge Width** `[0.1, 5]`: Sampling width for edge detection in texels. Default: 1.0
- **Edge Noise** `[0, 1]`: Amount of noise applied to edge sampling. Default: 1.0
- **Edge Noise Scale** `[0.1, 64]`: Scale of edge noise pattern. Default: 8.0
- **Edge Noise Speed** `[0, 10]`: Speed of edge noise animation. Default: 1.0
- **Edge Plasma** `[0, 1]`: Intensity of plasma effect on edges. Default: 0.1
- **Edge Plasma Scale** `[0.01, 64]`: Scale of plasma pattern. Default: 5.0
- **Edge Plasma Speed** `[0, 10]`: Animation speed of plasma. Default: 1.0

### Hue Variation Settings
- **Hue Variation** `[0, 1]`: Amount of angular hue shifting. Default: 0.0 (disabled)
- **Hue Variation Scale** `[0, 2]`: Angular frequency of hue bands. Default: 1.0
- **Hue Variation Speed** `[-10, 10]`: Speed of hue animation. Default: 1.0
- **Hue Variation Radial** `[0, 1]`: Amount of radial hue shifting. Default: 0.0 (disabled)
- **Hue Variation Radial Scale** `[0, 64]`: Radial frequency of hue bands. Default: 4.0

### Color Adjustment Settings
- **Brightness** `[-1.0, 1.0]`: Brightness adjustment for inside area. Default: 0.0
- **Contrast** `[0.0, 2.0]`: Contrast adjustment for inside area. Default: 1.0
- **Gamma** `[0.1, 10.0]`: Gamma correction for inside area. Default: 1.0
- **Hue** `[0.0, 1.0]`: Global hue shift (color wheel rotation). Default: 0.0
- **Saturation** `[0.0, 2.0]`: Color intensity adjustment. Default: 1.0
- **Color Blend**: Base color blending mode for inside area. Default: Solid

### Advanced Settings
- **Affect Scene View** `bool`: Whether the effect appears in Scene View. Default: false
- **Enable Profiling** `bool`: Enable render pass profiling (Unity 2022 only). Default: false
- **Filter Mode**: Texture filtering mode (Unity 2022 only). Default: Bilinear
- **When To Insert**: Render pass injection point. Default: BeforeRenderingPostProcessing

## Contact

Send me an email to _fronkongames@gmail.com_ If you want to report a bug, please send the [log file](https://docs.unity3d.com/Manual/LogFiles.html) as well.

## Remarks

**THIS ASSET CANNOT BE HOSTED IN PUBLIC REPOSITORIES**
