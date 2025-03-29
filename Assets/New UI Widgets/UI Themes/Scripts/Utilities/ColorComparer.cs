namespace UIThemes
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Color comparer.
	/// </summary>
	public class ColorComparer : IEqualityComparer<Color>
	{
		[DomainReloadExclude]
		static readonly ColorComparer StaticInstance = new ColorComparer();

		/// <summary>
		/// Instance.
		/// </summary>
		public static ColorComparer Instance
		{
			get
			{
				return StaticInstance;
			}
		}

		/// <summary>
		/// Compare two colors.
		/// </summary>
		/// <param name="x">First color.</param>
		/// <param name="y">Second color.</param>
		/// <returns>true if colors are equal; otherwise false.</returns>
		public bool Equals(Color x, Color y)
		{
			return x == y;
		}

		/// <summary>
		/// Get hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <returns>Hash code.</returns>
		public int GetHashCode(Color obj)
		{
			return obj.GetHashCode();
		}
	}
}