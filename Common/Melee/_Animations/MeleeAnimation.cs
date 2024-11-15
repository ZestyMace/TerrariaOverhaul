﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaOverhaul.Core.Configuration;
using TerrariaOverhaul.Core.Debugging;
using TerrariaOverhaul.Core.ItemComponents;
using TerrariaOverhaul.Utilities;

namespace TerrariaOverhaul.Common.Melee;

#pragma warning disable IDE0060 // Remove unused parameter

public abstract class MeleeAnimation : ItemComponent
{
	public static readonly ConfigEntry<bool> EnableImprovedMeleeAnimations = new(ConfigSide.ClientOnly, true, "Visuals");

	public abstract float GetItemRotation(Player player, Item item);

	protected virtual void ApplyAnimation(Item item, Player player)
	{
		if (!Enabled || !EnableImprovedMeleeAnimations) {
			return;
		}

		if (item.useStyle != ItemUseStyleID.Swing) {
			return;
		}

		float animationRotation = GetItemRotation(player, item);
		float weaponRotation = MathUtils.Modulo(animationRotation, MathHelper.TwoPi);
		float pitch = MathUtils.RadiansToPitch(weaponRotation);
		var weaponDirection = weaponRotation.ToRotationVector2();

		if (Math.Sign(weaponDirection.X) != player.direction) {
			pitch = weaponDirection.Y < 0f ? 1f : 0f;
		}

		player.bodyFrame = PlayerFrames.Use3.ToRectangle();

		Vector2 locationOffset;

		if (pitch > 0.95f) {
			player.bodyFrame = PlayerFrames.Use1.ToRectangle();
			locationOffset = new Vector2(-8f, -9f);
		} else if (pitch > 0.7f) {
			player.bodyFrame = PlayerFrames.Use2.ToRectangle();
			locationOffset = new Vector2(4f, -8f);
		} else if (pitch > 0.3f) {
			player.bodyFrame = PlayerFrames.Use3.ToRectangle();
			locationOffset = new Vector2(4f, 2f);
		} else if (pitch > 0.05f) {
			player.bodyFrame = PlayerFrames.Use4.ToRectangle();
			locationOffset = new Vector2(4f, 7f);
		} else {
			player.bodyFrame = PlayerFrames.Walk5.ToRectangle();
			locationOffset = new Vector2(-8f, 2f);
		}

		player.itemRotation = weaponRotation + MathHelper.PiOver4;

		if (player.direction < 0) {
			player.itemRotation += MathHelper.PiOver2;
		}

		player.itemLocation = player.Center + new Vector2(locationOffset.X * player.direction, locationOffset.Y);

		if (!Main.dedServ && DebugSystem.EnableDebugRendering) {
			DebugSystem.DrawCircle(player.itemLocation, 3f, Color.White);
		}
	}

	public sealed override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
	{
		TryApplyAnimation(player.HeldItem, player);
	}

	public sealed override void UseItemFrame(Item item, Player player)
	{
		TryApplyAnimation(player.HeldItem, player);
	}

	private void TryApplyAnimation(Item item, Player player)
	{
		var heldItem = player.HeldItem;

		if (heldItem.TryGetGlobalItem(this, out var correctInstance)) {
			correctInstance.ApplyAnimation(heldItem, player);
		}
	}
}
