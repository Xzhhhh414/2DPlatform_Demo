#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SelectedItems of a ListViewString depending on the System.Collections.Generic.List<System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewString SelectedItems Setter")]
	public class ListViewStringSelectedItemsSetter : ComponentSingleSetter<UIWidgets.ListViewString, System.Collections.Generic.List<System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewString target, System.Collections.Generic.List<System.String> value)
		{
			target.SelectedItems = value;
		}
	}
}
#endif