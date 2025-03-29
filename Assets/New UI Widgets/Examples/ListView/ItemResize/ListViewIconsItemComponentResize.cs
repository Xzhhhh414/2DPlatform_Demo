namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// ListViewIconsItemComponent to use with ListViewSelectedItemResize.
	/// </summary>
	public class ListViewIconsItemComponentResize : ListViewIconsItemComponent
	{
		ListViewSelectedItemResize resizer;

		/// <summary>
		/// Resizer.
		/// </summary>
		protected ListViewSelectedItemResize Resizer
		{
			get
			{
				if ((resizer == null) && (Owner != null))
				{
					Owner.TryGetComponent(out resizer);
				}

				return resizer;
			}
		}

		/// <inheritdoc/>
		public override void SetData(ListViewIconsItemDescription item)
		{
			if (Resizer != null)
			{
				var size = Owner.IsSelected(Index) ? Resizer.SelectedSize : Resizer.DefaultSize;
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}

			base.SetData(item);
		}
	}
}