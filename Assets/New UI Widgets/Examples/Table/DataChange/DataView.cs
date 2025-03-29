namespace UIWidgets.Examples.DataChange
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Data view.
	/// </summary>
	public class DataView : ListViewItem, IViewData<Data>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public TextAdapter Value;

		/// <summary>
		/// Value background.
		/// </summary>
		[SerializeField]
		public Image ValueBackground;

		[SerializeField]
		[FormerlySerializedAs("ColorUnchanged")]
		Color colorUnchanged = Color.black;

		/// <summary>
		/// Color of unchanged value.
		/// </summary>
		public Color ColorUnchanged
		{
			get
			{
				return colorUnchanged;
			}

			set
			{
				colorUnchanged = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ColorDecrease")]
		Color colorDecrease = Color.red;

		/// <summary>
		/// Color of the decreased value.
		/// </summary>
		public Color ColorDecrease
		{
			get
			{
				return colorDecrease;
			}

			set
			{
				colorDecrease = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ColorIncrease")]
		Color colorIncrease = Color.green;

		/// <summary>
		/// Color of the increased value.
		/// </summary>
		public Color ColorIncrease
		{
			get
			{
				return colorIncrease;
			}

			set
			{
				colorIncrease = value;
			}
		}

		Data Item;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(Name),
					UtilitiesUI.GetGraphic(Value),
				};
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				#pragma warning restore
				GraphicsBackgroundVersion = 1;
			}

			base.GraphicsBackgroundInit();
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Data item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			Name.text = Item.Name;
			Value.text = Item.Value.ToString();
			if (Item.Difference == 0)
			{
				ValueBackground.color = ColorUnchanged;
			}
			else
			{
				ValueBackground.color = (Item.Difference > 0) ? ColorIncrease : ColorDecrease;
			}
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), ValueBackground, nameof(ValueBackground.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), ValueBackground, nameof(ValueBackground.color), owner);
		}
	}
}