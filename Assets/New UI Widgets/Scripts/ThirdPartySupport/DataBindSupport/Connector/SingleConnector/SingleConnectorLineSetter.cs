#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Line of a SingleConnector depending on the UIWidgets.ConnectorLine data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SingleConnector Line Setter")]
	public class SingleConnectorLineSetter : ComponentSingleSetter<UIWidgets.SingleConnector, UIWidgets.ConnectorLine>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SingleConnector target, UIWidgets.ConnectorLine value)
		{
			target.Line = value;
		}
	}
}
#endif