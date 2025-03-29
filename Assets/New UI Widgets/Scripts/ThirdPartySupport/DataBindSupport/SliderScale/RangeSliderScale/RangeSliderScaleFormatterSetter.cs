#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Formatter of a RangeSliderScale depending on the System.Func<System.Single,System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSliderScale Formatter Setter")]
	public class RangeSliderScaleFormatterSetter : ComponentSingleSetter<UIWidgets.RangeSliderScale, System.Func<System.Single,System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderScale target, System.Func<System.Single,System.String> value)
		{
			target.Formatter = value;
		}
	}
}
#endif