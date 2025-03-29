#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TextTMPro.
	/// </summary>
	public class TextTMPro : ITextProxy
	{
		/// <summary>
		/// Is enum value has specified flag?
		/// </summary>
		/// <param name="value">Enum value.</param>
		/// <param name="flag">Flag.</param>
		/// <returns>true if enum has flag; otherwise false.</returns>
		public static bool IsSet(FontStyles value, FontStyles flag) => (value & flag) == flag;

		/// <summary>
		/// Text component.
		/// </summary>
		protected TextMeshProUGUI Component;

		/// <summary>
		/// GameObject.
		/// </summary>
		public GameObject GameObject => Component.gameObject;

		/// <summary>
		/// Graphic component.
		/// </summary>
		public Graphic Graphic => Component;

		/// <summary>
		/// Color.
		/// </summary>
		public Color color
		{
			get => Component.color;

			set => Component.color = value;
		}

		/// <summary>
		/// Font size.
		/// </summary>
		public float fontSize
		{
			get => Component.fontSize;

			set => Component.fontSize = value;
		}

		/// <summary>
		/// Font style.
		/// </summary>
		public FontStyle fontStyle
		{
			get
			{
				if (Bold && Italic)
				{
					return FontStyle.BoldAndItalic;
				}

				if (Bold)
				{
					return FontStyle.Bold;
				}

				if (Italic)
				{
					return FontStyle.Italic;
				}

				return FontStyle.Normal;
			}

			set
			{
				Bold = (value == FontStyle.Bold) || (value == FontStyle.BoldAndItalic);
				Italic = (value == FontStyle.Italic) || (value == FontStyle.BoldAndItalic);
			}
		}

		/// <summary>
		/// Text alignment.
		/// </summary>
		public TextAnchor alignment
		{
			get => ConvertAlignment(Component.alignment);

			set => Component.alignment = ConvertAlignment(value);
		}

		TextAnchor ConvertAlignment(TextAlignmentOptions alignment)
		{
			return alignment switch
			{
				TextAlignmentOptions.TopLeft => TextAnchor.UpperLeft,
				TextAlignmentOptions.Top => TextAnchor.UpperCenter,
				TextAlignmentOptions.TopRight => TextAnchor.UpperRight,
				TextAlignmentOptions.MidlineLeft => TextAnchor.MiddleLeft,
				TextAlignmentOptions.Center => TextAnchor.MiddleCenter,
				TextAlignmentOptions.MidlineRight => TextAnchor.MiddleRight,
				TextAlignmentOptions.BottomLeft => TextAnchor.LowerLeft,
				TextAlignmentOptions.Bottom => TextAnchor.LowerCenter,
				TextAlignmentOptions.BottomRight => TextAnchor.LowerRight,
				_ => TextAnchor.UpperLeft,
			};
		}

		TextAlignmentOptions ConvertAlignment(TextAnchor alignment)
		{
			return alignment switch
			{
				TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
				TextAnchor.UpperCenter => TextAlignmentOptions.Top,
				TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
				TextAnchor.MiddleLeft => TextAlignmentOptions.MidlineLeft,
				TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
				TextAnchor.MiddleRight => TextAlignmentOptions.MidlineRight,
				TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
				TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
				TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
				_ => TextAlignmentOptions.TopLeft,
			};
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get => IsSet(Component.fontStyle, FontStyles.Bold);

			set
			{
				if (Bold == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle |= FontStyles.Bold;
				}
				else
				{
					Component.fontStyle &= ~FontStyles.Bold;
				}
			}
		}

		/// <summary>
		/// Italic.
		/// </summary>
		public bool Italic
		{
			get => IsSet(Component.fontStyle, FontStyles.Italic);

			set
			{
				if (Italic == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle |= FontStyles.Italic;
				}
				else
				{
					Component.fontStyle &= ~FontStyles.Italic;
				}
			}
		}

		/// <summary>
		/// Text.
		/// </summary>
		public string text
		{
			get => Component.text;

			set => Component.text = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextTMPro"/> class.
		/// </summary>
		/// <param name="component">Component.</param>
		public TextTMPro(TextMeshProUGUI component)
		{
			Component = component;
		}
	}
}
#endif