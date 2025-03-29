#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Sprite of a SingleConnector depending on the UnityEngine.Sprite data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SingleConnector Sprite Setter")]
	public class SingleConnectorSpriteSetter : ComponentSingleSetter<UIWidgets.SingleConnector, UnityEngine.Sprite>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SingleConnector target, UnityEngine.Sprite value)
		{
			target.Sprite = value;
		}
	}
}
#endif