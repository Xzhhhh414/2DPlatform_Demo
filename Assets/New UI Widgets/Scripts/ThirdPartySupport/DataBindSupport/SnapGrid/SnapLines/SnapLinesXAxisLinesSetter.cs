#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the XAxisLines of a SnapLines depending on the UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineX> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapLines XAxisLines Setter")]
	public class SnapLinesXAxisLinesSetter : ComponentSingleSetter<UIWidgets.SnapLines, UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineX>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapLines target, UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineX> value)
		{
			target.XAxisLines = value;
		}
	}
}
#endif