#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewString depending on the UIWidgets.ObservableList<System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewString DataSource Setter")]
	public class ListViewStringDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewString, UIWidgets.ObservableList<System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewString target, UIWidgets.ObservableList<System.String> value)
		{
			target.DataSource = value;
		}
	}
}
#endif