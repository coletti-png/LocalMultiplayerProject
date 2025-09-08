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

namespace FronkonGames.Artistic.Spark
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Spark
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
      private TextureHandle renderTextureHandle0;
      private TextureHandle renderTextureHandle1;
      private TextureHandle renderTextureHandle2;
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");
      private readonly int renderTextureHandle1 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH1");
      private readonly int renderTextureHandle2 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH2");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int DownSample = Shader.PropertyToID("_Downsample");
        internal static readonly int Rays = Shader.PropertyToID("_Rays");
        internal static readonly int Blend = Shader.PropertyToID("_Blend");
        internal static readonly int Gain = Shader.PropertyToID("_Gain");
        internal static readonly int Size = Shader.PropertyToID("_Size");
        internal static readonly int Threshold = Shader.PropertyToID("_Threshold");
        internal static readonly int ThresholdClamp = Shader.PropertyToID("_ThresholdClamp");
        internal static readonly int Spin = Shader.PropertyToID("_Spin");
        internal static readonly int Barrel = Shader.PropertyToID("_Barrel");
        internal static readonly int BarrelBend = Shader.PropertyToID("_BarrelBend");
        internal static readonly int Tint = Shader.PropertyToID("_Tint");
        internal static readonly int Blur = Shader.PropertyToID("_Blur");
        internal static readonly int BlurWeights = Shader.PropertyToID("_BlurWeights");
        internal static readonly int Artifacts = Shader.PropertyToID("_Artifacts");
        internal static readonly int Twirl = Shader.PropertyToID("_Twirl");
        internal static readonly int Falloff = Shader.PropertyToID("_Falloff");
        internal static readonly int Dispersion = Shader.PropertyToID("_Dispersion");
        internal static readonly int DispersionCycles = Shader.PropertyToID("_DispersionCycles");
        internal static readonly int DispersionOffset = Shader.PropertyToID("_DispersionOffset");
        internal static readonly int Dirt = Shader.PropertyToID("_Dirt");
        internal static readonly int DirtFreq = Shader.PropertyToID("_DirtFreq");
        internal static readonly int Aspect = Shader.PropertyToID("_Aspect");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class TextureIDs
      {
        internal static readonly int Source = Shader.PropertyToID("_SourceTex");
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

        material.SetFloat(ShaderIDs.Rays, settings.rays);
        material.SetInt(ShaderIDs.Blend, (int)settings.blend);
        material.SetFloat(ShaderIDs.Gain, settings.gain);
        material.SetFloat(ShaderIDs.Size, settings.size);
        material.SetFloat(ShaderIDs.Threshold, settings.threshold);
        material.SetFloat(ShaderIDs.ThresholdClamp, settings.thresholdClamp);
        material.SetFloat(ShaderIDs.Spin, settings.spin);
        material.SetFloat(ShaderIDs.Twirl, settings.twirl * 1080.0f);
        material.SetFloat(ShaderIDs.Barrel, settings.barrel);
        material.SetFloat(ShaderIDs.BarrelBend, settings.barrelBend);
        material.SetColor(ShaderIDs.Tint, settings.tint);
        material.SetFloat(ShaderIDs.Blur, settings.blur);
        material.SetVector(ShaderIDs.BlurWeights, settings.blurWeights);
        material.SetFloat(ShaderIDs.Artifacts, settings.artifacts);
        material.SetFloat(ShaderIDs.Falloff, settings.falloff);
        material.SetFloat(ShaderIDs.Dispersion, settings.dispersion);
        material.SetFloat(ShaderIDs.DispersionCycles, settings.dispersionCycles);
        material.SetFloat(ShaderIDs.DispersionOffset, settings.dispersionOffset);
        material.SetFloat(ShaderIDs.Dirt, settings.dirt);
        material.SetFloat(ShaderIDs.DirtFreq, settings.dirtFreq);
        material.SetFloat(ShaderIDs.Aspect, settings.aspect);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
      }

#if UNITY_6000_0_OR_NEWER
      private class PassData
      {
        public int nameID;
        public TextureHandle texture;
      }

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
        TextureDesc sourceDesc = source.GetDescriptor(renderGraph);
        sourceDesc.width /= (int)settings.downSample;
        sourceDesc.height /= (int)settings.downSample;

        UpdateMaterial();

        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {
          passData.texture = source;
          passData.nameID = TextureIDs.Source;

          builder.UseTexture(source);
          builder.AllowPassCulling(false);
          builder.AllowGlobalStateModification(true);
          builder.SetRenderFunc((PassData data, RasterGraphContext context) => context.cmd.SetGlobalTexture(data.nameID, data.texture));
        }

        renderTextureHandle0 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle1 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle2 = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandle0, material, 0), $"{Constants.Asset.AssemblyName}.Pass0");
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle0, renderTextureHandle1, material, 1), $"{Constants.Asset.AssemblyName}.Pass1");
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle1, renderTextureHandle2, material, 2), $"{Constants.Asset.AssemblyName}.Pass1");

        resourceData.cameraColor = renderTextureHandle2;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;

        cmd.GetTemporaryRT(renderTextureHandle2, renderTextureDescriptor, settings.filterMode);

        renderTextureDescriptor.width /= (int)settings.downSample;
        renderTextureDescriptor.height /= (int)settings.downSample;

        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
        cmd.GetTemporaryRT(renderTextureHandle1, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.gain <= 0.0f ||
            settings.intensity <= 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.SetGlobalTexture(TextureIDs.Source, colorBuffer);

        cmd.Blit(colorBuffer, renderTextureHandle0, material, 0);
        cmd.Blit(renderTextureHandle0, renderTextureHandle1, material, 1);
        cmd.Blit(renderTextureHandle1, renderTextureHandle2, material, 2);
        cmd.Blit(renderTextureHandle2, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);
        cmd.ReleaseTemporaryRT(renderTextureHandle1);
        cmd.ReleaseTemporaryRT(renderTextureHandle2);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }
#else
#error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
