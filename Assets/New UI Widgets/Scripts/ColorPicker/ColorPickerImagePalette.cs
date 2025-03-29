namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker Image.
	/// You can use it to get colors from custom Texture2D.
	/// The texture must have the Read/Write Enabled flag set in the import settings, otherwise this function will fail.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/colorpicker.html")]
	public class ColorPickerImagePalette : ColorPickerBlock
	{
		/// <summary>
		/// Coordinates.
		/// </summary>
		protected readonly struct Coordinates : IEquatable<Coordinates>
		{
			/// <summary>
			/// X.
			/// </summary>
			public readonly int X;

			/// <summary>
			/// Y.
			/// </summary>
			public readonly int Y;

			/// <summary>
			/// Is valid?
			/// </summary>
			public readonly bool Valid => (X > -1) && (Y > -1);

			/// <summary>
			/// Initializes a new instance of the <see cref="Coordinates"/> struct.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="y">Y.</param>
			public Coordinates(int x, int y)
			{
				X = x;
				Y = y;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is Coordinates coordinates) && Equals(coordinates);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(Coordinates other) => (X == other.X) && (Y == other.Y);

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override readonly int GetHashCode() => X ^ Y;

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(Coordinates a, Coordinates b) => a.Equals(b);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(Coordinates a, Coordinates b) => !a.Equals(b);
		}

		[SerializeField]
		[Tooltip("The texture must have the Read/Write Enabled flag set in the import settings, otherwise this function will fail.")]
		Image image;

		RectTransform imageRect;

		DragListener dragListener;

		ClickListener clickListener;

		/// <summary>
		/// Gets or sets the palette.
		/// </summary>
		/// <value>The palette.</value>
		public Image Image
		{
			get => image;

			set => SetImage(value);
		}

		[SerializeField]
		RectTransform imageCursor;

		/// <summary>
		/// Gets or sets the palette cursor.
		/// </summary>
		/// <value>The palette cursor.</value>
		public RectTransform ImageCursor
		{
			get => imageCursor;

			set
			{
				imageCursor = value;
				if (imageCursor != null)
				{
					UpdateView();
				}
			}
		}

		ColorPickerInputMode inputMode;

		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		public ColorPickerInputMode InputMode
		{
			get => inputMode;

			set => inputMode = value;
		}

		ColorPickerPaletteMode paletteMode;

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		public ColorPickerPaletteMode PaletteMode
		{
			get => paletteMode;

			set => SetPaletteMode(value);
		}

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		public ColorRGBChangedEvent OnChangeRGB = new ColorRGBChangedEvent();

		/// <summary>
		/// OnChangeHSV event.
		/// </summary>
		public ColorHSVChangedEvent OnChangeHSV = new ColorHSVChangedEvent();

		/// <summary>
		/// OnChangeAlpha event.
		/// </summary>
		public ColorAlphaChangedEvent OnChangeAlpha = new ColorAlphaChangedEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Image = image;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Sets the palette.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetImage(Image value)
		{
			if (dragListener != null)
			{
				dragListener.OnDragEvent.RemoveListener(OnDrag);
			}

			image = value;
			if (image != null)
			{
				imageRect = image.transform as RectTransform;

				dragListener = Utilities.RequireComponent<DragListener>(image);
				dragListener.OnDragEvent.AddListener(OnDrag);

				clickListener = Utilities.RequireComponent<ClickListener>(image);
				clickListener.ClickEvent.AddListener(OnDrag);
			}
			else
			{
				imageRect = null;
			}
		}

		/// <summary>
		/// Sets the palette mode.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetPaletteMode(ColorPickerPaletteMode value)
		{
			paletteMode = value;
			var is_active = paletteMode == ColorPickerPaletteMode.Image;
			gameObject.SetActive(is_active);
			if (is_active)
			{
				UpdateView();
			}
		}

		/// <summary>
		/// If in update mode?
		/// </summary>
		protected bool inUpdateMode;

		/// <summary>
		/// Values the changed.
		/// </summary>
		protected virtual void ValueChanged()
		{
			if (!IsInteractable() || inUpdateMode)
			{
				return;
			}

			currentColor = GetColor();

			OnChangeRGB.Invoke(currentColor);
		}

		/// <summary>
		/// Current color.
		/// </summary>
		protected Color32 currentColor;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return (eventData.button == DragButton) && IsInteractable();
		}

		/// <summary>
		/// When drag is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			if (!imageCursor.gameObject.activeSelf)
			{
				imageCursor.gameObject.SetActive(true);
			}

			Vector2 size = imageRect.rect.size;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, eventData.position, eventData.pressEventCamera, out var cur_pos);

			cur_pos.x = Mathf.Clamp(cur_pos.x, 0, size.x);
			cur_pos.y = Mathf.Clamp(cur_pos.y, -size.y, 0);
			imageCursor.localPosition = cur_pos;

			ValueChanged();
		}

		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <returns>The color.</returns>
		protected Color32 GetColor()
		{
			return paletteMode switch
			{
				ColorPickerPaletteMode.Image => (Color32)GetPixelColorUnderCursor(),
				_ => currentColor,
			};
		}

		/// <summary>
		/// Gets the pixel color under cursor.
		/// </summary>
		/// <returns>The pixel color under cursor.</returns>
		protected virtual Color GetPixelColorUnderCursor()
		{
			var cursor = imageCursor.localPosition;
			var size = imageRect.rect.size;

			var texture = Image.sprite.texture;
			var x = Mathf.RoundToInt(cursor.x / size.x * texture.width);
			var y = texture.height - Mathf.RoundToInt(-cursor.y / size.y * texture.height);

			return texture.GetPixel(x, y);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();
		}

		/// <summary>
		/// Difference between colors.
		/// </summary>
		/// <returns>The delta, 0 if colors equal.</returns>
		/// <param name="x">The first color.</param>
		/// <param name="y">The second color.</param>
		protected virtual float ColorDelta(Color x, Color y)
		{
			return Mathf.Abs(x.r - y.r) + Mathf.Abs(x.g - y.g) + Mathf.Abs(x.b - y.b);
		}

		/// <summary>
		/// Cache texture data.
		/// </summary>
		public bool CacheTextureData = true;

		/// <summary>
		/// The cached texture identifier.
		/// </summary>
		protected InstanceID CachedTextureId;

		/// <summary>
		/// Cached texture pixels.
		/// </summary>
