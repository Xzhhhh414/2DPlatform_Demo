namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Header (group).
	/// </summary>
	public class GroupedTileViewComponentHeader : GroupedTileViewComponentBase
	{
		/// <summary>
		/// Date.
		/// </summary>
		[SerializeField]
		public TextAdapter DateAdapter;

		/// <summary>
		/// Layout.
		/// </summary>
		[SerializeField]
		protected EasyLayoutNS.EasyLayout Layout;

		/// <summary>
		/// Layout RectTransform.
		/// </summary>
		[SerializeField]
		protected RectTransform LayoutRect;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(Photo item)
		{
			Item = item;
			DateAdapter.text = Item.Created.ToString("MMM. dd, yyyy", UtilitiesCompare.Culture);

			if (Layout != null)
			{
				var width = LayoutRect.rect.width - Layout.MarginFullHorizontal;
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (Layout == null)
			{
				transform.parent.TryGetComponent(out Layout);
			}

			if (LayoutRect == null)
			{
				LayoutRect = transform.parent as RectTransform;
			}
		}
#endif
	}
}