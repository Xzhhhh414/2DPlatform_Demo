namespace UIWidgets
{
	using System;
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class ListViewIconsItemComponent : ListViewItem, IViewData<ListViewIconsItemDescription>, IViewData<TreeViewItem>
	{
		GameObject[] objectsToResize;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		[Obsolete("Unused.")]
		public GameObject[] ObjectsToResize
		{
			get
			{
				objectsToResize ??= (TextAdapter == null)
						 ? new GameObject[] { Icon.transform.parent.gameObject }
						 : new GameObject[] { Icon.transform.parent.gameObject, TextAdapter.gameObject, };

				return objectsToResize;
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// The text adapter.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewIconsItemDescription Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewIconsItemDescription item)
		{
			Item = item;

			#if UNITY_EDITOR
			name = item == null ? "DefaultItem " + Index.ToString() : item.Name;
			#endif

			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
				}

				if (TextAdapter != null)
				{
					TextAdapter.text = string.Empty;
				}
			}
			else
			{
				if (Icon != null)
				{
					Icon.sprite = Item.Icon;
				}

				if (TextAdapter != null)
				{
					UpdateName();
				}
			}

			if (Icon != null)
			{
				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.enabled = Icon.sprite != null;
			}
		}

		/// <summary>
		/// Update display name.
		/// </summary>
		protected void UpdateName()
		{
			if (Item == null)
			{
				return;
			}

			if (TextAdapter != null)
			{
				var name = Item.Name.Replace("\\n", "\n");
				var localization_support = (Owner as ILocalizationSupport)?.LocalizationSupport ?? false;
				TextAdapter.text = Item.LocalizedName ?? (localization_support ? Localization.GetTranslation(name) : name);
			}
		}

		/// <inheritdoc/>
		public override void LocaleChanged()
		{
			UpdateName();
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(TreeViewItem item)
		{
			SetData(new ListViewIconsItemDescription()
			{
				Name = item.Name,
				LocalizedName = item.LocalizedName,
				Icon = item.Icon,
				Value = item.Value,
			});
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Color), Icon, nameof(Icon.color), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Icon, nameof(Icon.sprite), owner);
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			base.MovedToCache();

			if (Icon != null)
			{
				Icon.sprite = null;
			}
		}

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

#pragma warning disable 0618
			Utilities.RequireComponent(Text, ref TextAdapter);
#pragma warning restore 0618
		}
	}
}