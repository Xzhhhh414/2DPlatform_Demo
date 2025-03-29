#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ShowMaxValue of a Scale depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Scale ShowMaxValue Setter")]
	public class ScaleShowMaxValueSetter : ComponentSingleSetter<UIWidgets.Scale, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Scale target, System.Boolean value)
		{
			target.ShowMaxValue = value;
		}
	}
}
#endif