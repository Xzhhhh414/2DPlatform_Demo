#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the interactable of a InputFieldAdapter depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] InputFieldAdapter interactable Setter")]
	public class InputFieldAdapterinteractableSetter : ComponentSingleSetter<UIWidgets.InputFieldAdapter, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.InputFieldAdapter target, System.Boolean value)
		{
			target.interactable = value;
		}
	}
}
#endif