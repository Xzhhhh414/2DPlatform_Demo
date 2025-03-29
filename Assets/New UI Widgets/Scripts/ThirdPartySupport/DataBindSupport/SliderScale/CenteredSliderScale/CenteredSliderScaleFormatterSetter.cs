#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Formatter of a CenteredSliderScale depending on the System.Func<System.Single,System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSliderScale Formatter Setter")]
	public class CenteredSliderScaleFormatterSetter : ComponentSingleSetter<UIWidgets.CenteredSliderScale, System.Func<System.Single,System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderScale target, System.Func<System.Single,System.String> value)
		{
			target.Formatter = value;
		}
	}
}
#endif