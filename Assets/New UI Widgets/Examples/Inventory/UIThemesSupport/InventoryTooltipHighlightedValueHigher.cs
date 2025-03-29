namespace UIWidgets.Examples.Inventory
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the HighlightedValueHigher of InventoryTooltip.
	/// </summary>
	public class InventoryTooltipHighlightedValueHigher : Wrapper<Color, InventoryTooltip>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InventoryTooltipHighlightedValueHigher"/> class.
		/// </summary>
		public InventoryTooltipHighlightedValueHigher()
		{
			Name = nameof(InventoryTooltip.HighlightedValueHigher);
		}

		/// <inheritdoc/>
		protected override Color Get(InventoryTooltip widget)
		{
			return widget.HighlightedValueHigher;
		}

		/// <inheritdoc/>
		protected override void Set(InventoryTooltip widget, Color value)
		{
			widget.HighlightedValueHigher = value;
		}
	}
}