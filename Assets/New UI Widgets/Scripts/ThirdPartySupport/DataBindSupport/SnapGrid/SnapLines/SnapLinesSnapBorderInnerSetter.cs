#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SnapBorderInner of a SnapLines depending on the UIWidgets.SnapGridBase.Border data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapLines SnapBorderInner Setter")]
	public class SnapLinesSnapBorderInnerSetter : ComponentSingleSetter<UIWidgets.SnapLines, UIWidgets.SnapGridBase.Border>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapLines target, UIWidgets.SnapGridBase.Border value)
		{
			target.SnapBorderInner = value;
		}
	}
}
#endif