namespace UIWidgets
{
	using System.Collections;
	using UnityEngine;

	/// <content>
	/// Base class for the custom ListViews.
	/// </content>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// AutoScroll for the ListView.
		/// </summary>
		protected class ListViewAutoScroll : AutoScrollBase<ListViewCustom<TItemView, TItem>>
		{
			/// <inheritdoc/>
			public override float Area => Owner.AutoScrollArea;

			/// <inheritdoc/>
			public override float Speed => Owner.AutoScrollSpeed;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewAutoScroll"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="target">Target.</param>
			public ListViewAutoScroll(ListViewCustom<TItemView, TItem> owner, RectTransform target)
				: base(owner, target)
			{
			}

			/// <inheritdoc/>
			protected override IEnumerator Scroll()
			{
				var min = 0;
				var max = Owner.ListRenderer.CanScroll ? Owner.GetItemPositionBottom(Owner.DataSource.Count - 1) : 0f;

				while (true)
				{
					var delta = Speed * UtilitiesTime.GetDeltaTime(Owner.ScrollUnscaledTime) * Direction;

					var pos = Owner.GetScrollPosition() + delta;
					if (!Owner.LoopedListAvailable)
					{
						pos = Mathf.Clamp(pos, min, max);
					}

					Owner.ScrollToPosition(pos);
					yield return null;

					DragCallback?.Invoke(EventData);
				}
			}

			/// <inheritdoc/>
			protected override bool IsHorizontal() => Owner.IsHorizontal();
		}
	}
}