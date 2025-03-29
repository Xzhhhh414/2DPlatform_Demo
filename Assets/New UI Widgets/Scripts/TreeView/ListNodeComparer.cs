namespace UIWidgets
{
	using System.Collections.Generic;

	/// <summary>
	/// Compare ListNode only by node.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class ListNodeComparer<TItem> : IEqualityComparer<ListNode<TItem>>
	{
		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="item1">The first object to compare.</param>
		/// <param name="item2">The second object to compare.</param>
		/// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
		public bool Equals(ListNode<TItem> item1, ListNode<TItem> item2)
		{
			if (item2 == null && item1 == null)
			{
				return true;
			}
			else if (item1 == null || item2 == null)
			{
				return false;
			}

			return ReferenceEquals(item1.Node, item2.Node);
		}

		/// <summary>
		/// Serves as a hash function.
		/// </summary>
		/// <param name="item">The instance to get hash code.</param>
		/// <returns>A hash code for the specified item that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public int GetHashCode(ListNode<TItem> item)
		{
			return item.Node.GetHashCode();
		}
	}
}