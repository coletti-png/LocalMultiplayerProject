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

namespace FronkonGames.Artistic.RadialBlur
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class RadialBlur
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

      private RenderTextureDescriptor renderTextureDescriptor;
#if UNITY_6000_0_OR_NEWER
#else
      private RenderTargetIdentifier colorBuffer;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");


      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int Center = Shader.PropertyToID("_Center");
        internal static readonly int Samples = Shader.PropertyToID("_Samples");
        internal static readonly int Distance = Shader.PropertyToID("_Distance");
        internal static readonly int Falloff = Shader.PropertyToID("_Falloff");
        internal static readonly int ChannelsOffset = Shader.PropertyToID("_ChannelsOffset");
        internal static readonly int Fisheye = Shader.PropertyToID("_Fisheye");
        internal static readonly int GradientPower = Shader.PropertyToID("_GradientPower");
        internal static readonly int GradientRangeMin = Shader.PropertyToID("_GradientRangeMin");
        internal static readonly int GradientRangeMax = Shader.PropertyToID("_GradientRangeMax");
        internal static readonly int InnerColor = Shader.PropertyToID("_InnerColor");
        internal static readonly int InnerBrightness = Shader.PropertyToID("_InnerBrightness");
        internal static readonly int InnerContrast = Shader.PropertyToID("_InnerContrast");
        internal static readonly int InnerGamma = Shader.PropertyToID("_InnerGamma");
        internal static readonly int InnerHue = Shader.PropertyToID("_InnerHue");
        internal static readonly int InnerSaturation = Shader.PropertyToID("_InnerSaturation");
        internal static readonly int OuterColor = Shader.PropertyToID("_OuterColor");
        internal static readonly int OuterBrightness = Shader.PropertyToID("_OuterBrightness");
        internal static readonly int OuterContrast = Shader.PropertyToID("_OuterContrast");
        internal static readonly int OuterGamma = Shader.PropertyToID("_OuterGamma");
        internal static readonly int OuterHue = Shader.PropertyToID("_OuterHue");
        internal static readonly int OuterSaturation = Shader.PropertyToID("_OuterSaturation");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;

#if UNITY_6000_0_OR_NEWER
        renderTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        material.SetVector(ShaderIDs.Center, settings.center);
        material.SetInt(ShaderIDs.Samples, settings.samples);
        material.SetFloat(ShaderIDs.Distance, 1.0f - settings.density);
        material.SetFloat(ShaderIDs.Falloff, settings.falloff);
        material.SetVector(ShaderIDs.ChannelsOffset, settings.channelsOffset);
        material.SetFloat(ShaderIDs.Fisheye, settings.fishEye);

        material.SetFloat(ShaderIDs.GradientPower, settings.gradientPower);
        material.SetFloat(ShaderIDs.GradientRangeMin, settings.gradientRangeMin);
        material.SetFloat(ShaderIDs.GradientRangeMax, settings.gradientRangeMax);

        material.SetColor(ShaderIDs.InnerColor, settings.innerColor);
        material.SetFloat(ShaderIDs.InnerBrightness, settings.innerBrightness);
        material.SetFloat(ShaderIDs.InnerContrast, settings.innerContrast);
        material.SetFloat(ShaderIDs.InnerGamma, 1.0f / settings.innerGamma);
        material.SetFloat(ShaderIDs.InnerHue, settings.innerHue);
        material.SetFloat(ShaderIDs.InnerSaturation, settings.innerSaturation);

        material.SetColor(ShaderIDs.OuterColor, settings.outerColor);
        material.SetFloat(ShaderIDs.OuterBrightness, settings.outerBrightness);
        material.SetFloat(ShaderIDs.OuterContrast, settings.outerContrast);
        material.SetFloat(ShaderIDs.OuterGamma, 1.0f / settings.outerGamma);
        material.SetFloat(ShaderIDs.OuterHue, settings.outerHue);
        material.SetFloat(ShaderIDs.OuterSaturation, settings.outerSaturation);

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
            settings.intensity <= 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.Blit(colorBuffer, renderTextureHandle0, material);
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
