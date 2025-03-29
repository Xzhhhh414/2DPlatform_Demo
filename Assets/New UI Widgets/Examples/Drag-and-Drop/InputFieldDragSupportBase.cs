namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class InputFieldDragSupportBase : DragSupport<string>
	{
		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = TryGetComponent<InputFieldAdapter>(out var adapter) ? adapter.text : string.Empty;
		}
	}
}