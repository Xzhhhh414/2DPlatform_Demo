namespace UIWidgets.Examples.Inventory
{
	using UIThemes;
	using UnityEngine;
	using UnityEngine.Scripting;

	/// <summary>
	/// Inventory view.
	/// </summary>
	public class InventoryView : ListViewCustom<ItemView, Item>
	{
		/// <summary>
		/// Add wrappers.
		/// </summary>
		[PropertiesRegistry]
		[Preserve]
		public static void AddWrappers()
		{
			PropertyWrappers<Color>.Add(new InventoryTooltipHighlightedValueHigher());
			PropertyWrappers<Color>.Add(new InventoryTooltipHighlightedValueLower());
			PropertyWrappers<Color>.Add(new ItemViewEmptyColor());
		}
	}
}