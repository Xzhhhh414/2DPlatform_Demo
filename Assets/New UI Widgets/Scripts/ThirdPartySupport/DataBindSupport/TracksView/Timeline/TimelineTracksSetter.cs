#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Tracks of a Timeline depending on the UIWidgets.ObservableList<UIWidgets.Track<UIWidgets.Timeline.TimelineData,System.TimeSpan>> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline Tracks Setter")]
	public class TimelineTracksSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, UIWidgets.ObservableList<UIWidgets.Track<UIWidgets.Timeline.TimelineData,System.TimeSpan>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, UIWidgets.ObservableList<UIWidgets.Track<UIWidgets.Timeline.TimelineData,System.TimeSpan>> value)
		{
			target.Tracks = value;
		}
	}
}
#endif