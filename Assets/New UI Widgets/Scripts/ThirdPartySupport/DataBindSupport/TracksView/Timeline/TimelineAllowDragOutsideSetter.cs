#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the AllowDragOutside of a Timeline depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline AllowDragOutside Setter")]
	public class TimelineAllowDragOutsideSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, System.Boolean value)
		{
			target.AllowDragOutside = value;
		}
	}
}
#endif