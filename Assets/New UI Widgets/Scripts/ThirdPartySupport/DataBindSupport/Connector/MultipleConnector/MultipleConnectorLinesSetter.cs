#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Lines of a MultipleConnector depending on the UIWidgets.ObservableList<UIWidgets.ConnectorLine> data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] MultipleConnector Lines Setter")]
	public class MultipleConnectorLinesSetter : ComponentSingleSetter<UIWidgets.MultipleConnector, UIWidgets.ObservableList<UIWidgets.ConnectorLine>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.MultipleConnector target, UIWidgets.ObservableList<UIWidgets.ConnectorLine> value)
		{
			target.Lines = value;
		}
	}
}
#endif