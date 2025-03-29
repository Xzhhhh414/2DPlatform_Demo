namespace UIWidgets.Timeline
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Serialized TimeSpan.
	/// </summary>
	[Serializable]
	public struct SerializedTimeSpan : IEquatable<SerializedTimeSpan>
	{
		/// <summary>
		/// Hours.
		/// </summary>
		[SerializeField]
		public int Hours;

		/// <summary>
		/// Minutes.
		/// </summary>
		[SerializeField]
		public int Minutes;

		/// <summary>
		/// Seconds.
		/// </summary>
		[SerializeField]
		public int Seconds;

		/// <summary>
		/// Milliseconds.
		/// </summary>
		[SerializeField]
		public int Milliseconds;

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializedTimeSpan"/> struct.
		/// </summary>
		/// <param name="hours">Hours.</param>
		/// <param name="minutes">Minutes.</param>
		/// <param name="seconds">Seconds.</param>
		/// <param name="milliseconds">Milliseconds.</param>
		public SerializedTimeSpan(int hours, int minutes, int seconds, int milliseconds = 0)
		{
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
			Milliseconds = milliseconds;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly override bool Equals(object obj) => (obj is SerializedTimeSpan span) && Equals(span);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly bool Equals(SerializedTimeSpan other)
		{
			return Hours == other.Hours && Minutes == other.Minutes && Seconds == other.Seconds && Milliseconds == other.Milliseconds;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public readonly override int GetHashCode() => Hours ^ Minutes ^ Seconds ^ Milliseconds;

		/// <summary>
		/// Compare specified values.
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <returns>true if the values are equal; otherwise, false.</returns>
		public static bool operator ==(SerializedTimeSpan a, SerializedTimeSpan b) => a.Equals(b);

		/// <summary>
		/// Compare specified values.
		/// </summary>
		/// <param name="a">First value.</param>
		/// <param name="b">Second value.</param>
		/// <returns>true if the values not equal; otherwise, false.</returns>
		public static bool operator !=(SerializedTimeSpan a, SerializedTimeSpan b) => !a.Equals(b);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
		public override string ToString() => string.Format("{0}:{1}:{2}.{3}", Hours.ToString(), Minutes.ToString(), Seconds.ToString(), Milliseconds.ToString());

		/// <summary>
		/// Convert SerializedTimeSpan to TimeSpan.
		/// </summary>
		/// <param name="time">Time to convert.</param>
		public static implicit operator TimeSpan(SerializedTimeSpan time) => new TimeSpan(0, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
	}
}