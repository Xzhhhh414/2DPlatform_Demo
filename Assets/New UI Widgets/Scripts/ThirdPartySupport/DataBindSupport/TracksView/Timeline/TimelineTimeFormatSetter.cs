#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the TimeFormat of a Timeline depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline TimeFormat Setter")]
	public class TimelineTimeFormatSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, System.String value)
		{
			target.TimeFormat = value;
		}
	}
}
#endif