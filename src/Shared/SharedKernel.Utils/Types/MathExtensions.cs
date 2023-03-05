namespace System;

/// <summary>
/// Métodos adicionales de las clases <see cref="Math"/> y <see cref="MathF"/>
/// </summary>
public static class MathExt
{
	public static int FloorToInt(double value)
	{
		return (int)Math.Floor(value);
	}

	public static int FloorToInt(decimal value)
	{
		return (int)Math.Floor(value);
	}

	public static int FloorToInt(float value)
	{
		return (int)MathF.Floor(value);
	}

	public static int CeilingToInt(double value)
	{
		return (int)Math.Ceiling(value);
	}

	public static int CeilingToInt(decimal value)
	{
		return (int)Math.Ceiling(value);
	}

	public static int CeilingToInt(float value)
	{
		return (int)MathF.Ceiling(value);
	}
}
