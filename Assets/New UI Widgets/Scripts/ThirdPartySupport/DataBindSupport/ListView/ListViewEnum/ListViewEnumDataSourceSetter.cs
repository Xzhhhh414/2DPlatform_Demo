#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewEnum depending on the UIWidgets.ObservableList<UIWidgets.Item> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewEnum DataSource Setter")]
	public class ListViewEnumDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewEnum, UIWidgets.ObservableList<UIWidgets.ListViewEnum.Item>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewEnum target, UIWidgets.ObservableList<UIWidgets.ListViewEnum.Item> value)
		{
			target.DataSource = value;
		}
	}
}
#endif