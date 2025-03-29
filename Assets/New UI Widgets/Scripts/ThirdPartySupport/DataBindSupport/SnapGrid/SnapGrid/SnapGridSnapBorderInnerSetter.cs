#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SnapBorderInner of a SnapGrid depending on the UIWidgets.Border data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SnapGrid SnapBorderInner Setter")]
	public class SnapGridSnapBorderInnerSetter : ComponentSingleSetter<UIWidgets.SnapGrid, UIWidgets.SnapGridBase.Border>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SnapGrid target, UIWidgets.SnapGridBase.Border value)
		{
			target.SnapBorderInner = value;
		}
	}
}
#endif