#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SelectedItems of a ListViewColors depending on the System.Collections.Generic.List<UnityEngine.Color> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewColors SelectedItems Setter")]
	public class ListViewColorsSelectedItemsSetter : ComponentSingleSetter<UIWidgets.ListViewColors, System.Collections.Generic.List<UnityEngine.Color>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewColors target, System.Collections.Generic.List<UnityEngine.Color> value)
		{
			target.SelectedItems = value;
		}
	}
}
#endif