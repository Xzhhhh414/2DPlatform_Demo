#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ColorMax of a Rating depending on the UnityEngine.Color data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Rating ColorMax Setter")]
	public class RatingColorMaxSetter : ComponentSingleSetter<UIWidgets.Rating, UnityEngine.Color>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Rating target, UnityEngine.Color value)
		{
			target.ColorMax = value;
		}
	}
}
#endif