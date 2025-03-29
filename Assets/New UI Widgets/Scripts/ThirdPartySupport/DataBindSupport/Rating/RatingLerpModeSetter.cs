#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the LerpMode of a Rating depending on the UIWidgets.ColorLerpMode data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Rating LerpMode Setter")]
	public class RatingLerpModeSetter : ComponentSingleSetter<UIWidgets.Rating, UIWidgets.Rating.ColorLerpMode>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Rating target, UIWidgets.Rating.ColorLerpMode value)
		{
			target.LerpMode = value;
		}
	}
}
#endif