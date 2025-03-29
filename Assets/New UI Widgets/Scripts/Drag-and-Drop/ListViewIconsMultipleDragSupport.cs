namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support of selected items for the ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class ListViewIconsMultipleDragSupport : DragSupport<List<ListViewIconsItemDescription>>
	{
		/// <summary>
		/// Delete selected items after drop.
		/// </summary>
		[SerializeField]
		public bool DeleteAfterDrop;

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewIcons ListView;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TryGetComponent(out ListView);
		}

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Init();
			Data = ListView.SelectedItems;
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			if (success && DeleteAfterDrop)
			{
				using var _ = ListView.DataSource.BeginUpdate();

				foreach (var item in Data)
				{
					ListView.DataSource.Remove(item);
				}
			}

			base.Dropped(success);
		}
	}
}