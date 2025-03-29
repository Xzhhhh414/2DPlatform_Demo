namespace UIThemes
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Color32 comparer.
	/// </summary>
	public class Color32Comparer : IEqualityComparer<Color>
	{
		[DomainReloadExclude]
		static readonly Color32Comparer StaticInstance = new Color32Comparer();

		/// <summary>
		/// Instance.
		/// </summary>
		public static Color32Comparer Instance
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
			var x_32 = (Color32)x;
			var y_32 = (Color32)y;

			return (x_32.r == y_32.r) && (x_32.g == y_32.g) && (x_32.b == y_32.b) && (x_32.a == y_32.a);
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