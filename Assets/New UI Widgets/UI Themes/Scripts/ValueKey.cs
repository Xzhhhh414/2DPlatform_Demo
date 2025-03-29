namespace UIThemes
{
	using System;

	/// <summary>
	/// Value key.
	/// </summary>
	public readonly struct ValueKey : IEquatable<ValueKey>
	{
		/// <summary>
		/// Variation ID.
		/// </summary>
		public readonly VariationId VariationId;

		/// <summary>
		/// Option ID.
		/// </summary>
		public readonly OptionId OptionId;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueKey"/> struct.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		public ValueKey(VariationId variationId, OptionId optionId)
		{
			VariationId = variationId;
			OptionId = optionId;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj) => (obj is ValueKey key) && Equals(key);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(ValueKey other) => (VariationId == other.VariationId) && (OptionId == other.OptionId);

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => VariationId.Id ^ OptionId.Id;

		/// <summary>
		/// Compare specified objects.
		/// </summary>
		/// <param name="a">First object.</param>
		/// <param name="b">Second object.</param>
		/// <returns>true if the objects are equal; otherwise, false.</returns>
		public static bool operator ==(ValueKey a, ValueKey b) => a.Equals(b);

		/// <summary>
		/// Compare specified objects.
		/// </summary>
		/// <param name="a">First object.</param>
		/// <param name="b">Second object.</param>
		/// <returns>true if the objects not equal; otherwise, false.</returns>
		public static bool operator !=(ValueKey a, ValueKey b) => !a.Equals(b);
	}
}