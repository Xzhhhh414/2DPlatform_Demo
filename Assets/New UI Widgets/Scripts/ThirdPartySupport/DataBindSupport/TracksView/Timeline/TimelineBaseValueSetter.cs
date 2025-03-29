#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the BaseValue of a Timeline depending on the System.TimeSpan data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline BaseValue Setter")]
	public class TimelineBaseValueSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, System.TimeSpan>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, System.TimeSpan value)
		{
			target.BaseValue = value;
		}
	}
}
#endif