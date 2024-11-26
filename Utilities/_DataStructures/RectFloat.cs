﻿using Microsoft.Xna.Framework;

namespace TerrariaOverhaul.Utilities;

public struct RectFloat
{
	public static readonly RectFloat Default = new(0f, 0f, 1f, 1f);
	public static readonly RectFloat Empty = new(0f, 0f, 0f, 0f);

	public float X;
	public float Y;
	public float Width;
	public float Height;

	public float Left {
		readonly get => X;
		set => X = value;
	}
	public float Top {
		readonly get => Y;
		set => Y = value;
	}
	public float Right {
		readonly get => X + Width;
		set => X = value - Width;
	}
	public float Bottom {
		readonly get => Y + Height;
		set => Y = value - Height;
	}
	public Vector2 TopLeft {
		readonly get => new(Left, Top);
		set {
			Left = value.X;
			Top = value.Y;
		}
	}
	public Vector2 TopRight {
		readonly get => new(Right, Top);
		set {
			Right = value.X;
			Top = value.Y;
		}
	}
	public Vector2 BottomLeft {
		readonly get => new(Left, Bottom);
		set {
			Left = value.X;
			Bottom = value.Y;
		}
	}
	public Vector2 BottomRight {
		readonly get => new(Right, Bottom);
		set {
			Right = value.X;
			Bottom = value.Y;
		}
	}
	public Vector2 Position {
		readonly get => TopLeft;
		set => TopLeft = value;
	}
	public Vector2 Size {
		readonly get => new(Width, Height);
		set {
			Width = value.X;
			Height = value.Y;
		}
	}
	public Vector4 Points {
		readonly get => new(X, Y, X + Width, Y + Height);
		set {
			X = value.X;
			Y = value.Y;
			Width = value.Z - X;
			Height = value.W - Y;
		}
	}

	public RectFloat(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public readonly override string ToString()
		=> $"[X:{X} Y:{Y} Width:{Width} Height:{Height}]";

	public readonly bool ContainsExclusive(Vector2 point) => point.X > X && point.X < X + Width && point.Y > Y && point.Y < Y + Height;
	public readonly bool ContainsInclusive(Vector2 point) => point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;

	public readonly bool IntersectsExclusive(Rectangle other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;
	public readonly bool IntersectsExclusive(RectFloat other) => other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;
	public readonly bool IntersectsInclusive(Rectangle other) => other.Left <= Right && Left <= other.Right && other.Top <= Bottom && Top <= other.Bottom;
	public readonly bool IntersectsInclusive(RectFloat other) => other.Left <= Right && Left <= other.Right && other.Top <= Bottom && Top <= other.Bottom;

	public static RectFloat FromPoints(Vector4 points)
		=> FromPoints(points.X, points.Y, points.Z, points.W);

	public static RectFloat FromPoints(float x1, float y1, float x2, float y2)
	{
		RectFloat rect;

		rect.X = x1;
		rect.Y = y1;
		rect.Width = x2 - x1;
		rect.Height = y2 - y1;

		return rect;
	}

	public static RectFloat operator *(RectFloat rectF, float mul)
		=> new(rectF.X * mul, rectF.Y * mul, rectF.Width * mul, rectF.Height * mul);

	public static RectFloat operator /(RectFloat rectF, float div)
		=> new(rectF.X / div, rectF.Y / div, rectF.Width / div, rectF.Height / div);

	public static RectFloat operator *(RectFloat rectF, Vector2 mul)
		=> new(rectF.X * mul.X, rectF.Y * mul.Y, rectF.Width * mul.X, rectF.Height * mul.Y);

	public static RectFloat operator /(RectFloat rectF, Vector2 div)
		=> new(rectF.X / div.X, rectF.Y / div.Y, rectF.Width / div.X, rectF.Height / div.Y);

	public static explicit operator RectFloat(Rectangle rectI)
		=> new(rectI.X, rectI.Y, rectI.Width, rectI.Height);

	public static explicit operator Rectangle(RectFloat rectF)
		=> new((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
}
