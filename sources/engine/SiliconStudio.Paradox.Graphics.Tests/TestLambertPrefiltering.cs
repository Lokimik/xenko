﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System.Threading.Tasks;

using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Effects;
using SiliconStudio.Paradox.Effects.Cubemap;
using SiliconStudio.Paradox.Effects.Images;
using SiliconStudio.Paradox.Effects.Images.Cubemap;
using SiliconStudio.Paradox.Games;
using SiliconStudio.Paradox.Input;

namespace SiliconStudio.Paradox.Graphics.Tests
{
    public class TestLambertPrefiltering : Game
    {
        private SpriteBatch spriteBatch;

        private ImageEffectContext imageEffectContext;

        private Texture inputCubemap;

        private Texture displayedCubemap;

        private LambertianPrefiltering lamberFilter;

        private Texture outputCubemap;

        private bool shouldPrefilter = true;

        private Int2 screenSize = new Int2(1200, 900);

        private Effect cubemapSpriteEffect;

        public TestLambertPrefiltering()
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = screenSize.X;
            GraphicsDeviceManager.PreferredBackBufferHeight = screenSize.Y;
            GraphicsDeviceManager.PreferredGraphicsProfile = new[] { GraphicsProfile.Level_11_0 };
        }

        protected override async Task LoadContent()
        {
            await base.LoadContent();

            cubemapSpriteEffect = EffectSystem.LoadEffect("CubemapSprite");

            imageEffectContext = new ImageEffectContext(this);
            lamberFilter = new LambertianPrefiltering(imageEffectContext);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            inputCubemap = Asset.Load<Texture>("CubeMap");
            outputCubemap = Texture.NewCube(GraphicsDevice, 256, 1, PixelFormat.R16G16B16A16_UNorm, TextureFlags.RenderTarget | TextureFlags.ShaderResource).DisposeBy(this);
            displayedCubemap = outputCubemap;

            RenderSystem.Pipeline.Renderers.Add(new RenderTargetSetter(Services));
            RenderSystem.Pipeline.Renderers.Add(new DelegateRenderer(Services) { Render = RenderCubeMap });
            RenderSystem.Pipeline.Renderers.Add(new DelegateRenderer(Services) { Render = PrefilterCubeMap });
        }

        private void PrefilterCubeMap(RenderContext obj)
        {
            if (!shouldPrefilter)
                return;

            lamberFilter.SetInput(0, inputCubemap);
            lamberFilter.SetOutput(outputCubemap);
            lamberFilter.Draw();

            //shouldPrefilter = false;
            //displayedCubemap = outputCubemap;
        }

        private void RenderCubeMap(RenderContext obj)
        {
            if (displayedCubemap == null || spriteBatch == null)
                return;

            var size = new Vector2(screenSize.X / 4f, screenSize.Y / 3f);
            
            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 1);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(0, size.Y, size.X, size.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 2);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(size.X, 0f, size.X, size.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 4);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(size.X, size.Y, size.X, size.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 3);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(size.X, 2f * size.Y, size.X, size.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 0);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(2f * size.X, size.Y, size.X, size.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.Parameters.Set(ComputeColorSpriteKeys.ViewIndex, 5);
            spriteBatch.Begin(SpriteSortMode.Texture, cubemapSpriteEffect);
            spriteBatch.Draw(displayedCubemap, new RectangleF(3f * size.X, size.Y, size.X, size.Y), Color.White);
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.IsKeyPressed(Keys.Space))
                shouldPrefilter = true;

            if (Input.IsKeyPressed(Keys.I))
                displayedCubemap = inputCubemap;

            if (Input.IsKeyPressed(Keys.O))
                displayedCubemap = outputCubemap;
        }

        public static void Main()
        {
            using (var game = new TestLambertPrefiltering())
                game.Run();
        }
    }
}