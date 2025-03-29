#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ShowCurrentValue of a Scale depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Scale ShowCurrentValue Setter")]
	public class ScaleShowCurrentValueSetter : ComponentSingleSetter<UIWidgets.Scale, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Scale target, System.Boolean value)
		{
			target.ShowCurrentValue = value;
		}
	}
}
#endif