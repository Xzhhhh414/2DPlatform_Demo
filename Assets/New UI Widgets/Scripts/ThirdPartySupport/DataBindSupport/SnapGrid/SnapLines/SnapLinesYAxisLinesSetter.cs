#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the YAxisLines of a SnapLines depending on the UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineY> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapLines YAxisLines Setter")]
	public class SnapLinesYAxisLinesSetter : ComponentSingleSetter<UIWidgets.SnapLines, UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineY>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapLines target, UIWidgets.ObservableList<UIWidgets.SnapGridBase.LineY> value)
		{
			target.YAxisLines = value;
		}
	}
}
#endif