﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;

namespace TerrariaOverhaul.Utilities;

internal static class WeightedValue
{
	static WeightedValue()
	{
		WeightedValue<double>.FnTable = new() {
			AddT = static (a, b) => a + b,
			MulF = static (a, b) => a * b,
			DivF = static (a, b) => a / b,
		};
		WeightedValue<Vector2D>.FnTable = new() {
			AddT = static (a, b) => a + b,
			MulF = static (a, b) => a * b,
			DivF = static (a, b) => a / b,
		};
	}
}

public struct WeightedValue<T>(T Value, double Weight)
{
	public struct Functions
	{
		public required Func<T, T, T> AddT;
		public required Func<T, double, T> MulF;
		public required Func<T, double, T> DivF;
	}

	public static Functions FnTable { get; set; }

	public T TotalValue { get; private set; } = Value;
	public double TotalWeight { get; private set; } = Weight;
	public double MinWeight { get; init; } = 0d;

	static WeightedValue() => RuntimeHelpers.RunClassConstructor(typeof(WeightedValue).TypeHandle);

	public void Add(T value, double weight)
	{
		TotalWeight += weight;
		TotalValue = FnTable.AddT(TotalValue, FnTable.MulF(value, weight));
	}

	public readonly T Total()
	{
		if (TotalWeight == 0.0)
			return TotalValue;

		return FnTable.DivF(TotalValue, Math.Max(MinWeight, TotalWeight));
	}
}
