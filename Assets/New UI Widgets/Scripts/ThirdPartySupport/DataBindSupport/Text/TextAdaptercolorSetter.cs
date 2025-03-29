#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the color of a TextAdapter depending on the UnityEngine.Color data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TextAdapter color Setter")]
	public class TextAdaptercolorSetter : ComponentSingleSetter<UIWidgets.TextAdapter, UnityEngine.Color>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TextAdapter target, UnityEngine.Color value)
		{
			target.color = value;
		}
	}
}
#endif