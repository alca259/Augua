namespace System
{
	public static class NumberExtensions
	{
		/// <summary>
		/// Intenta igualar recortando
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="epsilon">0.01F</param>
		/// <returns></returns>
		public static bool Equals(this float a, float b, float epsilon)
		{
			float absA = Math.Abs(a);
			float absB = Math.Abs(b);
			float diff = Math.Abs(a - b);

			if (a == b)
			{
				// Shortcut, handles infinities
				return true;
			}

			if (a == 0.0f || b == 0.0f || diff < float.Epsilon)
			{
				// a or b is zero, or both are extremely close to it.
				// relative error is less meaningful here
				return diff < epsilon;
			}

			// use relative error
			return diff / Math.Min((absA + absB), float.MaxValue) < epsilon;
		}

		/// <summary>
		/// Intenta igualar recortando
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="epsilon">0.01</param>
		/// <returns></returns>
		public static bool Equals(this double a, double b, double epsilon)
		{
			double absA = Math.Abs(a);
			double absB = Math.Abs(b);
			double diff = Math.Abs(a - b);

			if (a == b)
			{
				// shortcut, handles infinities
				return true;
			}
			else if (a == 0 || b == 0 || diff < double.Epsilon)
			{
				// a or b is zero or both are extremely close to it
				// relative error is less meaningful here
				return diff < epsilon;
			}
			else
			{
				// use relative error
				return diff / (absA + absB) < epsilon;
			}
		}

		/// <summary>
		/// Intenta igualar recortando
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="epsilon">0.01M</param>
		/// <returns></returns>
		public static bool Equals(this decimal a, decimal b, decimal epsilon)
		{
			decimal absA = Math.Abs(a);
			decimal absB = Math.Abs(b);
			decimal diff = Math.Abs(a - b);

			if (a == b)
			{
				// shortcut, handles infinities
				return true;
			}
			else if (a == 0 || b == 0 || diff < Convert.ToDecimal(double.Epsilon))
			{
				// a or b is zero or both are extremely close to it
				// relative error is less meaningful here
				return diff < epsilon;
			}
			else
			{
				// use relative error
				return diff / (absA + absB) < epsilon;
			}
		}
	}
}
