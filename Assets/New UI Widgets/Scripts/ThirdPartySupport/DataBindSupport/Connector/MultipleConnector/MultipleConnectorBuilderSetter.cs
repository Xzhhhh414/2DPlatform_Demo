#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Builder of a MultipleConnector depending on the UIWidgets.ILineBuilder data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] MultipleConnector Builder Setter")]
	public class MultipleConnectorBuilderSetter : ComponentSingleSetter<UIWidgets.MultipleConnector, UIWidgets.ILineBuilder>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.MultipleConnector target, UIWidgets.ILineBuilder value)
		{
			target.Builder = value;
		}
	}
}
#endif