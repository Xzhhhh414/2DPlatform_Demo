#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a Rating depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Rating Interactable Setter")]
	public class RatingInteractableSetter : ComponentSingleSetter<UIWidgets.Rating, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Rating target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif