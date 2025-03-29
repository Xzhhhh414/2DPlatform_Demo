#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a TextAdapter depending on the System.String data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TextAdapter Value Setter")]
	public class TextAdapterValueSetter : ComponentSingleSetter<UIWidgets.TextAdapter, System.String>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TextAdapter target, System.String value)
		{
			target.Value = value;
		}
	}
}
#endif