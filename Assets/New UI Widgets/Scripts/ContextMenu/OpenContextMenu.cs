namespace UIWidgets.Menu
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Opens context menu by clicking on a non-UI game object.
	/// Requires PhysicsRaycaster on main camera for the 3D objects.
	/// Requires PhysicsRaycaster2D on main camera for the 2D objects.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/open-contextmenu.html")]
	public class OpenContextMenu : UIBehaviourInteractable, IPointerClickHandler
	{
		/// <summary>
		/// Context menu to open.
		/// </summary>
		[SerializeField]
		public ContextMenu Menu;

		/// <summary>
		/// Pointer button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton PointerButton = PointerEventData.InputButton.Right;

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (IsInteractable() && Menu.IsInteractable() && (eventData.button == PointerButton))
			{
				Menu.Open(eventData);
			}
		}
	}
}