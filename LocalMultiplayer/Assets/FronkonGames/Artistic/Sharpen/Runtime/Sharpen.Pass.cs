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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

namespace FronkonGames.Artistic.Sharpen
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Sharpen
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");
        internal static readonly int OffsetBias = Shader.PropertyToID("_OffsetBias");
        internal static readonly int SharpClamp = Shader.PropertyToID("_SharpClamp");
        internal static readonly int Vibrance = Shader.PropertyToID("_Vibrance");
        internal static readonly int EdgeThreshold = Shader.PropertyToID("_EdgeThreshold");
        internal static readonly int EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        internal static readonly int SpatialSigma = Shader.PropertyToID("_SpatialSigma");
        internal static readonly int RangeSigma = Shader.PropertyToID("_RangeSigma");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        internal static readonly string LumaFast = "LUMA_FAST";
        internal static readonly string LumaNormal = "LUMA_NORMAL";
        internal static readonly string LumaWider = "LUMA_WIDER";
        internal static readonly string LumaPyramid = "LUMA_PYRAMID";

        internal static readonly string Laplacian3x3 = "LAPLACIAN_3X3";
        internal static readonly string Laplacian5x5 = "LAPLACIAN_5X5";
        internal static readonly string Laplacian7x7 = "LAPLACIAN_7X7";

        internal static readonly string EdgeSobel = "EDGE_SOBEL";
        internal static readonly string EdgeCanny = "EDGE_CANNY";
        internal static readonly string EdgeLaplacian = "EDGE_LAPLACIAN";

        internal static readonly string DebugView = "DEBUG_VIEW";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;
#if UNITY_6000_0_OR_NEWER
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        if (settings.algorithm == Algorithm.Luma)
        {
          switch (settings.lumaPattern)
          {
            case LumaPattern.Fast: material.EnableKeyword(Keywords.LumaFast); break;
            case LumaPattern.Normal: material.EnableKeyword(Keywords.LumaNormal); break;
            case LumaPattern.Wider: material.EnableKeyword(Keywords.LumaWider); break;
            case LumaPattern.Pyramid: material.EnableKeyword(Keywords.LumaPyramid); break;
          }

          material.SetFloat(ShaderIDs.SharpClamp, settings.sharpClamp);
        }
        else if (settings.algorithm == Algorithm.Laplacian)
        {
          switch (settings.laplacianKernel)
          {
            case LaplacianKernel.Kernel3x3: material.EnableKeyword(Keywords.Laplacian3x3); break;
            case LaplacianKernel.Kernel5x5: material.EnableKeyword(Keywords.Laplacian5x5); break;
            case LaplacianKernel.Kernel7x7: material.EnableKeyword(Keywords.Laplacian7x7); break;
          }
        }
        else if (settings.algorithm == Algorithm.EdgeAware)
        {
          switch (settings.edgeDetectionMethod)
          {
            case EdgeDetectionMethod.Sobel: material.EnableKeyword(Keywords.EdgeSobel); break;
            case EdgeDetectionMethod.Canny: material.EnableKeyword(Keywords.EdgeCanny); break;
            case EdgeDetectionMethod.Laplacian: material.EnableKeyword(Keywords.EdgeLaplacian); break;
          }

          material.SetFloat(ShaderIDs.EdgeThreshold, settings.edgeThreshold);
          material.SetFloat(ShaderIDs.EdgeWidth, settings.edgeWidth);
        }
        else if (settings.algorithm == Algorithm.Bilateral)
        {
          material.SetFloat(ShaderIDs.SpatialSigma, settings.spatialSigma);
          material.SetFloat(ShaderIDs.RangeSigma, settings.rangeSigma);
        }

#if UNITY_EDITOR
        if (settings.debugView == true)
          material.EnableKeyword(Keywords.DebugView);
#endif

        material.SetFloat(ShaderIDs.Sharpness, settings.sharpness * (settings.algorithm == Algorithm.Luma ? 3.0f : 1.0f));
        material.SetFloat(ShaderIDs.OffsetBias, settings.offsetBias);

        material.SetFloat(ShaderIDs.Vibrance, settings.vibrance);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
      }

#if UNITY_6000_0_OR_NEWER
      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        if (material == null || settings.intensity == 0.0f)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity == 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        int passIndex = settings.algorithm switch
        {
          Algorithm.Luma => 0,
          Algorithm.ContrastAdaptive => 1,
          Algorithm.Laplacian => 2,
          Algorithm.EdgeAware => 3,
          Algorithm.Bilateral => 4,
          _ => 1
        };
        cmd.Blit(colorBuffer, renderTextureHandle0, material, passIndex);
        cmd.Blit(renderTextureHandle0, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd) => cmd.ReleaseTemporaryRT(renderTextureHandle0);
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
