#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the interactable of a InputFieldExtendedAdapter depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldExtendedAdapter interactable Setter")]
	public class InputFieldExtendedAdapterinteractableSetter : ComponentSingleSetter<UIWidgets.InputFieldExtendedAdapter, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldExtendedAdapter target, System.Boolean value)
		{
			target.interactable = value;
		}
	}
}
#endif