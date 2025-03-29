#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ShowMinValue of a Scale depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Scale ShowMinValue Setter")]
	public class ScaleShowMinValueSetter : ComponentSingleSetter<UIWidgets.Scale, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Scale target, System.Boolean value)
		{
			target.ShowMinValue = value;
		}
	}
}
#endif