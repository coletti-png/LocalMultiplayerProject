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

namespace FronkonGames.Artistic.Photo
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Photo
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
        internal static readonly int Center             = Shader.PropertyToID("_Center");
        internal static readonly int Focus              = Shader.PropertyToID("_Focus");
        internal static readonly int FocusOffset        = Shader.PropertyToID("_FocusOffset");
        internal static readonly int Blur               = Shader.PropertyToID("_Blur");
        internal static readonly int Grid               = Shader.PropertyToID("_Grid");
        internal static readonly int GridColor          = Shader.PropertyToID("_GridColor");
        internal static readonly int GridColorBlend     = Shader.PropertyToID("_GridColorBlend");
        internal static readonly int Rings              = Shader.PropertyToID("_Rings");
        internal static readonly int RingsColor         = Shader.PropertyToID("_RingsColor");
        internal static readonly int RingsColorBlend    = Shader.PropertyToID("_RingsColorBlend");
        internal static readonly int RingsThickness     = Shader.PropertyToID("_RingsThickness");
        internal static readonly int RingsSharpness     = Shader.PropertyToID("_RingsSharpness");
        internal static readonly int Ring2Scale         = Shader.PropertyToID("_Ring2Scale");
        internal static readonly int Ring1Scale         = Shader.PropertyToID("_Ring1Scale");
        internal static readonly int Ring0Scale         = Shader.PropertyToID("_Ring0Scale");
        internal static readonly int RingSplitScale     = Shader.PropertyToID("_RingSplitScale");
        internal static readonly int Frost              = Shader.PropertyToID("_Frost");
        internal static readonly int FrostColor         = Shader.PropertyToID("_FrostColor");
        internal static readonly int FrostColorBlend    = Shader.PropertyToID("_FrostColorBlend");
        internal static readonly int Aberration         = Shader.PropertyToID("_Aberration");
        internal static readonly int AberrationChannels = Shader.PropertyToID("_AberrationChannels");
        internal static readonly int VignetteSize       = Shader.PropertyToID("_VignetteSize");
        internal static readonly int VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
        internal static readonly int VignetteAspect     = Shader.PropertyToID("_VignetteAspect");
        internal static readonly int Grain              = Shader.PropertyToID("_Grain");
        internal static readonly int Halation           = Shader.PropertyToID("_Halation");
        internal static readonly int ExpiredYears       = Shader.PropertyToID("_ExpiredYears");
        internal static readonly int ChromaticFringing  = Shader.PropertyToID("_ChromaticFringing");
        internal static readonly int Dust               = Shader.PropertyToID("_Dust");
        internal static readonly int DustSize           = Shader.PropertyToID("_DustSize");
        internal static readonly int LightLeak          = Shader.PropertyToID("_LightLeak");
        internal static readonly int LightLeakSpeed     = Shader.PropertyToID("_LightLeakSpeed");
        internal static readonly int ColorBleed         = Shader.PropertyToID("_ColorBleed");
        internal static readonly int ColorBleedAmount   = Shader.PropertyToID("_ColorBleedAmount");
        internal static readonly int ApertureSize       = Shader.PropertyToID("_ApertureSize");
        internal static readonly int ApertureBlades     = Shader.PropertyToID("_ApertureBlades");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast   = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma      = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue        = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        internal static readonly string VignetteRounded     = "VIGNETTE_ROUNDED";
        internal static readonly string VignetteRectangular = "VIGNETTE_RECTANGULAR";
        internal static readonly string VignetteCircular    = "VIGNETTE_CIRCULAR";

        internal static readonly string FilmAgfaVista400       = "FILM_AFGA_VISTA_400";
        internal static readonly string FilmPolaroid600        = "FILM_POLAROID_600";
        internal static readonly string FilmKodakGold200       = "FILM_KODAK_GOLD_200";
        internal static readonly string FilmKodakPortra400     = "FILM_KODAK_PORTRA_400";
        internal static readonly string FilmKodakEktar100      = "FILM_KODAK_EKTAR_100";
        internal static readonly string FilmFujiC200           = "FILM_FUJI_C200";
        internal static readonly string FilmFujiVelvia50       = "FILM_FUJI_VELVIA_50";
        internal static readonly string FilmFujiPro400H        = "FILM_FUJI_PRO_400H";
        internal static readonly string FilmCinestill800T      = "FILM_CINESTILL_800T";
        internal static readonly string FilmLomographyColor800 = "FILM_LOMOGRAPHY_COLOR_800";
        internal static readonly string FilmORWOUT18           = "FILM_ORWO_UT18";
        internal static readonly string FilmIlfordHP5BW        = "FILM_ILFORD_HP5_BW";

        internal static readonly string GlitchChromaticFringing = "GLITCH_CHROMATIC_FRINGING";
        internal static readonly string GlitchDust              = "GLITCH_DUST";
        internal static readonly string GlitchLightLeak         = "GLITCH_LIGHT_LEAK";
        internal static readonly string GlitchColorBleed        = "GLITCH_COLOR_BLEED";

        internal static readonly string Aperture = "APERTURE";
      }

      private static class TextureIDs
      {
        internal static readonly int BlurTexture = Shader.PropertyToID("_BlurTex");
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

        material.SetVector(ShaderIDs.Center, new Vector2(settings.center.x, 1.0f - settings.center.y));
        material.SetFloat(ShaderIDs.Focus, settings.focus);
        material.SetFloat(ShaderIDs.FocusOffset, settings.focusOffset * 50.0f);

        material.SetFloat(ShaderIDs.Grid, settings.grid * 2.0f);
        material.SetColor(ShaderIDs.GridColor, settings.gridColor);
        material.SetInt(ShaderIDs.GridColorBlend, (int)settings.gridColorBlend);

        material.SetFloat(ShaderIDs.Rings, settings.rings);
        material.SetColor(ShaderIDs.RingsColor, settings.ringsColor);
        material.SetInt(ShaderIDs.RingsColorBlend, (int)settings.ringsColorBlend);
        material.SetFloat(ShaderIDs.RingsThickness, settings.ringsThickness * 0.01f);
        material.SetFloat(ShaderIDs.RingsSharpness, settings.ringsSharpness * 64.0f);
        material.SetFloat(ShaderIDs.Ring0Scale, 2.0f - settings.ring1Scale);
        material.SetFloat(ShaderIDs.Ring1Scale, 2.0f - settings.ring2Scale);
        material.SetFloat(ShaderIDs.Ring2Scale, 2.0f - settings.ring3Scale);
        material.SetFloat(ShaderIDs.RingSplitScale, 2.0f - settings.ringSplitScale);

        material.SetFloat(ShaderIDs.Frost, settings.frost);
        material.SetColor(ShaderIDs.FrostColor, settings.frostColor);
        material.SetInt(ShaderIDs.FrostColorBlend, (int)settings.frostColorBlend);

        material.SetFloat(ShaderIDs.Aberration, settings.aberration * 100.0f);
        material.SetVector(ShaderIDs.AberrationChannels, settings.aberrationChannels);

        switch (settings.vignette)
        {
          case Vignettes.Rectangular: material.EnableKeyword(Keywords.VignetteRectangular); break;
          case Vignettes.Circular:    material.EnableKeyword(Keywords.VignetteCircular); break;
        }

        if (settings.apertureSize < 1.0f)
        {
          material.EnableKeyword(Keywords.Aperture);
          material.SetFloat(ShaderIDs.ApertureSize, settings.apertureSize);
          material.SetInt(ShaderIDs.ApertureBlades, settings.apertureBlades);
        }

        if (settings.vignette != Vignettes.None)
          {
            material.SetFloat(ShaderIDs.VignetteSize, settings.vignetteSize);
            material.SetFloat(ShaderIDs.VignetteSmoothness, settings.vignetteSmoothness);
            material.SetFloat(ShaderIDs.VignetteAspect, settings.vignetteAspect);
          }

        switch (settings.film)
        {
          case Films.Agfa_Vista_400:       material.EnableKeyword(Keywords.FilmAgfaVista400); break;
          case Films.Polaroid_600:         material.EnableKeyword(Keywords.FilmPolaroid600); break;
          case Films.Kodak_Gold_200:       material.EnableKeyword(Keywords.FilmKodakGold200); break;
          case Films.Kodak_Portra_400:     material.EnableKeyword(Keywords.FilmKodakPortra400); break;
          case Films.Kodak_Ektar_100:      material.EnableKeyword(Keywords.FilmKodakEktar100); break;
          case Films.Fuji_C200:            material.EnableKeyword(Keywords.FilmFujiC200); break;
          case Films.Fuji_Velvia_50:       material.EnableKeyword(Keywords.FilmFujiVelvia50); break;
          case Films.Fuji_Pro_400H:        material.EnableKeyword(Keywords.FilmFujiPro400H); break;
          case Films.Lomography_Color_800: material.EnableKeyword(Keywords.FilmLomographyColor800); break;
          case Films.ORWO_UT18:            material.EnableKeyword(Keywords.FilmORWOUT18); break;
          case Films.Cinestill_800T:
            material.EnableKeyword(Keywords.FilmCinestill800T);
            material.SetFloat(ShaderIDs.Halation, settings.halation);
            break;
          case Films.Ilford_HP5_BW:        material.EnableKeyword(Keywords.FilmIlfordHP5BW); break;
        }

        if (settings.chromaticFringing > 0.0f)
        {
          material.EnableKeyword(Keywords.GlitchChromaticFringing);
          material.SetFloat(ShaderIDs.ChromaticFringing, settings.chromaticFringing);
        }

        if (settings.dust > 0.0f)
        {
          material.EnableKeyword(Keywords.GlitchDust);
          material.SetFloat(ShaderIDs.Dust, settings.dust);
          material.SetFloat(ShaderIDs.DustSize, settings.dustSize);
        }

        if (settings.lightLeak > 0.0f)
        {
          material.EnableKeyword(Keywords.GlitchLightLeak);
          material.SetFloat(ShaderIDs.LightLeak, settings.lightLeak * 0.25f);
          material.SetFloat(ShaderIDs.LightLeakSpeed, settings.lightLeakSpeed);
        }

        if (settings.colorBleed > 0.0f && settings.colorBleedAmount > 0.0f)
        {
          material.EnableKeyword(Keywords.GlitchColorBleed);
          material.SetFloat(ShaderIDs.ColorBleed, settings.colorBleed);
          material.SetFloat(ShaderIDs.ColorBleedAmount, settings.colorBleedAmount);
        }

        material.SetFloat(ShaderIDs.ExpiredYears, settings.expiredYears);

        material.SetInt(ShaderIDs.Blur, settings.blur);
        material.SetFloat(ShaderIDs.Grain, settings.grain);

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
        if (material == null)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureDesc sourceDesc = source.GetDescriptor(renderGraph);

        UpdateMaterial();

        renderTextureHandle0 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle1 = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandle2 = renderGraph.CreateTexture(sourceDesc);

        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandle0, material, 0), $"{Constants.Asset.AssemblyName}.Pass0");
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle0, renderTextureHandle1, material, 1), $"{Constants.Asset.AssemblyName}.Pass1");

        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {
          passData.texture = renderTextureHandle1;
          passData.nameID = TextureIDs.BlurTexture;

          builder.UseTexture(renderTextureHandle1);
          builder.AllowPassCulling(false);
          builder.AllowGlobalStateModification(true);
          builder.SetRenderFunc((PassData data, RasterGraphContext context) => context.cmd.SetGlobalTexture(data.nameID, data.texture));
        }

        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandle2, material, 2), $"{Constants.Asset.AssemblyName}.Pass2");

        resourceData.cameraColor = renderTextureHandle2;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
        cmd.GetTemporaryRT(renderTextureHandle1, renderTextureDescriptor, settings.filterMode);
        cmd.GetTemporaryRT(renderTextureHandle2, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.Blit(colorBuffer, renderTextureHandle0, material, 0);
        cmd.Blit(renderTextureHandle0, renderTextureHandle1, material, 1);

        cmd.SetGlobalTexture(TextureIDs.BlurTexture, renderTextureHandle1);

        cmd.Blit(colorBuffer, renderTextureHandle2, material, 2);
        cmd.Blit(renderTextureHandle2, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);
        cmd.ReleaseTemporaryRT(renderTextureHandle1);
        cmd.ReleaseTemporaryRT(renderTextureHandle2);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd)
      {
        cmd.ReleaseTemporaryRT(renderTextureHandle0);
        cmd.ReleaseTemporaryRT(renderTextureHandle1);
        cmd.ReleaseTemporaryRT(renderTextureHandle2);
      }
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
