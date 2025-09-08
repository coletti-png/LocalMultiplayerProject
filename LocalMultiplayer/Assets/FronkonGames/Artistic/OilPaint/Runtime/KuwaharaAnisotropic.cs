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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

namespace FronkonGames.Artistic.OilPaint
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary>
  /// 
  /// Kuwahara Anisotropic.
  /// 
  /// 🕹️ Documentation: https://fronkongames.github.io/store/artistic.html
  /// 📄 Demo:          https://fronkongames.github.io/demos-artistic/oilpaint/
  /// 📧 Support:       fronkongames@gmail.com
  /// ❤️ More assets:   https://assetstore.unity.com/publishers/62716
  /// 
  /// 💡 Do you want to report an error? Please, send the log file along with the mail.
  /// 
  /// ❤️ Leave a review if you found this asset useful, thanks! ❤️
  ///
  /// </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  [DisallowMultipleRendererFeature("Oil Paint: Kuwahara Anisotropic")]
  public sealed class KuwaharaAnisotropic : ScriptableRendererFeature
  {
    // MUST be named "settings" (lowercase) to be shown in the Render Features inspector.
    public Settings settings = new();

    private RenderPass renderPass;

    private Material material;

    /// <summary> Initializes this feature's resources. </summary>
    public override void Create() => renderPass ??= new RenderPass(settings);

    /// <summary> Injects one or multiple ScriptableRenderPass in the renderer. Called every frame once per camera. </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
      if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
        return;

      renderPass.renderPassEvent = settings.whenToInsert;
      if (material == null)
      {
        string shaderPath = $"Shaders/KuwaharaAnisotropic_URP";
        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader != null)
        {
          if (shader.isSupported == true)
            material = CoreUtils.CreateEngineMaterial(shader);
          else
            Log.Warning($"'{shaderPath}.shader' not supported");
        }
      }

      renderPass.material = material;
      renderer.EnqueuePass(renderPass);
    }

    protected override void Dispose(bool disposing) => CoreUtils.Destroy(material);

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
      #region Kuwahara Anisotropic settings.

      internal const int MaxPasses = 4;

      /// <summary> [1, MaxPasses]. Default 1. </summary>
      public int passes = 1;

      /// <summary> Size of the kuwahara filter kernel [1, 20]. Default 10. </summary>
      public int radius = 10;

      /// <summary> Size of the gaussian blur kernel for eigenvectors [1, 6]. Default 2. </summary>
      public int blur = 2;

      /// <summary> Adjusts sharpness of the color segments [0, 18]. Default 8. </summary>
      public float sharpness = 8.0f;

      /// <summary> Adjusts hardness of the color segments [1, 100]. Default 8. </summary>
      public float hardness = 8.0f;

      /// <summary> How extreme the angle of the kernel is [0.01, 2]. Default 1. </summary>
      public float alpha = 1.0f;

      /// <summary> How much sectors overlap with each other [0.25, 2]. Default 0.58. </summary>
      public float zeroCrossing = 0.58f;

      /// <summary> Detail algorithm used: None (default), Sharpen or Emboss. </summary>
      public OilPaint.Detail detail = OilPaint.Detail.None;

      /// <summary> Strength of the detail [0, 1]. Default 1. Only valid if Detail it not None. </summary>
      public float detailStrength = 1.0f;

      /// <summary> Strength of the emboss effect [0, 20]. Default 5. Only valid if Detail it Emboss. </summary>
      public float embossStrength = 5.0f;

      /// <summary> Angle of the emboss effect [0, 360]. Default 0. Only valid if Detail it Emboss. </summary>
      public float embossAngle = 0.0f;

      /// <summary> Strength of the Water Color effect [0, 1]. Default 0. </summary>
      public float waterColor = 0.0f;

      /// <summary> Activate the use of the depth buffer to improve the effect. Default False. </summary>
      /// <remarks> The camera must have the 'Depth Texture' field set to 'On'. </remarks>
      public bool processDepth = false;

      /// <summary> Change rate at which kernel sizes change between depths. Only valid if process depth is on. </summary>
      public AnimationCurve depthCurve = OilPaint.DefaultDepthCurve;

      /// <summary> Factor to concentrate the depth curve [0, 1]. Default 1. </summary>
      public float depthPower = 1.0f;

      /// <summary> Affect the sky? Default True. </summary>
      /// <remarks> In night skies (with stars), it is recommended to deactivate this option. </remarks>
      public bool sampleSky = true;

      /// <summary> To view the depth curve in the Editor. </summary>
      /// <remarks> Only works in the Editor. </remarks>
      public bool viewDepthCurve = false;

      /// <summary> Final render size, if it is less than 1 it will be faster but more blurred [0, 1]. Default 1. </summary>
      public float renderSize = 1.0f;

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

      // Internal use.
      public bool forceCurveTextureUpdate;

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        passes = 1;
        radius = 10;
        blur = 2;
        sharpness = 8.0f;
        hardness = 8.0f;
        alpha = 1.0f;
        zeroCrossing = 0.58f;
        detail = OilPaint.Detail.None;
        detailStrength = 1.0f;
        embossStrength = 5.0f;
        embossAngle = 0.0f;
        waterColor = 0.0f;
        renderSize = 1.0f;
        processDepth = false;
        sampleSky = true;
        depthCurve = new AnimationCurve() { keys = OilPaint.DefaultDepthCurve.keys };
        depthPower = 1.0f;
        viewDepthCurve = false;

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

    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
      private readonly TextureHandle[] renderTextureHandles = new TextureHandle[Settings.MaxPasses];
      private TextureHandle renderTextureHandlesDetail;
      private TextureHandle renderTextureHandleTensor;
      private TextureHandle renderTextureHandleBlurHorizontal;
      private TextureHandle renderTextureHandleBlurVertical;
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int[] renderTextureHandles = new int[Settings.MaxPasses];
      private readonly int renderTextureHandleDetail = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTHDetail");
      private readonly int renderTextureHandleTensor = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTHTensor");
      private readonly int renderTextureHandleBlurHorizontal = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTHBlurHorizontal");
      private readonly int renderTextureHandleBlurVertical = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTHBlurVertical");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      private AnimationCurve curve = null;
      private Texture2D curveTexture;

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int Radius = Shader.PropertyToID("_Radius");
        internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");
        internal static readonly int Hardness = Shader.PropertyToID("_Hardness");
        internal static readonly int Alpha = Shader.PropertyToID("_Alpha");
        internal static readonly int ZeroCrossing = Shader.PropertyToID("_ZeroCrossing");
        internal static readonly int Blur = Shader.PropertyToID("_Blur");
        internal static readonly int DetailStrength = Shader.PropertyToID("_DetailStrength");
        internal static readonly int EmbossStrength = Shader.PropertyToID("_EmbossStrength");
        internal static readonly int EmbossAngle = Shader.PropertyToID("_EmbossAngle");
        internal static readonly int WaterColor = Shader.PropertyToID("_WaterColorStrength");
        internal static readonly int WaterColorBlend = Shader.PropertyToID("_WaterColorBlend");
        internal static readonly int SampleSky = Shader.PropertyToID("_SampleSky");
        internal static readonly int DepthCurve = Shader.PropertyToID("_DepthCurve");
        internal static readonly int DepthPower = Shader.PropertyToID("_DepthPower");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Textures
      {
        internal static readonly int TensorTexture = Shader.PropertyToID("_TensorTex");
        internal static readonly int CurveTexture = Shader.PropertyToID("_CurveTex");
      }

      private static class Keywords
      {
        internal static readonly string DetailSharpen = "DETAIL_SHARPEN";
        internal static readonly string DetailEmboss = "DETAIL_EMBOSS";

        internal static readonly string WaterColor = "WATER_COLOR";

        internal static readonly string ProcessDepth = "PROCESS_DEPTH";
        internal static readonly string DetailPass = "DETAIL_PASS";
        internal static readonly string ViewDepth = "VIEW_DEPTH";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;
#if UNITY_6000_0_OR_NEWER
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#else
        for (int i = 0; i < Settings.MaxPasses; ++i)
          renderTextureHandles[i] = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH{i}");
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      /// <summary> Update curve texture. </summary>
      internal void UpdateCurveTexture()
      {
        curve = settings.depthCurve;

        const int width = 256;
        const int height = 4;
        curveTexture = new Texture2D(width, height, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        const float inv = 1.0f / (width - 1);
        for (int y = 0; y < height; ++y)
        {
          for (int x = 0; x < width; ++x)
            curveTexture.SetPixel(x, y, Color.white * curve.Evaluate(x * inv));
        }

        curveTexture.Apply();

        settings.forceCurveTextureUpdate = false;
      }

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        float renderSize = Mathf.Max(0.01f, settings.renderSize);

        material.SetInt(ShaderIDs.Radius, settings.radius);
        material.SetInt(ShaderIDs.Blur, settings.blur);
        material.SetFloat(ShaderIDs.Sharpness, settings.sharpness);
        material.SetFloat(ShaderIDs.Hardness, settings.hardness);
        material.SetFloat(ShaderIDs.Alpha, settings.alpha);
        material.SetFloat(ShaderIDs.ZeroCrossing, settings.zeroCrossing);

        if (settings.detail != OilPaint.Detail.None || settings.waterColor > 0.0f)
          material.EnableKeyword(Keywords.DetailPass);

        if (settings.processDepth == true)
        {
          material.EnableKeyword(Keywords.ProcessDepth);

          if (settings.forceCurveTextureUpdate == true || curve == null || curve != settings.depthCurve)
            UpdateCurveTexture();

#if UNITY_EDITOR
          if (settings.viewDepthCurve == true)
            material.EnableKeyword(Keywords.ViewDepth);
#endif
          material.SetTexture(Textures.CurveTexture, curveTexture);
          material.SetFloat(ShaderIDs.DepthPower, settings.depthPower * 0.1f);
          material.SetInt(ShaderIDs.SampleSky, settings.sampleSky == true ? 1 : 0);
        }

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
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureDesc sourceDesc = source.GetDescriptor(renderGraph);
        float renderSize = Mathf.Max(0.01f, settings.renderSize);
        sourceDesc.width = (int)(Screen.width * renderSize);
        sourceDesc.height = (int)(Screen.height * renderSize);

        UpdateMaterial();

        int i = 0;
        for (; i < Settings.MaxPasses; ++i)
          renderTextureHandles[i] = renderGraph.CreateTexture(sourceDesc);

        renderTextureHandleTensor = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandleBlurHorizontal = renderGraph.CreateTexture(sourceDesc);
        renderTextureHandleBlurVertical = renderGraph.CreateTexture(sourceDesc);

        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandleTensor, material, 2));
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandleTensor, renderTextureHandleBlurHorizontal, material, 3));
        renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandleBlurHorizontal, renderTextureHandleBlurVertical, material, 4));

        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
        {
          passData.texture = renderTextureHandleBlurVertical;
          passData.nameID = Textures.TensorTexture;

          builder.UseTexture(renderTextureHandleBlurVertical);
          builder.AllowPassCulling(false);
          builder.AllowGlobalStateModification(true);
          builder.SetRenderFunc((PassData data, RasterGraphContext context) => context.cmd.SetGlobalTexture(data.nameID, data.texture));
        }

        for (i = 0; i < settings.passes; ++i)
          renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(i == 0 ? source : renderTextureHandles[i - 1], renderTextureHandles[i], material, 0), $"{Constants.Asset.AssemblyName}.Pass{i}");

        TextureHandle lastRenderTarget = renderTextureHandles[i - 1];

        if (settings.detail != OilPaint.Detail.None || settings.waterColor > 0.0f)
        {
          renderTextureHandlesDetail = renderGraph.CreateTexture(sourceDesc);

          material.SetFloat(ShaderIDs.DetailStrength, settings.detailStrength);

          switch (settings.detail)
          {
            case OilPaint.Detail.None: break;

            case OilPaint.Detail.Sharpen:
              material.EnableKeyword(Keywords.DetailSharpen);
              break;

            case OilPaint.Detail.Emboss:
              material.EnableKeyword(Keywords.DetailEmboss);
              material.SetFloat(ShaderIDs.EmbossStrength, settings.embossStrength);
              material.SetFloat(ShaderIDs.EmbossAngle, settings.embossAngle * Mathf.Deg2Rad);
              break;
          }

          if (settings.waterColor > 0.0f)
          {
            material.EnableKeyword(Keywords.WaterColor);
            material.SetFloat(ShaderIDs.WaterColor, settings.waterColor);
          }

          renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(lastRenderTarget, renderTextureHandlesDetail, material, 1), $"{Constants.Asset.AssemblyName}.PassDetail");
          lastRenderTarget = renderTextureHandlesDetail;
        }

        resourceData.cameraColor = lastRenderTarget;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
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

        float renderSize = Mathf.Max(0.01f, settings.renderSize);
        renderTextureDescriptor.width = (int)(Screen.width * renderSize);
        renderTextureDescriptor.height = (int)(Screen.height * renderSize);

        int i = 0;
        for (; i < Settings.MaxPasses; ++i)
          cmd.GetTemporaryRT(renderTextureHandles[i], renderTextureDescriptor, settings.filterMode);

        cmd.GetTemporaryRT(renderTextureHandleTensor, renderTextureDescriptor, settings.filterMode);
        cmd.GetTemporaryRT(renderTextureHandleBlurHorizontal, renderTextureDescriptor, settings.filterMode);
        cmd.GetTemporaryRT(renderTextureHandleBlurVertical, renderTextureDescriptor, settings.filterMode);

        cmd.Blit(colorBuffer, renderTextureHandleTensor, material, 2);
        cmd.Blit(renderTextureHandleTensor, renderTextureHandleBlurHorizontal, material, 3);
        cmd.Blit(renderTextureHandleBlurHorizontal, renderTextureHandleBlurVertical, material, 4);

        cmd.SetGlobalTexture(Textures.TensorTexture, renderTextureHandleBlurVertical);

        for (i = 0; i < settings.passes; ++i)
          cmd.Blit(i == 0 ? colorBuffer : renderTextureHandles[i - 1], renderTextureHandles[i], material, 0);

        int lastRenderTarget = renderTextureHandles[i - 1];

        if (settings.detail != OilPaint.Detail.None || settings.waterColor > 0.0f)
        {
          cmd.GetTemporaryRT(renderTextureHandleDetail, renderTextureDescriptor, settings.filterMode);

          material.SetFloat(ShaderIDs.DetailStrength, settings.detailStrength);

          switch (settings.detail)
          {
            case OilPaint.Detail.None: break;

            case OilPaint.Detail.Sharpen:
              material.EnableKeyword(Keywords.DetailSharpen);
              break;

            case OilPaint.Detail.Emboss:
              material.EnableKeyword(Keywords.DetailEmboss);
              material.SetFloat(ShaderIDs.EmbossStrength, settings.embossStrength);
              material.SetFloat(ShaderIDs.EmbossAngle, settings.embossAngle * Mathf.Deg2Rad);
              break;
          }

          if (settings.waterColor > 0.0f)
          {
            material.EnableKeyword(Keywords.WaterColor);
            material.SetFloat(ShaderIDs.WaterColor, settings.waterColor);
          }

          cmd.Blit(lastRenderTarget, renderTextureHandleDetail, material, 1);
          cmd.Blit(renderTextureHandleDetail, colorBuffer);

          cmd.ReleaseTemporaryRT(renderTextureHandleDetail);
        }
        else
          cmd.Blit(lastRenderTarget, colorBuffer);

        for (i = 0; i < settings.passes; ++i)
        {
          if (renderTextureHandles[i] != lastRenderTarget)
            cmd.ReleaseTemporaryRT(renderTextureHandles[i]);
        }

        cmd.ReleaseTemporaryRT(lastRenderTarget);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd)
      {
        for (int i = 0; i < settings.passes; ++i)
          cmd.ReleaseTemporaryRT(renderTextureHandles[i]);

        cmd.ReleaseTemporaryRT(renderTextureHandleDetail);
        cmd.ReleaseTemporaryRT(renderTextureHandleTensor);
        cmd.ReleaseTemporaryRT(renderTextureHandleBlurHorizontal);
        cmd.ReleaseTemporaryRT(renderTextureHandleBlurVertical);
      }
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }

    private const string RenderListFieldName = "m_RendererDataList";

    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly KuwaharaAnisotropic[] NoEffects = new KuwaharaAnisotropic[0];

    /// <summary> Is it in the default render pipeline? </summary>
    /// <returns> True / false </returns>
    /// <remarks> This function use Reflection, so it can be slow. </remarks>
    public static bool IsInRenderFeatures() => Instance != null;

    /// <summary> Is it in any renders pipeline? </summary>
    /// <returns> True / false </returns>
    /// <remarks> This function use Reflection, so it can be slow. </remarks>
    public static bool IsInAnyRenderFeatures() => Instances.Length > 0;

    /// <summary> Returns the effect in the default render pipeline or null </summary>
    /// <returns> Effect or null </returns>
    /// <remarks> This function use Reflection, so it can be slow. </remarks>
    public static KuwaharaAnisotropic Instance
    {
      get
      {
        UniversalRenderPipelineAsset pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        if (pipelineAsset != null)
        {
          FieldInfo propertyInfo = pipelineAsset.GetType().GetField(RenderListFieldName, bindingFlags);
          ScriptableRendererData scriptableRendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipelineAsset))?[0];
          for (int i = 0; i < scriptableRendererData.rendererFeatures.Count; ++i)
          {
            if (scriptableRendererData.rendererFeatures[i] is KuwaharaAnisotropic)
              return scriptableRendererData.rendererFeatures[i] as KuwaharaAnisotropic;
          }
        }

        return null;
      }
    }

    /// <summary> Returns an array with all the effects found. </summary>
    /// <returns> Array with effects </returns>
    /// <remarks> This function use Reflection, so it can be slow. </remarks>
    private static KuwaharaAnisotropic[] Instances
    {
      get
      {
        if (UniversalRenderPipeline.asset != null)
        {
          ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset)
            .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(UniversalRenderPipeline.asset);

          List<KuwaharaAnisotropic> effects = new();
          for (int i = 0; i < rendererDataList.Length; ++i)
          {
            if (rendererDataList[i] != null && rendererDataList[i].rendererFeatures.Count > 0)
              foreach (var feature in rendererDataList[i].rendererFeatures)
                if (feature is KuwaharaAnisotropic)
                  effects.Add(feature as KuwaharaAnisotropic);
          }

          return effects.ToArray();
        }

        return NoEffects;
      }
    }
  }
}
