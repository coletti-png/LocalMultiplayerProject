////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Artistic.Sharpen
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Sharpen
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Common settings.

      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Sharpen settings.

      /// <summary> Algorithm used. </summary>
      public Algorithm algorithm = Algorithm.ContrastAdaptive;

      /// <summary> Blur patterns used with the Luma algorithm. </summary>
      public LumaPattern lumaPattern = LumaPattern.Normal;

      /// <summary> Kernel type used with the Laplacian algorithm. </summary>
      public LaplacianKernel laplacianKernel = LaplacianKernel.Kernel5x5;

      /// <summary> Edge detection method used with the Edge-Aware algorithm. </summary>
      public EdgeDetectionMethod edgeDetectionMethod = EdgeDetectionMethod.Sobel;

      /// <summary> Strength of the sharpening [0, 1]. Default 1. </summary>
      public float sharpness = 1.0f;

      /// <summary> Adjusts the radius of the sampling pattern [0, 6]. Default 1. </summary>
      public float offsetBias = 1.0f;

      /// <summary> Limits maximum amount of sharpening a pixel receives [0, 1]. Default 0.035. </summary>
      public float sharpClamp = 0.035f;

      /// <summary> Color vibrance [0, 2]. Default 0. </summary>
      public float vibrance = 0.0f;

      /// <summary> Edge threshold for Edge-Aware algorithm [0, 1]. Default 0.1. </summary>
      public float edgeThreshold = 0.1f;

      /// <summary> Edge width for Edge-Aware algorithm [0, 5]. Default 1.0. </summary>
      public float edgeWidth = 1.0f;

      /// <summary> Spatial sigma for Bilateral algorithm [0, 10]. Default 2.0. </summary>
      public float spatialSigma = 2.0f;

      /// <summary> Range sigma for Bilateral algorithm [0, 1]. Default 0.1. </summary>
      public float rangeSigma = 0.1f;

      /// <summary> Activate it to see the changing areas of the image. Only available in the Editor. </summary>
      public bool debugView = false;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10.0]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>
      public float saturation = 1.0f;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

#if !UNITY_6000_0_OR_NEWER
      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif
      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        algorithm = Algorithm.ContrastAdaptive;
        lumaPattern = LumaPattern.Normal;
        laplacianKernel = LaplacianKernel.Kernel5x5;
        edgeDetectionMethod = EdgeDetectionMethod.Sobel;
        sharpness = 1.0f;
        offsetBias = 1.0f;
        sharpClamp = 0.035f;
        vibrance = 0.0f;
        edgeThreshold = 0.1f;
        edgeWidth = 1.0f;
        spatialSigma = 2.0f;
        rangeSigma = 0.1f;
        debugView = false;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        affectSceneView = false;
#if !UNITY_6000_0_OR_NEWER
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }
    }
  }
}
