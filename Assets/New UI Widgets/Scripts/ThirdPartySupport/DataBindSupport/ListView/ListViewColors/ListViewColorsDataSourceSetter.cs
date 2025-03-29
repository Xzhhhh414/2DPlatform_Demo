#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewColors depending on the UIWidgets.ObservableList<UnityEngine.Color> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewColors DataSource Setter")]
	public class ListViewColorsDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewColors, UIWidgets.ObservableList<UnityEngine.Color>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewColors target, UIWidgets.ObservableList<UnityEngine.Color> value)
		{
			target.DataSource = value;
		}
	}
}
#endif