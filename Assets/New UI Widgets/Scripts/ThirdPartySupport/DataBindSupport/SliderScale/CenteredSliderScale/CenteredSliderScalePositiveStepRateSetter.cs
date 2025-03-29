#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the PositiveStepRate of a CenteredSliderScale depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSliderScale PositiveStepRate Setter")]
	public class CenteredSliderScalePositiveStepRateSetter : ComponentSingleSetter<UIWidgets.CenteredSliderScale, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderScale target, System.Single value)
		{
			target.PositiveStepRate = value;
		}
	}
}
#endif