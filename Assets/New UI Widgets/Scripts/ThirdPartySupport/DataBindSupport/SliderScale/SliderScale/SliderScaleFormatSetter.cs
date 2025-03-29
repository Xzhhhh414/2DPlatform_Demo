#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Format of a SliderScale depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SliderScale Format Setter")]
	public class SliderScaleFormatSetter : ComponentSingleSetter<UIWidgets.SliderScale, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SliderScale target, System.String value)
		{
			target.Format = value;
		}
	}
}
#endif