namespace UIWidgets
{
	using System;

	/// <summary>
	/// List node.
	/// </summary>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListNode<TItem> : IEquatable<ListNode<TItem>>
	{
		/// <summary>
		/// The depth.
		/// </summary>
		public int Depth;

		/// <summary>
		/// The node.
		/// </summary>
		public TreeNode<TItem> Node;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListNode{TItem}"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public ListNode(TreeNode<TItem> node, int depth)
		{
			Node = node;
			Depth = depth;
		}

		/// <summary>
		/// Replace fields with new data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		/// <returns>This instance.</returns>
		public ListNode<TItem> Replace(TreeNode<TItem> node, int depth)
		{
			Node = node;
			Depth = depth;

			return this;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public override bool Equals(object other) => (other is ListNode<TItem> node) && Equals(node);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public virtual bool Equals(ListNode<TItem> other)
		{
			if (other is null)
			{
				return false;
			}

			return (Depth == other.Depth) && ReferenceEquals(Node, other.Node);
		}

		/// <summary>
		/// Returns true if the nodes items are equal, false otherwise.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns>true if the objects equal; otherwise, false.</returns>
		public static bool operator ==(ListNode<TItem> a, ListNode<TItem> b)
		{
			if (a is null)
			{
				return b is null;
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Returns true if the nodes items are not equal, false otherwise.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns>true if the objects not equal; otherwise, false.</returns>
		public static bool operator !=(ListNode<TItem> a, ListNode<TItem> b) => !(a == b);

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode() => base.GetHashCode();

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString() => string.Format("ListNode(Node = {0}; Depth = {1})", Node != null ? Node.ToString() : "null", Depth.ToString());
	}
}