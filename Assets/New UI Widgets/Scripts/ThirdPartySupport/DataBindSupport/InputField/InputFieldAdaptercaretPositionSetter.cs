#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the caretPosition of a InputFieldAdapter depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldAdapter caretPosition Setter")]
	public class InputFieldAdaptercaretPositionSetter : ComponentSingleSetter<UIWidgets.InputFieldAdapter, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldAdapter target, System.Int32 value)
		{
			target.caretPosition = value;
		}
	}
}
#endif