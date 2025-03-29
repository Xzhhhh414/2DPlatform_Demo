#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value2Color of a Rating depending on the System.Func<System.Int32,UnityEngine.Color> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Rating Value2Color Setter")]
	public class RatingValue2ColorSetter : ComponentSingleSetter<UIWidgets.Rating, System.Func<System.Int32,UnityEngine.Color>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.Rating target, System.Func<System.Int32,UnityEngine.Color> value)
		{
			target.Value2Color = value;
		}
	}
}
#endif