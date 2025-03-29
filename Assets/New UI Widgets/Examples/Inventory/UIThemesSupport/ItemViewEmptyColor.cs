namespace UIWidgets.Examples.Inventory
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the EmptyColor of Shadow.
	/// </summary>
	public class ItemViewEmptyColor : Wrapper<Color, ItemView>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemViewEmptyColor"/> class.
		/// </summary>
		public ItemViewEmptyColor()
		{
			Name = nameof(ItemView.EmptyColor);
		}

		/// <inheritdoc/>
		protected override Color Get(ItemView widget)
		{
			return widget.EmptyColor;
		}

		/// <inheritdoc/>
		protected override void Set(ItemView widget, Color value)
		{
			widget.EmptyColor = value;
		}
	}
}