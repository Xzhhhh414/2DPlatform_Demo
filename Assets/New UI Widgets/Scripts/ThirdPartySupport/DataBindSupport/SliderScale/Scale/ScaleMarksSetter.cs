#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Marks of a Scale depending on the UIWidgets.ObservableList<UIWidgets.ScaleMark> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Scale Marks Setter")]
	public class ScaleMarksSetter : ComponentSingleSetter<UIWidgets.Scale, UIWidgets.ObservableList<UIWidgets.ScaleMark>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Scale target, UIWidgets.ObservableList<UIWidgets.ScaleMark> value)
		{
			target.Marks = value;
		}
	}
}
#endif