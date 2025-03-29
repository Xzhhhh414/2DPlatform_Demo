#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Formatter of a SliderScale depending on the System.Func<System.Single,System.String> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SliderScale Formatter Setter")]
	public class SliderScaleFormatterSetter : ComponentSingleSetter<UIWidgets.SliderScale, System.Func<System.Single,System.String>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SliderScale target, System.Func<System.Single,System.String> value)
		{
			target.Formatter = value;
		}
	}
}
#endif