#if UNITY_2018_4_OR_NEWER
		protected Unity.Collections.NativeArray<Color> CachedTexturePixels;
#else
		protected Color[] CachedTexturePixels;
#endif

		/// <summary>
		/// Get index of color in texture.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <returns>Index.</returns>
		protected int ColorIndex(Color color)
		{
			for (int i = 0; i < CachedTexturePixels.Length; i++)
			{
				if (CachedTexturePixels[i] == color)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Get color coordinates with specified texture.
		/// </summary>
		/// <returns>The texture coordinates, [-1, -1] if not found.</returns>
		/// <param name="color">Color.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="diff">Allowable difference between colors.</param>
		protected Coordinates Color2TextureCoords(Color color, Texture2D texture, float diff = 0f)
		{
			if (CacheTextureData)
			{
				var texture_id = new InstanceID(texture);
				if ((CachedTextureId != texture_id) || (CachedTexturePixels.Length == 0))
				{
					CachedTextureId = texture_id;
#if UNITY_2018_4_OR_NEWER
					CachedTexturePixels = texture.GetRawTextureData<Color>();
#else
					CachedTexturePixels = texture.GetPixels();
#endif
				}

				var pos = ColorIndex(color);

				if (pos != -1)
				{
					int texture_y = pos / texture.width;
					int texture_x = pos - (texture_y * texture.width);

					return new Coordinates(texture_x, texture_y);
				}
				else
				{
					return new Coordinates(-1, -1);
				}
			}
			else
			{
#if UNITY_2018_4_OR_NEWER
				var pixels = texture.GetRawTextureData<Color>();
#else
				var pixels = texture.GetPixels();
#endif
				int pos = 0;
				float delta = ColorDelta(color, pixels[0]);

				for (int i = 1; i < pixels.Length; i++)
				{
					var new_delta = diff > 0
						? ColorDelta(color, pixels[i])
						: (color == pixels[i] ? 0 : 1);
					if (new_delta < delta)
					{
						delta = new_delta;
						pos = i;
						if (delta == 0)
						{
							break;
						}
					}
				}

				if (delta <= diff)
				{
					int texture_y = pos / texture.width;
					int texture_x = pos - (texture_y * texture.width);

					return new Coordinates(texture_x, texture_y);
				}
				else
				{
					return new Coordinates(-1, -1);
				}
			}
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			if (Image.sprite == null)
			{
				return;
			}

			inUpdateMode = true;

			// set image drag position
			if ((imageCursor != null) && (image != null) && (imageRect != null) && (GetPixelColorUnderCursor() != currentColor))
			{
				var texture = Image.sprite.texture;

				var coords = Color2TextureCoords(currentColor, texture);

				if (!coords.Valid)
				{
					imageCursor.gameObject.SetActive(false);
				}
				else
				{
					var size = imageRect.rect.size;
					var x = coords.X / (float)texture.width;
					var y = coords.Y / (float)texture.height;

					imageCursor.gameObject.SetActive(true);
					imageCursor.localPosition = new Vector3(x * size.x, (y - 1) * size.y, 0);
				}
			}

			inUpdateMode = false;
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			UpdateViewReal();
		}

		/// <inheritdoc/>
		public override void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Image, nameof(Graphic.color), this);
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnDestroy()
		{
			dragListener = null;
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (image.transform.parent.TryGetComponent<Image>(out var border))
			{
				styleColorPicker.PaletteBorder.ApplyTo(border);
			}

			if ((imageCursor != null) && imageCursor.TryGetComponent<Image>(out var cursor))
			{
				styleColorPicker.PaletteCursor.ApplyTo(cursor);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (image.transform.parent.TryGetComponent<Image>(out var border))
			{
				styleColorPicker.PaletteBorder.GetFrom(border);
			}

			if ((imageCursor != null) && imageCursor.TryGetComponent<Image>(out var cursor))
			{
				styleColorPicker.PaletteCursor.GetFrom(cursor);
			}
		}
	}
}