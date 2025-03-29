#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ScrollMode of a SliderScroll depending on the UIWidgets.ScrollModes data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SliderScroll ScrollMode Setter")]
	public class SliderScrollScrollModeSetter : ComponentSingleSetter<UIWidgets.SliderScroll, UIWidgets.SliderScroll.ScrollModes>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SliderScroll target, UIWidgets.SliderScroll.ScrollModes value)
		{
			target.ScrollMode = value;
		}
	}
}
#endif