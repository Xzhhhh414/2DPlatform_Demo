#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a InputFieldAdapter depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldAdapter Value Setter")]
	public class InputFieldAdapterValueSetter : ComponentSingleSetter<UIWidgets.InputFieldAdapter, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldAdapter target, System.String value)
		{
			target.Value = value;
		}
	}
}
#endif