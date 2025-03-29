#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SnapBorderOuter of a SnapLines depending on the UIWidgets.SnapGridBase.Border data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapLines SnapBorderOuter Setter")]
	public class SnapLinesSnapBorderOuterSetter : ComponentSingleSetter<UIWidgets.SnapLines, UIWidgets.SnapGridBase.Border>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapLines target, UIWidgets.SnapGridBase.Border value)
		{
			target.SnapBorderOuter = value;
		}
	}
}
#endif