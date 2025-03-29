namespace UIThemes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Variation ID.
	/// </summary>
	[Serializable]
	public struct VariationId : IEquatable<VariationId>
	{
		/// <summary>
		/// None.
		/// </summary>
		[DomainReloadExclude]
		public static readonly VariationId None = new VariationId(-1);

		[SerializeField]
		int id;

		/// <summary>
		/// ID.
		/// </summary>
		public int Id
		{
			readonly get => id;

			private set => id = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VariationId"/> struct.
		/// </summary>
		/// <param name="id">ID.</param>
		public VariationId(int id) => this.id = id;

		/// <summary>
		/// Convert this instance to the string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString() => id.ToString();

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly override bool Equals(object obj) => (obj is VariationId variationId) && Equals(variationId);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly bool Equals(VariationId other) => id == other.id;

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public readonly override int GetHashCode() => id;

		/// <summary>
		/// Compare specified objects.
		/// </summary>
		/// <param name="a">First object.</param>
		/// <param name="b">Second object.</param>
		/// <returns>true if the objects are equal; otherwise, false.</returns>
		public static bool operator ==(VariationId a, VariationId b) => a.Equals(b);

		/// <summary>
		/// Compare specified objects.
		/// </summary>
		/// <param name="a">First object.</param>
		/// <param name="b">Second object.</param>
		/// <returns>true if the objects not equal; otherwise, false.</returns>
		public static bool operator !=(VariationId a, VariationId b) => !a.Equals(b);
	}
}