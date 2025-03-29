#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ValueMax of a Rating depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Rating ValueMax Setter")]
	public class RatingValueMaxSetter : ComponentSingleSetter<UIWidgets.Rating, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Rating target, System.Int32 value)
		{
			target.ValueMax = value;
		}
	}
}
#endif