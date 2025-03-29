#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Format of a CenteredSliderScale depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSliderScale Format Setter")]
	public class CenteredSliderScaleFormatSetter : ComponentSingleSetter<UIWidgets.CenteredSliderScale, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderScale target, System.String value)
		{
			target.Format = value;
		}
	}
}
#endif