// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering.Lights;
using SiliconStudio.Xenko.Shaders;
using Buffer = SiliconStudio.Xenko.Graphics.Buffer;

namespace SiliconStudio.Xenko.Rendering.LightProbes
{
    /// <summary>
    /// Light renderer for clustered shading.
    /// </summary>
    /// <remarks>
    /// Due to the fact that it handles both Point and Spot with a single logic, it doesn't fit perfectly the current logic of one "direct light groups" per renderer.
    /// </remarks>
    public class LightProbeRenderer : LightGroupRendererBase
    {
        private LightProbeShaderGroupData lightprobeGroup;

        private Buffer lightprobeCoefficients;

        public override Type[] LightTypes { get; } = Type.EmptyTypes;

        public LightProbeRenderer()
        {
            IsEnvironmentLight = true;
        }

        public override void Initialize(RenderContext context)
        {
            base.Initialize(context);

            lightprobeGroup = new LightProbeShaderGroupData(context, this);
        }

        public override void Reset()
        {
            base.Reset();

            lightprobeGroup.Reset();
        }

        public override void SetViews(FastList<RenderView> views)
        {
            base.SetViews(views);

            lightprobeGroup.SetViews(views);
        }

        public override void ProcessLights(ProcessLightsParameters parameters)
        {
            lightprobeGroup.AddView(parameters.ViewIndex, parameters.View, parameters.LightIndices.Count);

            foreach(var index in parameters.LightIndices)
            {
                lightprobeGroup.AddLight(parameters.LightCollection[index], null);
            }

            // Consume all the lights
            parameters.LightIndices.Clear();
        }

        public override void UpdateShaderPermutationEntry(ForwardLightingRenderFeature.LightShaderPermutationEntry shaderEntry)
        {
            shaderEntry.EnvironmentLights.Add(lightprobeGroup);
        }

        class LightProbeShaderGroupData : LightShaderGroupDynamic
        {
            private readonly LightProbeRenderer lightProbeRenderer;

            public LightProbeShaderGroupData(RenderContext renderContext, LightProbeRenderer lightProbeRenderer)
                : base(renderContext, null)
            {
                this.lightProbeRenderer = lightProbeRenderer;
                ShaderSource = new ShaderClassSource("LightProbeShader", 3);
            }

            public override unsafe void ApplyViewParameters(RenderDrawContext context, int viewIndex, ParameterCollection parameters)
            {
                // Note: no need to fill CurrentLights since we have no shadow maps
                base.ApplyViewParameters(context, viewIndex, parameters);
            }
        }
    }
}
