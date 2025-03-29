#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.Timeline.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a Timeline depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Timeline Interactable Setter")]
	public class TimelineInteractableSetter : ComponentSingleSetter<UIWidgets.Timeline.Timeline, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Timeline.Timeline target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif