namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ScrollBlock item.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/scroll-block.html")]
	public class ScrollBlockItem : MonoBehaviour, IMovableToCache
	{
		/// <summary>
		/// Index of the item.
		/// </summary>
		[NonSerialized]
		public int Index;

		/// <summary>
		/// Text adapter.
		/// </summary>
		[SerializeField]
		public TextAdapter Text;

		/// <summary>
		/// Owner.
		/// </summary>
		[NonSerialized]
		public ScrollBlockBase Owner;

		/// <summary>
		/// Called when data changed.
		/// </summary>
		public virtual void DataChanged()
		{
		}

		/// <summary>
		/// Process move to cache event.
		/// </summary>
		public virtual void MovedToCache()
		{
		}
	}
}