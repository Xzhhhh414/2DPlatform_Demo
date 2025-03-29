namespace UIWidgets.Examples.Inventory
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Item view.
	/// </summary>
	public class ItemView : ListViewItem, IViewData<Item>
	{
		/// <summary>
		/// Name view.
		/// </summary>
		[SerializeField]
		public TextAdapter NameView;

		/// <summary>
		/// Color view.
		/// </summary>
		[SerializeField]
		public Graphic ColorView;

		/// <summary>
		/// Weight view.
		/// </summary>
		[SerializeField]
		public TextAdapter WeightView;

		/// <summary>
		/// Price view.
		/// </summary>
		[SerializeField]
		public TextAdapter PriceView;

		[SerializeField]
		[FormerlySerializedAs("EmptyColor")]
		Color emptyColor = Color.clear;

		/// <summary>
		/// Empty color.
		/// </summary>
		public Color EmptyColor
		{
			get
			{
				return emptyColor;
			}

			set
			{
				emptyColor = value;
			}
		}

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		protected InventoryTooltip Tooltip;

		/// <summary>
		/// Item.
		/// </summary>
		public Item Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Slot index.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public int SlotIndex = -1;

		/// <summary>
		/// Get drag data.
		/// </summary>
		/// <returns>Drag data.</returns>
		public virtual ItemDragData GetDragData()
		{
			return (Owner != null)
				? ItemDragData.Inventory(Item, Index)
				: ItemDragData.Slot(Item, SlotIndex);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(Item item)
		{
			Item = item;
			UpdateView();

			if (Tooltip != null)
			{
				if (Item != null)
				{
					Tooltip.Register(gameObject, Item, new TooltipSettings(TooltipPosition.TopLeft));
				}
				else
				{
					Tooltip.Unregister(gameObject);
				}
			}
		}

		/// <inheritdoc/>
		public override void LocaleChanged()
		{
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			var not_null = Item != null;

			if (Owner != null)
			{
				name = not_null ? Item.Name : "Index " + Index.ToString();
			}

			if (ColorView != null)
			{
				ColorView.color = not_null ? Item.Color : EmptyColor;
			}

			if (NameView != null)
			{
				NameView.text = not_null ? Item.Name : string.Empty;
			}

			if (WeightView != null)
			{
				WeightView.text = not_null ? Item.Weight.ToString("0.00") : string.Empty;
			}

			if (PriceView != null)
			{
				PriceView.text = not_null ? Item.Price.ToString() : string.Empty;
			}
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), ColorView, nameof(Image.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), ColorView, nameof(ColorView.color), owner);
		}
	}
}