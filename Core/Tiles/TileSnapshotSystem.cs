﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using TerrariaOverhaul.Utilities;

namespace TerrariaOverhaul.Core.Tiles;

[Autoload(Side = ModSide.Client)]
public sealed class TileSnapshotSystem : ModSystem
{
	private static SpriteBatch? spriteBatch;

	public override void Load()
	{

	}

	public override void Unload()
	{
		spriteBatch?.Dispose();
		spriteBatch = null;
	}

	public static RenderTarget2D CreateSpecificTilesSnapshot(Vector2Int sizeInTiles, Vector2Int baseTilePosition, ReadOnlySpan<Vector2Int> tilePositions)
	{
		if (!Program.IsMainThread) {
			throw new InvalidOperationException($"{nameof(CreateSpecificTilesSnapshot)} can only be called on the main thread.");
		}

		var graphicsDevice = Main.graphics.GraphicsDevice;
		var originalRenderTargets = graphicsDevice.GetRenderTargets();

		var textureSize = sizeInTiles * Vector2Int.One * TileUtils.TileSizeInPixels;
		var renderTarget = new RenderTarget2D(graphicsDevice, textureSize.X, textureSize.Y, false, SurfaceFormat.Color, DepthFormat.None);

		graphicsDevice.SetRenderTarget(renderTarget);
		graphicsDevice.Clear(Color.Transparent);

		RenderSpecificTiles(baseTilePosition, tilePositions);

		graphicsDevice.SetRenderTargets(originalRenderTargets);

		return renderTarget;
	}

	public static void RenderSpecificTiles(Vector2Int baseTilePosition, ReadOnlySpan<Vector2Int> tilePositions)
	{
		Main.instance.ClearCachedTileDraws();

		var tileRenderer = Main.instance.TilesRenderer;
		var tileDrawData = new TileDrawInfo();
		var screenOffset = Vector2.Zero;
		var originalZoomFactor = Main.GameViewMatrix.Zoom;
		var originalScreenPosition = Main.screenPosition;
		bool originalGameMenu = Main.gameMenu;

		// Adjust draw position
		Main.screenPosition = baseTilePosition * TileUtils.TileSizeInPixels;
		// Get rid of scaling
		Main.GameViewMatrix.Zoom = Vector2.One;
		// This hack forces Lighting.GetColor to yield with Color.White
		Main.gameMenu = true;

		ClearLegacyCachedDraws(tileRenderer);

		tileRenderer.PreDrawTiles(solidLayer: false, forRenderTargets: true, intoRenderTargets: true);
		Main.spriteBatch.Begin();
		
		for (int i = 0; i < tilePositions.Length; i++) {
			var tilePosition = tilePositions[i];
			var tile = Main.tile[tilePosition.X, tilePosition.Y];

			DrawSingleTile(tileRenderer, tileDrawData, true, -1, Main.screenPosition, screenOffset, tilePosition.X, tilePosition.Y);
		}

		DrawSpecialTilesLegacy(tileRenderer, Main.screenPosition, screenOffset);

		Main.spriteBatch.End();
		tileRenderer.PostDrawTiles(solidLayer: false, forRenderTargets: false, intoRenderTargets: false);

		Main.gameMenu = originalGameMenu;
		Main.GameViewMatrix.Zoom = originalZoomFactor;
		Main.screenPosition = originalScreenPosition;
	}

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern void DrawSingleTile(TileDrawing instance, TileDrawInfo drawData, bool solidLayer, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY);

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern void DrawSpecialTilesLegacy(TileDrawing instance, Vector2 screenPosition, Vector2 offSet);

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern void ClearLegacyCachedDraws(TileDrawing instance);
}
