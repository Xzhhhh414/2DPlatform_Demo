#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SelectedItems of a ListViewEnum depending on the System.Collections.Generic.List<UIWidgets.Item> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewEnum SelectedItems Setter")]
	public class ListViewEnumSelectedItemsSetter : ComponentSingleSetter<UIWidgets.ListViewEnum, System.Collections.Generic.List<UIWidgets.ListViewEnum.Item>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewEnum target, System.Collections.Generic.List<UIWidgets.ListViewEnum.Item> value)
		{
			target.SelectedItems = value;
		}
	}
}
#endif