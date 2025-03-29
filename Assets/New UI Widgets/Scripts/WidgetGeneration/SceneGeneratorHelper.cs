namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Scene generator helper.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class SceneGeneratorHelper : MonoBehaviour
	{
		/// <summary>
		/// Canvas.
		/// </summary>
		[SerializeField]
		public GameObject Canvas;

		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		public Accordion Accordion;

		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		public Tabs Tabs;

		/// <summary>
		/// ListView's parent.
		/// </summary>
		[SerializeField]
		public RectTransform ListsParent;

		/// <summary>
		/// TileView parent.
		/// </summary>
		[SerializeField]
		public RectTransform TileViewParent;

		/// <summary>
		/// TreeView parent.
		/// </summary>
		[SerializeField]
		public RectTransform TreeViewParent;

		/// <summary>
		/// ListView label.
		/// </summary>
		[SerializeField]
		public GameObject LabelListView;

		/// <summary>
		/// ListView button.
		/// </summary>
		[SerializeField]
		public Button ListViewButton;

		/// <summary>
		/// TreeView button.
		/// </summary>
		[SerializeField]
		public Button TreeViewButton;

		/// <summary>
		/// First button to toggle style/theme.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("StyleDefaultButton")]
		public Button StyleButton01;

		/// <summary>
		/// Second button to toggle style/theme.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("StyleBlueButton")]
		public Button StyleButton02;

		/// <summary>
		/// Third button to toggle style/theme.
		/// </summary>
		[SerializeField]
		public Button StyleButton03;

		/// <summary>
		/// Fourth button to toggle style/theme.
		/// </summary>
		[SerializeField]
		public Button StyleButton04;

		/// <summary>
		/// Table parent.
		/// </summary>
		[SerializeField]
		public RectTransform TableParent;

		/// <summary>
		/// Table label.
		/// </summary>
		[SerializeField]
		public GameObject LabelTable;

		/// <summary>
		/// TreeGraph parent.
		/// </summary>
		[SerializeField]
		public RectTransform TreeGraphParent;

		/// <summary>
		/// TreeGraph label.
		/// </summary>
		[SerializeField]
		public GameObject LabelTreeGraph;

		/// <summary>
		/// Autocomplete parent.
		/// </summary>
		[SerializeField]
		public RectTransform AutocompleteParent;

		/// <summary>
		/// Autocomplete label.
		/// </summary>
		[SerializeField]
		public GameObject LabelAutocomplete;

		/// <summary>
		/// AutoCombobox label.
		/// </summary>
		[SerializeField]
		public GameObject LabelAutoCombobox;

		/// <summary>
		/// Combobox label.
		/// </summary>
		[SerializeField]
		public GameObject LabelCombobox;

		/// <summary>
		/// ComboboxMultiselect label.
		/// </summary>
		[SerializeField]
		public GameObject LabelComboboxMultiselect;
	}
}