namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test ComboboxIcons.
	/// </summary>
	public class TestComboboxIcons : MonoBehaviour
	{
		/// <summary>
		/// ComboboxIcons.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("comboboxIcons")]
		protected ComboboxIcons ComboboxIcons;

		/// <summary>
		/// Sample icon to test adding items with icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("sampleIcon")]
		protected Sprite SampleIcon;

		/// <summary>
		/// New items.
		/// </summary>
		[SerializeField]
		protected List<ListViewIconsItemDescription> NewItems = new List<ListViewIconsItemDescription>()
		{
			new ListViewIconsItemDescription()
			{
				Icon = null,
				Name = "Test Item 0",
			},
			new ListViewIconsItemDescription()
			{
				Icon = null,
				Name = "Test Item 1",
			},
			new ListViewIconsItemDescription()
			{
				Icon = null,
				Name = "Test Item 2",
			},
		};

		/// <summary>
		/// Awake this instance.
		/// </summary>
		public void Awake()
		{
			ComboboxIcons.Init();

			GetSelected();
		}

		/// <summary>
		/// Get selected item.
		/// </summary>
		public void GetSelected()
		{
			// Get last selected index
			Debug.Log(ComboboxIcons.ListView.SelectedIndex.ToString());

			// Get last selected string
			Debug.Log(ComboboxIcons.ListView.SelectedItem.Name);
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		public void Remove()
		{
			// Deleting specified item
			var items = ComboboxIcons.ListView.DataSource;
			items.Remove(items[0]);
		}

		/// <summary>
		/// Remove item at position.
		/// </summary>
		public void RemoveAt()
		{
			// Deleting item by index
			ComboboxIcons.ListView.DataSource.RemoveAt(0);
		}

		/// <summary>
		/// Clear items list.
		/// </summary>
		public void Clear()
		{
			ComboboxIcons.Clear();
		}

		/// <summary>
		/// Add item.
		/// </summary>
		public void AddItem()
		{
			var new_item = new ListViewIconsItemDescription()
			{
				Icon = SampleIcon,
				Name = "test item",
			};
			ComboboxIcons.ListView.DataSource.Add(new_item);
		}

		/// <summary>
		/// Add items.
		/// </summary>
		public void AddItems()
		{
			ComboboxIcons.ListView.DataSource.AddRange(NewItems);
		}

		/// <summary>
		/// Select item.
		/// </summary>
		public void Select()
		{
			// Set selected index
			ComboboxIcons.ListView.SelectedIndex = 1;

			// or
			ComboboxIcons.ListView.Select(1);
		}
	}
}