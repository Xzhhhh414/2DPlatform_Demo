#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a SpinnerVector3 depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SpinnerVector3 Interactable Setter")]
	public class SpinnerVector3InteractableSetter : ComponentSingleSetter<UIWidgets.SpinnerVector3, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SpinnerVector3 target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif