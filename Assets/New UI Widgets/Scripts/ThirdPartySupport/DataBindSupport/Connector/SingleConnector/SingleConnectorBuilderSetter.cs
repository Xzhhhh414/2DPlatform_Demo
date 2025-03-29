#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Builder of a SingleConnector depending on the UIWidgets.ILineBuilder data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SingleConnector Builder Setter")]
	public class SingleConnectorBuilderSetter : ComponentSingleSetter<UIWidgets.SingleConnector, UIWidgets.ILineBuilder>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SingleConnector target, UIWidgets.ILineBuilder value)
		{
			target.Builder = value;
		}
	}
}
#endif