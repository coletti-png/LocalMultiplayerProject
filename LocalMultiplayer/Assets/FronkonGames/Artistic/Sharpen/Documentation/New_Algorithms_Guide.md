# New Sharpen Algorithms Guide

This guide covers the three new sharpen algorithms added to the FronkonGames Artistic Sharpen module in version 2.2.0.

## Overview

The module now includes five sharpen algorithms:
1. **Luma** (Original) - Unsharp mask variant working in luma channel
2. **Contrast Adaptive** (Original) - Dynamic per-pixel sharpening regulation
3. **Laplacian** (New) - Edge detection and enhancement using Laplacian operator
4. **Edge-Aware** (New) - Intelligent sharpening only where edges are detected
5. **Bilateral** (New) - Edge-preserving sharpening with noise reduction

## Laplacian Algorithm

### Description
The Laplacian algorithm uses the Laplacian operator to detect edges and enhance them. This is a classic image processing technique that provides precise control over edge enhancement.

### How It Works
- Applies a Laplacian kernel to detect edges in the image
- Subtracts the detected edges from the original image to create sharpening
- Uses different kernel sizes for varying levels of precision

### Parameters
- **Kernel Size**: Choose between 3x3, 5x5, or 7x7 kernels
  - **3x3**: Fastest but less precise, good for subtle sharpening
  - **5x5**: Good balance of quality and performance (recommended)
  - **7x7**: Highest quality but more computationally expensive
- **Sharpness**: Controls the intensity of the sharpening effect [0, 1]
- **Offset Bias**: Adjusts the radius of the sampling pattern [0, 6]

### Best Use Cases
- Medical imaging and scientific applications
- High-precision edge enhancement
- When you need predictable, mathematical sharpening
- Architectural and technical photography

### Tips
- Start with the 5x5 kernel for most applications
- Use lower sharpness values (0.3-0.7) to avoid artifacts
- Higher kernel sizes provide more precise edge detection but are slower

## Edge-Aware Algorithm

### Description
The Edge-Aware algorithm intelligently applies sharpening only where edges are detected, preventing over-sharpening of smooth areas while enhancing important details.

### How It Works
- Uses edge detection to identify areas that need sharpening
- Applies sharpening with intensity proportional to edge strength
- Preserves smooth areas while enhancing edges and details

### Parameters
- **Edge Detection Method**: Choose between Sobel, Canny, or Laplacian
  - **Sobel**: Good general-purpose edge detection, fast
  - **Canny**: More precise edge detection with noise suppression
  - **Laplacian**: Simple and fast edge detection
- **Edge Threshold**: Minimum edge strength to apply sharpening [0, 1]
- **Edge Width**: Controls the transition zone around edges [0, 5]
- **Sharpness**: Controls the intensity of the sharpening effect [0, 1]

### Best Use Cases
- Portrait photography (preserves skin texture)
- Images with mixed content (smooth and detailed areas)
- When you want to avoid over-sharpening noise
- Natural-looking sharpening for general photography

### Tips
- Start with Sobel edge detection for most applications
- Use lower edge threshold values (0.05-0.2) for subtle effects
- Higher edge width values create smoother transitions
- Canny detection is best for noisy images

## Bilateral Algorithm

### Description
The Bilateral algorithm combines spatial and range filtering for edge-preserving sharpening. It preserves edges while smoothing noise, making it ideal for images with texture that should be preserved.

### How It Works
- Uses bilateral filtering to create a "smart blur"
- Applies sharpening by subtracting the bilateral blur from the original
- Preserves edges based on both spatial distance and color similarity

### Parameters
- **Spatial Sigma**: Controls the spatial extent of the filter [0, 10]
  - Lower values: More local sharpening
  - Higher values: More global smoothing
- **Range Sigma**: Controls color similarity threshold [0, 1]
  - Lower values: More edge preservation
  - Higher values: More smoothing
- **Sharpness**: Controls the intensity of the sharpening effect [0, 1]

### Best Use Cases
- Images with noise that should be reduced
- Textured surfaces (fabric, wood, stone)
- When you want to preserve fine details while reducing noise
- Medical imaging and microscopy

### Tips
- Start with spatial sigma around 2.0 and range sigma around 0.1
- Lower range sigma values preserve more edges
- Higher spatial sigma values create smoother results
- Good for images with high ISO noise

## Algorithm Comparison

| Algorithm | Speed | Quality | Noise Handling | Edge Preservation | Best For |
|-----------|-------|---------|----------------|-------------------|----------|
| Luma | Fast | Good | Poor | Good | General use, performance |
| Contrast Adaptive | Medium | Very Good | Good | Very Good | Balanced results |
| Laplacian | Medium | Excellent | Poor | Excellent | Precision work |
| Edge-Aware | Medium | Very Good | Excellent | Very Good | Natural results |
| Bilateral | Slow | Excellent | Excellent | Excellent | Noisy images |

## Performance Considerations

### Fastest to Slowest
1. Luma (Fast)
2. Contrast Adaptive (Medium)
3. Laplacian (Medium)
4. Edge-Aware (Medium)
5. Bilateral (Slow)

### Memory Usage
- All algorithms use similar memory footprint
- Bilateral uses slightly more due to larger sampling window
- Laplacian 7x7 uses more texture samples than 3x3

## Recommended Settings by Use Case

### Portrait Photography
- **Algorithm**: Edge-Aware
- **Edge Detection**: Sobel
- **Edge Threshold**: 0.1
- **Edge Width**: 1.5
- **Sharpness**: 0.7

### Landscape Photography
- **Algorithm**: Contrast Adaptive
- **Sharpness**: 0.8
- **Offset Bias**: 1.2

### Product Photography
- **Algorithm**: Laplacian
- **Kernel**: 5x5
- **Sharpness**: 0.6

### Noisy Images
- **Algorithm**: Bilateral
- **Spatial Sigma**: 2.5
- **Range Sigma**: 0.08
- **Sharpness**: 0.5

### Technical/Medical Imaging
- **Algorithm**: Laplacian
- **Kernel**: 7x7
- **Sharpness**: 0.4

## Troubleshooting

### Common Issues and Solutions

**Over-sharpening artifacts:**
- Reduce sharpness value
- Use Edge-Aware algorithm with higher threshold
- Try Bilateral algorithm with higher range sigma

**Performance issues:**
- Use smaller Laplacian kernels (3x3 instead of 7x7)
- Switch to Luma or Contrast Adaptive algorithms
- Reduce bilateral spatial sigma

**Noise amplification:**
- Use Bilateral algorithm
- Reduce sharpness in all algorithms
- Use Edge-Aware with higher threshold

**Loss of detail:**
- Increase sharpness value
- Use Laplacian with larger kernel
- Reduce bilateral range sigma

## Migration from Previous Versions

If you're upgrading from version 2.1.x or earlier:

1. **Luma and Contrast Adaptive** algorithms remain unchanged
2. **New algorithms** are available as additional options
3. **Default algorithm** is still Contrast Adaptive
4. **All existing parameters** work the same way
5. **New parameters** are only used by new algorithms

## Future Enhancements

Planned features for future versions:
- Machine learning-based sharpening
- Temporal sharpening for video
- Content-aware algorithm selection
- Advanced noise estimation
- GPU-optimized bilateral filtering

---

For more information, visit the [online documentation](https://fronkongames.github.io/store/artistic.html) or contact support at fronkongames@gmail.com.
