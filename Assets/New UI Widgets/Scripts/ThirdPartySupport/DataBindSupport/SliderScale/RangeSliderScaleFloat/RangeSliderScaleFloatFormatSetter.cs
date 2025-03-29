#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Format of a RangeSliderScaleFloat depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSliderScaleFloat Format Setter")]
	public class RangeSliderScaleFloatFormatSetter : ComponentSingleSetter<UIWidgets.RangeSliderScaleFloat, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderScaleFloat target, System.String value)
		{
			target.Format = value;
		}
	}
}
#endif