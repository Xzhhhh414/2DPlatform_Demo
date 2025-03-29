namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// Base class for the generic tooltip.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/misc/tooltip.html")]
	public abstract class TooltipBase : MonoBehaviourInitiable
	{
		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform
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
		/// Current target.
		/// </summary>
		public GameObject CurrentTarget
		{
			get;
			protected set;
		}

		/// <summary>
		/// Current parent.
		/// </summary>
		protected RectTransform CurrentParent;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (CurrentTarget == null)
			{
				gameObject.SetActive(false);
			}
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!localeSubscription)
			{
				Init();

				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected abstract void UpdateView();

		/// <summary>
		/// Get settings for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Settings.</returns>
		public abstract TooltipSettings GetSettings(GameObject target);

		/// <summary>
		/// Update settings for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="settings">Settings.</param>
		/// <returns>true if target registered and settings was updated; otherwise false.</returns>
		public abstract bool UpdateSettings(GameObject target, TooltipSettings settings);

		/// <summary>
		/// Show tooltip for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		public abstract void Show(GameObject target);

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		public abstract void Hide();
	}
}