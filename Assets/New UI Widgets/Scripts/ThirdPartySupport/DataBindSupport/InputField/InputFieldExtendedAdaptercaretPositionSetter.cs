#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the caretPosition of a InputFieldExtendedAdapter depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldExtendedAdapter caretPosition Setter")]
	public class InputFieldExtendedAdaptercaretPositionSetter : ComponentSingleSetter<UIWidgets.InputFieldExtendedAdapter, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldExtendedAdapter target, System.Int32 value)
		{
			target.caretPosition = value;
		}
	}
}
#endif