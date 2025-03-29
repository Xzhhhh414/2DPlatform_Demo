#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SnapBorderOuter of a SnapGrid depending on the UIWidgets.Border data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapGrid SnapBorderOuter Setter")]
	public class SnapGridSnapBorderOuterSetter : ComponentSingleSetter<UIWidgets.SnapGrid, UIWidgets.SnapGridBase.Border>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapGrid target, UIWidgets.SnapGridBase.Border value)
		{
			target.SnapBorderOuter = value;
		}
	}
}
#endif