namespace UIWidgets
{
	/// <summary>
	/// Item position info.
	/// Should be nested inside TileViewStaggered, but impossible due to a bug.
	/// https://issuetracker.unity3d.com/issues/nullables-of-nested-struct-inside-generic-class-cause-internal-error
	/// </summary>
	struct TileViewStaggeredItemPosition : System.IEquatable<TileViewStaggeredItemPosition>
	{
		/// <summary>
		/// Block index.
		/// </summary>
		public int Block;

		/// <summary>
		/// Position.
		/// </summary>
		public float Position;

		/// <summary>
		/// Initializes a new instance of the <see cref="TileViewStaggeredItemPosition"/> struct.
		/// </summary>
		/// <param name="block">Block.</param>
		/// <param name="position">Position.</param>
		public TileViewStaggeredItemPosition(int block, float position)
		{
			Block = block;
			Position = position;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly override bool Equals(object obj) => (obj is TileViewStaggeredItemPosition position) && Equals(position);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public readonly bool Equals(TileViewStaggeredItemPosition other) => Position == other.Position && Block == other.Block;

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() => Position.GetHashCode() ^ Block;

		/// <summary>
		/// Compare specified visibility data.
		/// </summary>
		/// <param name="a">First data.</param>
		/// <param name="b">Second data.</param>
		/// <returns>true if the data are equal; otherwise, false.</returns>
		public static bool operator ==(TileViewStaggeredItemPosition a, TileViewStaggeredItemPosition b) => a.Equals(b);

		/// <summary>
		/// Compare specified visibility data.
		/// </summary>
		/// <param name="a">First data.</param>
		/// <param name="b">Second data.</param>
		/// <returns>true if the data not equal; otherwise, false.</returns>
		public static bool operator !=(TileViewStaggeredItemPosition a, TileViewStaggeredItemPosition b) => !a.Equals(b);
	}
}