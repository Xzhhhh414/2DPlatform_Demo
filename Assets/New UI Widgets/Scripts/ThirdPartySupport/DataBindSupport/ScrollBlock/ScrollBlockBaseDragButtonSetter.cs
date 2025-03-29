#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DragButton of a ScrollBlockBase depending on the UnityEngine.EventSystems.InputButton data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ScrollBlockBase DragButton Setter")]
	public class ScrollBlockBaseDragButtonSetter : ComponentSingleSetter<UIWidgets.ScrollBlockBase, UnityEngine.EventSystems.PointerEventData.InputButton>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ScrollBlockBase target, UnityEngine.EventSystems.PointerEventData.InputButton value)
		{
			target.DragButton = value;
		}
	}
}
#endif