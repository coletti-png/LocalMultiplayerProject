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

namespace FronkonGames.Artistic.Shockwave
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Shockwave
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

        internal static readonly int Radius = Shader.PropertyToID("_Radius");
        internal static readonly int Center = Shader.PropertyToID("_Center");
        internal static readonly int Strength = Shader.PropertyToID("_Strength");
        internal static readonly int Width = Shader.PropertyToID("_Width");
         internal static readonly int RingWidthInner = Shader.PropertyToID("_RingWidthInner");
         internal static readonly int RingWidthOuter = Shader.PropertyToID("_RingWidthOuter");
         internal static readonly int RingSharpness = Shader.PropertyToID("_RingSharpness");
         internal static readonly int RingSkew = Shader.PropertyToID("_RingSkew");
        internal static readonly int ChromaticAberration = Shader.PropertyToID("_ChromaticAberration");
        internal static readonly int ColorStrength = Shader.PropertyToID("_ColorStrength");
        internal static readonly int ShockwaveColorBlend = Shader.PropertyToID("_ShockwaveColorBlend");
        internal static readonly int InsideTint = Shader.PropertyToID("_InsideTint");
        internal static readonly int Flares = Shader.PropertyToID("_Flares");
        internal static readonly int FlaresColorBlend = Shader.PropertyToID("_FlaresColorBlend");
        internal static readonly int FlaresColor = Shader.PropertyToID("_FlaresColor");
        internal static readonly int FlaresFrequency = Shader.PropertyToID("_FlaresFrequency");
        internal static readonly int FlaresSpeed = Shader.PropertyToID("_FlaresSpeed");
        internal static readonly int FlaresThreshold = Shader.PropertyToID("_FlaresThreshold");
        internal static readonly int FlaresSoftness = Shader.PropertyToID("_FlaresSoftness");
        internal static readonly int Noise = Shader.PropertyToID("_Noise");
        internal static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        internal static readonly int NoiseSpeed = Shader.PropertyToID("_NoiseSpeed");
        internal static readonly int Edge = Shader.PropertyToID("_Edge");
        internal static readonly int EdgeColorBlend = Shader.PropertyToID("_EdgeColorBlend");
        internal static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
         internal static readonly int EdgeWidth = Shader.PropertyToID("_EdgeWidth");
         internal static readonly int EdgeNoise = Shader.PropertyToID("_EdgeNoise");
         internal static readonly int EdgeNoiseScale = Shader.PropertyToID("_EdgeNoiseScale");
         internal static readonly int EdgeNoiseSpeed = Shader.PropertyToID("_EdgeNoiseSpeed");
         internal static readonly int EdgePlasma = Shader.PropertyToID("_EdgePlasma");
         internal static readonly int EdgePlasmaScale = Shader.PropertyToID("_EdgePlasmaScale");
         internal static readonly int EdgePlasmaSpeed = Shader.PropertyToID("_EdgePlasmaSpeed");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
        internal static readonly int ColorBlend = Shader.PropertyToID("_ColorBlend");
        internal static readonly int HueVar = Shader.PropertyToID("_HueVar");
        internal static readonly int HueVarSpeed = Shader.PropertyToID("_HueVarSpeed");
        internal static readonly int HueVarScale = Shader.PropertyToID("_HueVarScale");
         internal static readonly int HueVarRadial = Shader.PropertyToID("_HueVarRadial");
         internal static readonly int HueVarRadialScale = Shader.PropertyToID("_HueVarRadialScale");
      }

      private static class Keywords
      {
        internal static readonly string ChromaticAberration = "CHROMATIC_ABERRATION_ON";
        internal static readonly string Flares = "FLARES_ON";
        internal static readonly string Noise = "NOISE_ON";
        internal static readonly string Edge = "EDGE_ON";
        internal static readonly string HueVariation = "HUE_VARIATION_ON";
         internal static readonly string EdgeNoise = "EDGE_NOISE_ON";
         internal static readonly string EdgePlasma = "EDGE_PLASMA_ON";
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

        material.SetFloat(ShaderIDs.Radius, settings.radius);
        material.SetVector(ShaderIDs.Center, settings.center);
        material.SetFloat(ShaderIDs.Strength, settings.strength);
        material.SetFloat(ShaderIDs.Width, settings.width);
        material.SetFloat(ShaderIDs.RingWidthInner, settings.ringWidthInner);
        material.SetFloat(ShaderIDs.RingWidthOuter, settings.ringWidthOuter);
        material.SetFloat(ShaderIDs.RingSharpness, settings.ringSharpness);
        material.SetFloat(ShaderIDs.RingSkew, settings.ringSkew);

        if (settings.chromaticAberration != Vector3.zero)
        {
          material.EnableKeyword(Keywords.ChromaticAberration);
          material.SetVector(ShaderIDs.ChromaticAberration, settings.chromaticAberration * 100.0f);
        }

        material.SetColor(ShaderIDs.InsideTint, settings.insideTint);

        material.SetVector(ShaderIDs.ColorStrength, settings.colorStrength);
        material.SetInt(ShaderIDs.ShockwaveColorBlend, (int)settings.shockwaveColorBlend);

        if (settings.flares > 0.0f)
        {
          material.EnableKeyword(Keywords.Flares);
          material.SetFloat(ShaderIDs.Flares, settings.flares);
          material.SetInt(ShaderIDs.FlaresColorBlend, (int)settings.flaresColorBlend);
          material.SetColor(ShaderIDs.FlaresColor, settings.flaresColor);
          material.SetFloat(ShaderIDs.FlaresFrequency, settings.flaresFrequency);
          material.SetFloat(ShaderIDs.FlaresSpeed, settings.flaresSpeed);
          material.SetFloat(ShaderIDs.FlaresThreshold, settings.flaresThreshold);
          material.SetFloat(ShaderIDs.FlaresSoftness, settings.flaresSoftness);
        }

        if (settings.noise > 0.0f)
        {
          material.EnableKeyword(Keywords.Noise);
          material.SetFloat(ShaderIDs.Noise, settings.noise);
          material.SetFloat(ShaderIDs.NoiseScale, settings.noiseScale);
          material.SetFloat(ShaderIDs.NoiseSpeed, settings.noiseSpeed);
        }

        if (settings.edge > 0.0f)
        {
          material.EnableKeyword(Keywords.Edge);
          material.SetFloat(ShaderIDs.Edge, settings.edge);
          material.SetInt(ShaderIDs.EdgeColorBlend, (int)settings.edgeColorBlend);
          material.SetColor(ShaderIDs.EdgeColor, settings.edgeColor);
          material.SetFloat(ShaderIDs.EdgeWidth, settings.edgeWidth);

          if (settings.hueVariation > 0.0f || settings.hueVariationRadial > 0.0f)
          {
            material.EnableKeyword(Keywords.HueVariation);
            material.SetFloat(ShaderIDs.HueVar, settings.hueVariation);
            material.SetFloat(ShaderIDs.HueVarSpeed, settings.hueVariationSpeed);
            material.SetFloat(ShaderIDs.HueVarScale, settings.hueVariationScale * 0.5f);
            material.SetFloat(ShaderIDs.HueVarRadial, settings.hueVariationRadial);
            material.SetFloat(ShaderIDs.HueVarRadialScale, settings.hueVariationRadialScale);
          }

          if (settings.edgeNoise > 0.0f)
          {
            material.EnableKeyword(Keywords.EdgeNoise);
            material.SetFloat(ShaderIDs.EdgeNoise, settings.edgeNoise * 5.0f);
            material.SetFloat(ShaderIDs.EdgeNoiseScale, settings.edgeNoiseScale);
            material.SetFloat(ShaderIDs.EdgeNoiseSpeed, settings.edgeNoiseSpeed);
          }

          if (settings.edgePlasma > 0.0f)
          {
            material.EnableKeyword(Keywords.EdgePlasma);
            material.SetFloat(ShaderIDs.EdgePlasma, settings.edgePlasma * 0.1f);
            material.SetFloat(ShaderIDs.EdgePlasmaScale, settings.edgePlasmaScale * 0.1f);
            material.SetFloat(ShaderIDs.EdgePlasmaSpeed, settings.edgePlasmaSpeed * 0.1f);
          }
        }

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
        material.SetInt(ShaderIDs.ColorBlend, (int)settings.colorBlend);
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
