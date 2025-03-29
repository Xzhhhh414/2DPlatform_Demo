#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a ListViewColors depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewColors Interactable Setter")]
	public class ListViewColorsInteractableSetter : ComponentSingleSetter<UIWidgets.ListViewColors, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewColors target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif