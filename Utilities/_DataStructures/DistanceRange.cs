using System;

namespace TerrariaOverhaul.Utilities;

public struct DistanceRange(float Start, float End, float Exponent = 1f)
{
	public float Start = Start;
	public float End = End;
	public float Exponent = Exponent;

	public readonly float DistanceFactor(float distance)
	{
		if (distance < Start) return 1f;

		float factor = 1f - MathF.Min(1f, MathF.Max(0f, distance - Start) / (End - Start));
		if (!float.IsNormal(factor)) return 0f;

		return MathF.Pow(factor, Exponent);
	}
}
