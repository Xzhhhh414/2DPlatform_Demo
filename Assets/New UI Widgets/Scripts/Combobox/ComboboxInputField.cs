namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Combobox.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/combobox-inputfield.html")]
	public class ComboboxInputField : UIBehaviourInteractable, ISubmitHandler, IStylable
	{
		[SerializeField]
		ListViewString listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public ListViewString ListView
		{
			get => listView;

			set => SetListView(value);
		}

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton
		{
			get => toggleButton;

			set => SetToggleButton(value);
		}

		/// <summary>
		/// Is Combobox.InputField editable?
		/// </summary>
		[SerializeField]
		protected bool editable = true;

		/// <summary>
		/// Gets or sets a value indicating whether this is editable and user can add new items.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get => editable;

			set
			{
				editable = value;
				if (Input != null)
				{
					Input.interactable = editable;
				}
			}
		}

		/// <summary>
		/// Adapter for the InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMeshPro versions).
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Input;

		/// <summary>
		/// Input field value.
		/// </summary>
		/// <value>The input value.</value>
		public string InputValue
		{
			get => Input.text;

			set
			{
				if (InputValue != null)
				{
					Input.text = value;
				}
			}
		}

		/// <summary>
		/// Hide ListView on item select or deselect.
		/// </summary>
		[SerializeField]
		[Tooltip("Hide ListView on item select or deselect.")]
		public bool HideAfterItemToggle = true;

		/// <summary>
		/// Allow to add new items to list.
		/// </summary>
		[SerializeField]
		public bool AllowNewItems = true;

		/// <summary>
		/// Reset input if item not found and new items not allowed.
		/// </summary>
		[SerializeField]
		public bool ResetInput = true;

		/// <summary>
		/// Parent canvas.
		/// </summary>
		[SerializeField]
		protected RectTransform parentCanvas;

		/// <summary>
		/// Canvas where ListView placed.
		/// </summary>
		protected RectTransform ParentCanvas
		{
			get
			{
				if (parentCanvas == null)
				{
					parentCanvas = UtilitiesUI.FindTopmostCanvas(listView.transform.parent);
				}

				return parentCanvas;
			}
		}

		/// <summary>
		/// ListView parent.
		/// </summary>
		protected RectTransform ListViewParent;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelect = new ListViewEvent();

		/// <summary>
		/// Raised when ListView opened.
		/// </summary>
		[SerializeField]
		public UnityEvent OnShowListView = new UnityEvent();

		/// <summary>
		/// Raised when ListView closed.
		/// </summary>
		[SerializeField]
		public UnityEvent OnHideListView = new UnityEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			InitInputFieldProxy();

			SetToggleButton(toggleButton);

			SetListView(listView);

			if (listView != null)
			{
				listView.gameObject.SetActive(true);
				listView.Init();
				if (listView.SelectedIndex != -1)
				{
					InputValue = listView.DataSource[listView.SelectedIndex];
				}

				listView.gameObject.SetActive(false);
			}
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			base.OnInteractableChange(interactableState);

			if (!interactableState)
			{
				Input.Deactivate();
			}
		}

		/// <summary>
		/// Init InputFieldProxy.
		/// </summary>
		protected virtual void InitInputFieldProxy()
		{
			TryGetComponent(out Input);
			Input.interactable = editable;
			Input.onEndEdit.AddListener(InputItem);
		}

		/// <summary>
		/// Sets the list view.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetListView(ListViewString value)
		{
			if (listView != null)
			{
				ListViewParent = null;

				listView.OnSelectInternal.RemoveListener(SelectItem);
				listView.OnFocusOut.RemoveListener(OnFocusHideList);

				listView.onCancel.RemoveListener(OnListViewCancel);
				listView.InstancesEventsInternal.Cancel.RemoveListener(OnListViewCancel);

				RemoveDeselectCallbacks();
			}

			listView = value;

			if (listView != null)
			{
				listView.KeepHighlight = false;
				ListViewParent = listView.transform.parent as RectTransform;

				listView.OnSelectInternal.AddListener(SelectItem);
				listView.OnFocusOut.AddListener(OnFocusHideList);

				listView.onCancel.AddListener(OnListViewCancel);
				listView.InstancesEventsInternal.Cancel.AddListener(OnListViewCancel);

				AddDeselectCallbacks();
			}
		}

		/// <summary>
		/// Sets the toggle button.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetToggleButton(Button value)
		{
			if (toggleButton != null)
			{
				toggleButton.onClick.RemoveListener(ToggleList);
			}

			toggleButton = value;

			if (toggleButton != null)
			{
				toggleButton.onClick.AddListener(ToggleList);
			}
		}

		/// <summary>
		/// Clear ListView and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.DataSource.Clear();
			InputValue = string.Empty;
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public virtual void ToggleList()
		{
			if (listView == null)
			{
				return;
			}

			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}
		}

		/// <summary>
		/// The modal ID.
		/// </summary>
		protected InstanceID? ModalKey;

		/// <summary>
		/// ListView hierarchy position.
		/// </summary>
		protected HierarchyPosition ListViewPosition;

		/// <summary>
		/// Canvas resize subscription.
		/// </summary>
		protected Subscription CanvasResize;

		/// <summary>
		/// Shows the list.
		/// </summary>
		public virtual void ShowList()
		{
			if (listView == null)
			{
				return;
			}

			ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), HideList, ParentCanvas);

			if (ParentCanvas != null)
			{
				ListViewPosition = HierarchyPosition.SetParent(listView.transform, ParentCanvas);

				var resize = Utilities.RequireComponent<ResizeListener>(ParentCanvas);
				CanvasResize.Clear();
				CanvasResize = new Subscription(resize.OnResize, RefreshPosition);
			}

			listView.gameObject.SetActive(true);

			if (listView.Layout != null)
			{
				listView.Layout.UpdateLayout();
			}

			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}

			OnShowListView.Invoke();
		}

		/// <summary>
		/// Refresh position.
		/// </summary>
		protected virtual void RefreshPosition() => ListViewPosition.Refresh();

		/// <summary>
		/// Hides the list.
		/// </summary>
		public virtual void HideList()
		{
			ModalHelper.Close(ref ModalKey);
			ListViewPosition.Restore();
			CanvasResize.Clear();

			if (listView != null)
			{
				listView.gameObject.SetActive(false);
			}

			OnHideListView.Invoke();
		}

		/// <summary>
		/// The children deselect.
		/// </summary>
		protected List<SelectListener> childrenDeselect = new List<SelectListener>();

		/// <summary>
		/// Hide list when focus lost.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject == gameObject)
			{
				return;
			}

			if (eventData is ListViewItemEventData ev_item)
			{
				if (ev_item.NewSelectedObject != null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}

				return;
			}

			if (!(eventData is PointerEventData ev))
			{
				HideList();
				return;
			}

			var go = ev.pointerPressRaycast.gameObject;
			if (go == null)
			{
				return;
			}

			if (go.Equals(toggleButton.gameObject))
			{
				return;
			}

			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return;
			}

			HideList();
		}

		/// <summary>
		/// Sets the child deselect listener.
		/// </summary>
		/// <param name="child">Child.</param>
		protected void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = GetDeselectListener(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(OnFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		/// <summary>
		/// Gets the deselect listener.
		/// </summary>
		/// <returns>The deselect listener.</returns>
		/// <param name="go">Go.</param>
		protected static SelectListener GetDeselectListener(GameObject go)
		{
			return Utilities.RequireComponent<SelectListener>(go);
		}

		/// <summary>
		/// Adds the deselect callbacks.
		/// </summary>
		protected void AddDeselectCallbacks()
		{
			if (listView.ScrollRect == null)
			{
				return;
			}

			if (listView.ScrollRect.verticalScrollbar == null)
			{
				return;
			}

			var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
			var deselectListener = GetDeselectListener(scrollbar);

			deselectListener.onDeselect.AddListener(OnFocusHideList);
			childrenDeselect.Add(deselectListener);
		}

		/// <summary>
		/// Removes the deselect callbacks.
		/// </summary>
		protected void RemoveDeselectCallbacks()
		{
			foreach (var c in childrenDeselect)
			{
				RemoveDeselectCallback(c);
			}

			childrenDeselect.Clear();
		}

		/// <summary>
		/// Removes the deselect callback.
		/// </summary>
		/// <param name="listener">Listener.</param>
		protected void RemoveDeselectCallback(SelectListener listener)
		{
			if (listener != null)
			{
				listener.onDeselect.RemoveListener(OnFocusHideList);
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public virtual int Set(string item, bool allowDuplicate = true)
		{
			return listView.Set(item, allowDuplicate);
		}

		/// <summary>
		/// Selects the item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void SelectItem(int index)
		{
			InputValue = ListView.DataSource[index];

			if (HideAfterItemToggle)
			{
				HideList();

				if ((EventSystem.current != null) && (!EventSystem.current.alreadySelecting))
				{
					EventSystem.current.SetSelectedGameObject(toggleButton.gameObject);
				}
			}

			OnSelect.Invoke(index, InputValue);
		}

		/// <summary>
		/// Work with input.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void InputItem(string item)
		{
			if (!editable)
			{
				return;
			}

			if (string.IsNullOrEmpty(item))
			{
				return;
			}

			if (!listView.DataSource.Contains(item))
			{
				if (AllowNewItems)
				{
					var index = listView.Add(item);
					listView.Select(index);
				}
				else if (ResetInput)
				{
					Input.text = string.Empty;
				}
			}
			else
			{
				var index = listView.DataSource.IndexOf(item);
				listView.Select(index);
			}
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			ListViewPosition.ParentDestroyed();

			ListView = null;
			ToggleButton = null;
			if (Input != null)
			{
				Input.onEndEdit.RemoveListener(InputItem);
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		protected void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected void OnListViewCancel(int index, ListViewItem item, BaseEventData eventData)
		{
			HideList();
		}

		/// <summary>
		/// Called when OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			ShowList();
		}

		#if UNITY_EDITOR
		/// <inheritdoc/>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (parentCanvas == null)
			{
				parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <inheritdoc/>
		protected override void Reset()
		{
			base.Reset();

			if (parentCanvas == null)
			{
				parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Combobox.SingleInputBackground.ApplyTo(bg);
			}

			if ((toggleButton != null) && toggleButton.TryGetComponent<Image>(out var btn))
			{
				style.Combobox.ToggleButton.ApplyTo(btn);
			}

			if (TryGetComponent<InputField>(out var input))
			{
				style.Combobox.Input.ApplyTo(input.textComponent, true);
				if (input.placeholder != null)
				{
					style.Combobox.Placeholder.ApplyTo(input.placeholder.gameObject, true);
				}
			}

			if (listView != null)
			{
				listView.SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Combobox.SingleInputBackground.GetFrom(bg);
			}

			if ((toggleButton != null) && toggleButton.TryGetComponent<Image>(out var btn))
			{
				style.Combobox.ToggleButton.GetFrom(btn);
			}

			if (TryGetComponent<InputField>(out var input))
			{
				style.Combobox.Input.GetFrom(input.textComponent, true);
				if (input.placeholder != null)
				{
					style.Combobox.Placeholder.GetFrom(input.placeholder.gameObject, true);
				}
			}

			if (listView != null)
			{
				listView.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}