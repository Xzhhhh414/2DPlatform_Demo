namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class InputFieldDragSupport : DragSupport<string>
	{
		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = TryGetComponent<InputFieldAdapter>(out var adapter) ? adapter.text : string.Empty;

			ShowDragInfo();
		}

		/// <inheritdoc/>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			base.Dropped(success);
		}

		/// <summary>
		/// Component to display draggable data.
		/// </summary>
		[SerializeField]
		public GameObject DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			HideDragInfo();
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo != null)
			{
				DragInfo.SetActive(false);
			}
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = DragInfoOffset;

			DragInfo.SetActive(true);

			var adapter = DragInfo.GetComponentInChildren<TextAdapter>(true);
			if (adapter != null)
			{
				adapter.text = Data;
			}
		}
	}
}