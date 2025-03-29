namespace UIThemes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Wrapper for Text.fontSize.
	/// </summary>
	[Serializable]
	public struct FontSizeValue : IEquatable<FontSizeValue>
	{
		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public float Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="FontSizeValue"/> struct.
		/// </summary>
		/// <param name="value">Value.</param>
		public FontSizeValue(float value)
		{
			Value = value;
		}

		/// <summary>
		/// Convert FontSizeValue to the float.
		/// </summary>
		/// <param name="value">Value.</param>
		public static implicit operator float(FontSizeValue value) => value.Value;

		/// <summary>
		/// Convert float to the FontSizeValue.
		/// </summary>
		/// <param name="value">Value.</param>
		public static implicit operator FontSizeValue(float value) => new FontSizeValue(value);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly override bool Equals(object obj) => (obj is FontSizeValue value) && Equals(value);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly bool Equals(FontSizeValue other) => Mathf.Approximately(Value, other.Value);

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Value.GetHashCode();

		/// <summary>
		/// Compare specified values.
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <returns>true if the value are equal; otherwise, false.</returns>
		public static bool operator ==(FontSizeValue a, FontSizeValue b) => a.Equals(b);

		/// <summary>
		/// Compare specified values.
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <returns>true if the values not equal; otherwise, false.</returns>
		public static bool operator !=(FontSizeValue a, FontSizeValue b) => !a.Equals(b);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() => Value.ToString();
	}
}