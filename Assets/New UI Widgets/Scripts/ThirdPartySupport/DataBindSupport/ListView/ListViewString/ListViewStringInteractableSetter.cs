#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a ListViewString depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewString Interactable Setter")]
	public class ListViewStringInteractableSetter : ComponentSingleSetter<UIWidgets.ListViewString, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewString target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif