#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ItemsSpacing of a Timeline depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline ItemsSpacing Setter")]
	public class TimelineItemsSpacingSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, System.Single value)
		{
			target.ItemsSpacing = value;
		}
	}
}
#endif