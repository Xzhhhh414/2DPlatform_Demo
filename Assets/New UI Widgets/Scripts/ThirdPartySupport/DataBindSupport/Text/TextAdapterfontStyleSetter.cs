#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the fontStyle of a TextAdapter depending on the UnityEngine.FontStyle data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TextAdapter fontStyle Setter")]
	public class TextAdapterfontStyleSetter : ComponentSingleSetter<UIWidgets.TextAdapter, UnityEngine.FontStyle>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TextAdapter target, UnityEngine.FontStyle value)
		{
			target.fontStyle = value;
		}
	}
}
#endif