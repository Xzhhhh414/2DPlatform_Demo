namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.l10n;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Menu item view.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/controls/contextmenu.html")]
	public class MenuItemView : Selectable, IViewData<MenuItem>, IPointerClickHandler, ISubmitHandler, IStylable, ILocalizationSupport
	{
		/// <summary>
		/// Index.
		/// </summary>
		[NonSerialized]
		public int Index = -1;

		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Check mark.
		/// </summary>
		[SerializeField]
		public RectTransform Checkmark;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Hot key.
		/// </summary>
		[SerializeField]
		public TextAdapter HotKey;

		/// <summary>
		/// Sub menu indicator.
		/// </summary>
		[SerializeField]
		public RectTransform SubmenuIndicator;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		[SerializeField]
		public bool SetNativeSize = true;

		[SerializeField]
		[Tooltip("If enabled translates menu name using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get => localizationSupport;

			set => localizationSupport = value;
		}

		MenuItem item;

		/// <summary>
		/// Menu item.
		/// </summary>
		public MenuItem Item
		{
			get => item;

			set
			{
				if (item == value)
				{
					return;
				}

				if (item != null)
				{
					item.OnChange -= UpdateView;
				}

				item = value;

				if (item != null)
				{
					item.OnChange += UpdateView;
				}

				UpdateView();
			}
		}

		List<IPointerEnterHandler> selectableHelpersEnter;

		List<IPointerExitHandler> selectableHelpersExit;

		/// <summary>
		/// Selectable helpers on pointer enter.
		/// </summary>
		protected List<IPointerEnterHandler> SelectableHelpersEnter
		{
			get
			{
				if (selectableHelpersEnter == null)
				{
					GetSelectableHelpers();
				}

				return selectableHelpersEnter;
			}
		}

		/// <summary>
		/// Selectable helpers on pointer exit.
		/// </summary>
		protected List<IPointerExitHandler> SelectableHelpersExit
		{
			get
			{
				if (selectableHelpersExit == null)
				{
					GetSelectableHelpers();
				}

				return selectableHelpersExit;
			}
		}

		/// <summary>
		/// Click event.
		/// </summary>
		[SerializeField]
		public MenuItemEvent OnClick = new MenuItemEvent();

		/// <summary>
		/// Enter event.
		/// </summary>
		[SerializeField]
		public MenuItemOverEvent OnEnter = new MenuItemOverEvent();

		/// <summary>
		/// Exit event.
		/// </summary>
		[SerializeField]
		public MenuItemOverEvent OnExit = new MenuItemOverEvent();

		/// <summary>
		/// Move event.
		/// </summary>
		[SerializeField]
		public MenuItemMoveEvent OnAxisMove = new MenuItemMoveEvent();

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			Item = null;
		}

		/// <summary>
		/// Get selectable helpers.
		/// </summary>
		protected void GetSelectableHelpers()
		{
			using var _ = ListPool<SelectableHelper>.Get(out var sh);
			GetComponents(sh);

			using var __ = ListPool<SelectableHelperList>.Get(out var shl);
			GetComponents(shl);

			selectableHelpersEnter = new List<IPointerEnterHandler>();
			selectableHelpersEnter.AddRange(sh);
			selectableHelpersEnter.AddRange(shl);

			selectableHelpersExit = new List<IPointerExitHandler>();
			selectableHelpersExit.AddRange(sh);
			selectableHelpersExit.AddRange(shl);
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			UpdateName();
		}

		/// <summary>
		/// Update display name.
		/// </summary>
		protected virtual void UpdateName()
		{
			if (Item == null)
			{
				return;
			}

			if (Name != null)
			{
				Name.text = LocalizationSupport ? Localization.GetTranslation(Item.Name) : Item.Name;
			}
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
					Icon.enabled = false;
				}

				return;
			}

			name = Item.Name;
			interactable = Item.Interactable;

			UpdateName();

			if (Checkmark != null)
			{
				Checkmark.gameObject.SetActive(Item.Checked);
			}

			if (SubmenuIndicator != null)
			{
				SubmenuIndicator.gameObject.SetActive(Item.HasVisibleItems);
			}

			if (HotKey != null)
			{
				HotKey.gameObject.SetActive(Item.HotKey.Valid);
				HotKey.text = Item.HotKey.ToString();
			}

			if (Icon != null)
			{
				Icon.sprite = Item.Icon;

				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.enabled = Icon.sprite != null;
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(MenuItem item)
		{
			Item = item;

			UpdateView();
		}

		/// <summary>
		/// Process the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnPointerEnter(eventData);

			OnEnter.Invoke(Index, true);
		}

		/// <summary>
		/// Highlight.
		/// </summary>
		public virtual void SelectableHighlight()
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnPointerEnter(null);
			foreach (var s in SelectableHelpersEnter)
			{
				s.OnPointerEnter(null);
			}
		}

		/// <summary>
		/// Highlight.
		/// </summary>
		public virtual void SelectableDefault()
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnPointerExit(null);
			foreach (var s in SelectableHelpersExit)
			{
				s.OnPointerExit(null);
			}
		}

		/// <summary>
		/// Process the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnPointerExit(eventData);

			OnExit.Invoke(Index, true);
		}

		/// <summary>
		/// Process the move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnMove(eventData);

			OnAxisMove.Invoke(Index, eventData);
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			OnClick.Invoke(Item);
		}

		/// <summary>
		/// Process the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			OnClick.Invoke(Item);
		}

		/// <summary>
		/// Process the select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnSelect(BaseEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnSelect(eventData);

			OnEnter.Invoke(Index, false);
		}

		/// <summary>
		/// Process the deselect event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnDeselect(BaseEventData eventData)
		{
			if (!IsInteractable())
			{
				return;
			}

			base.OnDeselect(eventData);

			OnExit.Invoke(Index, false);
		}

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			style.ContextMenu.ItemBackground.ApplyTo(targetGraphic);
			style.ContextMenu.ItemBackgroundSelectable.ApplyTo(this);

			using var _ = ListPool<SelectableHelper>.Get(out var sh);
			GetComponents(sh);
			foreach (var s in sh)
			{
				style.ContextMenu.ItemTextSelectable.ApplyTo(s);
			}

			if (Checkmark != null)
			{
				style.ContextMenu.ItemText.ApplyTo(Checkmark);
			}

			if (HotKey != null)
			{
				style.ContextMenu.ItemText.ApplyTo(HotKey.gameObject);
			}

			if (Name != null)
			{
				style.ContextMenu.ItemText.ApplyTo(Name.gameObject);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.ContextMenu.ItemBackground.GetFrom(targetGraphic);
			style.ContextMenu.ItemBackgroundSelectable.GetFrom(this);

			using var _ = ListPool<SelectableHelper>.Get(out var sh);
			foreach (var s in sh)
			{
				style.ContextMenu.ItemTextSelectable.GetFrom(s);
			}

			if (Checkmark != null)
			{
				style.ContextMenu.ItemText.GetFrom(Checkmark);
			}

			if (HotKey != null)
			{
				style.ContextMenu.ItemText.GetFrom(HotKey.gameObject);
			}

			if (Name != null)
			{
				style.ContextMenu.ItemText.GetFrom(Name.gameObject);
			}

			return true;
		}

		/// <summary>
		/// Set delimiter style.
		/// </summary>
		/// <param name="style">Style.</param>
		public void SetDelimiterStyle(StyleImage style)
		{
			style.ApplyTo(targetGraphic);
		}

		/// <summary>
		/// Set delimiter style options from widget properties.
		/// </summary>
		/// <param name="style">Style.</param>
		public void GetDelimiterStyle(StyleImage style)
		{
			style.GetFrom(targetGraphic);
		}
	}
}