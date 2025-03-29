namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Tab component.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/containers/tabs.html")]
	public class TabButtonComponentBase : MonoBehaviour, IUpgradeable, ISelectHandler
	{
		[SerializeField]
		[Tooltip("If enabled translates button name using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get
			{
				return localizationSupport;
			}

			set
			{
				localizationSupport = value;
			}
		}

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Select event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnSelectEvent = new UnityEvent();

		/// <summary>
		/// Select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			OnSelectEvent.Invoke();
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void SetButtonData(Tab tab)
		{
			NameAdapter.text = LocalizationSupport ? Localization.GetTranslation(tab.Name) : tab.Name;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}