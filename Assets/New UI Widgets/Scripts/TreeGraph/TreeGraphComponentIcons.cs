namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeGraph component with icons.
	/// </summary>
	public class TreeGraphComponentIcons : TreeGraphComponent<TreeViewItem>, IUpgradeable, ILocalizationSupport
	{
		[SerializeField]
		[Tooltip("If enabled translates item name using Localization.GetTranslation().")]
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
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsForeground = new Graphic[] { UtilitiesUI.GetGraphic(NameAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public override void LocaleChanged()
		{
			UpdateName();
		}

		/// <summary>
		/// Update display name.
		/// </summary>
		protected virtual void UpdateName()
		{
			NameAdapter.text = Node.Item.LocalizedName ?? (LocalizationSupport ? Localization.GetTranslation(Node.Item.Name) : Node.Item.Name);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(TreeNode<TreeViewItem> node)
		{
			Node = node;

			name = Node.Item.Name;
			UpdateName();
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Name, ref NameAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			Compatibility.Upgrade(this);

			base.OnValidate();
		}
#endif
	}
}