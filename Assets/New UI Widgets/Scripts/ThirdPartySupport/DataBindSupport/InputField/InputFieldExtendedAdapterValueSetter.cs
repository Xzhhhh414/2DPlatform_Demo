#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a InputFieldExtendedAdapter depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldExtendedAdapter Value Setter")]
	public class InputFieldExtendedAdapterValueSetter : ComponentSingleSetter<UIWidgets.InputFieldExtendedAdapter, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldExtendedAdapter target, System.String value)
		{
			target.Value = value;
		}
	}
}
#endif