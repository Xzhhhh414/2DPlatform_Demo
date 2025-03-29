namespace UIWidgets.Examples.Inventory
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the HighlightedValueLower of InventoryTooltip.
	/// </summary>
	public class InventoryTooltipHighlightedValueLower : Wrapper<Color, InventoryTooltip>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InventoryTooltipHighlightedValueLower"/> class.
		/// </summary>
		public InventoryTooltipHighlightedValueLower()
		{
			Name = nameof(InventoryTooltip.HighlightedValueLower);
		}

		/// <inheritdoc/>
		protected override Color Get(InventoryTooltip widget)
		{
			return widget.HighlightedValueLower;
		}

		/// <inheritdoc/>
		protected override void Set(InventoryTooltip widget, Color value)
		{
			widget.HighlightedValueLower = value;
		}
	}
}