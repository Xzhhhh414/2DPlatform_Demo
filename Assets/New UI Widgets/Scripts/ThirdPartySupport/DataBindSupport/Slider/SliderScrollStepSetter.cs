#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Step of a SliderScroll depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SliderScroll Step Setter")]
	public class SliderScrollStepSetter : ComponentSingleSetter<UIWidgets.SliderScroll, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SliderScroll target, System.Single value)
		{
			target.Step = value;
		}
	}
}
#endif