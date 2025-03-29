namespace UIThemes
{
	using System.Collections.Generic;

	/// <summary>
	/// Compare Unity objects using GetInstanceID().
	/// </summary>
	/// <typeparam name="T">Type of Unity object.</typeparam>
	public class UnityObjectComparer<T> : IEqualityComparer<T>
		where T : UnityEngine.Object
	{
		/// <summary>
		/// Comparer instance.
		/// </summary>
		[DomainReloadExclude]
		static readonly UnityObjectComparer<T> StaticInstance = new UnityObjectComparer<T>();

		/// <summary>
		/// Instance.
		/// </summary>
		public static UnityObjectComparer<T> Instance
		{
			get
			{
				return StaticInstance;
			}
		}

		/// <summary>
		/// Compare two objects.
		/// </summary>
		/// <param name="x">First object.</param>
		/// <param name="y">Second object.</param>
		/// <returns>true if two objects are equal; otherwise false.</returns>
		public bool Equals(T x, T y)
		{
			var x_null = x == null;
			var y_null = y == null;
			if (x_null && y_null)
			{
				return true;
			}

			if (x_null != y_null)
			{
				return false;
			}

			return x.GetInstanceID() == y.GetInstanceID();
		}

		/// <summary>
		/// Get hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <returns>Hash code.</returns>
		public int GetHashCode(T obj)
		{
			return obj.GetInstanceID();
		}
	}
}