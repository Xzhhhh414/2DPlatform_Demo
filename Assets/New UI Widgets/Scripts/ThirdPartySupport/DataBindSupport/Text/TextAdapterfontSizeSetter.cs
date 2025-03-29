#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the fontSize of a TextAdapter depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TextAdapter fontSize Setter")]
	public class TextAdapterfontSizeSetter : ComponentSingleSetter<UIWidgets.TextAdapter, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TextAdapter target, System.Single value)
		{
			target.fontSize = value;
		}
	}
}
#endif