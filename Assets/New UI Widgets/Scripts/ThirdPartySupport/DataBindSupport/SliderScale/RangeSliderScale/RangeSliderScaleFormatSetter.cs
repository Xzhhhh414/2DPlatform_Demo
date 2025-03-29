#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Format of a RangeSliderScale depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSliderScale Format Setter")]
	public class RangeSliderScaleFormatSetter : ComponentSingleSetter<UIWidgets.RangeSliderScale, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderScale target, System.String value)
		{
			target.Format = value;
		}
	}
}
#endif