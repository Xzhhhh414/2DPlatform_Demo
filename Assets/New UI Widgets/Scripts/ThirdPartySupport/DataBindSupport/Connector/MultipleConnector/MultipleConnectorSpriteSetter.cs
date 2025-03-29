#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Sprite of a MultipleConnector depending on the UnityEngine.Sprite data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] MultipleConnector Sprite Setter")]
	public class MultipleConnectorSpriteSetter : ComponentSingleSetter<UIWidgets.MultipleConnector, UnityEngine.Sprite>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.MultipleConnector target, UnityEngine.Sprite value)
		{
			target.Sprite = value;
		}
	}
}
#endif