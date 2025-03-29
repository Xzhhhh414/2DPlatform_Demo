namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.Internal;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// TreeViewCustom.
	/// </summary>
	/// <typeparam name="TItemView">Type of DefaultItem.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public partial class TreeViewCustom<TItemView, TItem> : ListViewCustom<TItemView, ListNode<TItem>>
		where TItemView : TreeViewComponentBase<TItem>
	{
		/// <summary>
		/// Scroll data for the ListView.
		/// </summary>
		protected sealed class TreeViewScrollData : ListViewScrollData<TreeNode<TItem>>
		{
			readonly TreeViewCustom<TItemView, TItem> owner;

			/// <inheritdoc/>
			protected sealed override ListViewBase ListView => owner;

			/// <inheritdoc/>
			protected sealed override float Margin => owner.LayoutBridge.GetMargin();

			/// <inheritdoc/>
			protected sealed override bool RetainScrollPosition => owner.RetainScrollPosition;

			/// <summary>
			/// Initializes a new instance of the <see cref="TreeViewScrollData"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public TreeViewScrollData(TreeViewCustom<TItemView, TItem> owner)
			{
				this.owner = owner;
			}

			/// <inheritdoc/>
			protected sealed override TreeNode<TItem> GetItem(int index) => owner.DataSource[index].Node;

			/// <inheritdoc/>
			protected sealed override int IndexOf(TreeNode<TItem> item) => item.Index;
		}
	}
}