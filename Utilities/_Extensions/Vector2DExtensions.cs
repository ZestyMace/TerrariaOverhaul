using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;

namespace TerrariaOverhaul.Utilities;

public static class Vector2DExtensions
{
	public static Vector2 ToF32(this Vector2D vec) => new((float)vec.X, (float)vec.Y);
	public static Vector2D ToF64(this Vector2 vec) => new(vec.X, vec.Y);
}
