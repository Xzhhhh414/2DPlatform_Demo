#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Interactable of a ListViewEnum depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewEnum Interactable Setter")]
	public class ListViewEnumInteractableSetter : ComponentSingleSetter<UIWidgets.ListViewEnum, System.Boolean>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewEnum target, System.Boolean value)
		{
			target.Interactable = value;
		}
	}
}
#endif