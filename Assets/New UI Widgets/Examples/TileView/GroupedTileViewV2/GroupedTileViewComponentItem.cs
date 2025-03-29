namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Item.
	/// </summary>
	public class GroupedTileViewComponentItem : GroupedTileViewComponentBase
	{
		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public Image Image;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(Photo item)
		{
			Item = item;
			Image.sprite = Item.Image;
		}

		/// <inheritdoc/>
		public override void MovedToCache()
		{
			base.MovedToCache();

			Image.sprite = null;
		}
	}
}