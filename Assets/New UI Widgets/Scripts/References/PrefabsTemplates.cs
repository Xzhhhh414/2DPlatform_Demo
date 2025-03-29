namespace UIWidgets
{
	using UIThemes;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Prefabs templates.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class PrefabsTemplates : ScriptableObject
	{
#if UNITY_EDITOR
		static PrefabsTemplates instance;

		/// <summary>
		/// Instance.
		/// </summary>
		public static PrefabsTemplates Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UtilitiesEditor.LoadAssetWithGUID<PrefabsTemplates>(ReferenceGUID.PrefabsTemplates);
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}

		#if UNITY_2019_3_OR_NEWER
		/// <summary>
		/// Reload support.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[UIWidgets.Attributes.DomainReload(nameof(instance))]
		static void StaticInit()
		{
			instance = null;
		}
		#endif
#endif

		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public GameObject Autocomplete;

		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public GameObject Combobox;

		/// <summary>
		/// Combobox with enabled multiselect.
		/// </summary>
		[SerializeField]
		public GameObject ComboboxMultiselect;

		/// <summary>
		/// DragInfo.
		/// </summary>
		[SerializeField]
		public GameObject DragInfo;

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public GameObject ListView;

		/// <summary>
		/// PickerListView.
		/// </summary>
		[SerializeField]
		public GameObject PickerListView;

		/// <summary>
		/// PickerTreeView.
		/// </summary>
		[SerializeField]
		public GameObject PickerTreeView;

		/// <summary>
		/// Scene.
		/// </summary>
		[SerializeField]
		public GameObject Scene;

		/// <summary>
		/// Table.
		/// </summary>
		[SerializeField]
		public GameObject Table;

		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		public GameObject TileView;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		[SerializeField]
		public GameObject TreeGraph;

		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public GameObject TreeView;

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		public GameObject Tooltip;

		/// <summary>
		/// Default style.
		/// </summary>
		[SerializeField]
		public Style StyleDefault;

		/// <summary>
		/// Blue style.
		/// </summary>
		[SerializeField]
		public Style StyleBlue;

		/// <summary>
		/// Theme.
		/// </summary>
		[SerializeField]
		public Theme Theme;
	}
}