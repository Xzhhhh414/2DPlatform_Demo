namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TextUnity.
	/// </summary>
	public class TextUnity : ITextProxy
	{
		/// <summary>
		/// Text component.
		/// </summary>
		protected Text Component;

		/// <summary>
		/// GameObject.
		/// </summary>
		public GameObject GameObject => (Component != null) ? Component.gameObject : null;

		/// <summary>
		/// Text.
		/// </summary>
		public string text
		{
			get => Component.text;

			set => Component.text = value;
		}

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

			set => Component.fontSize = Mathf.RoundToInt(value);
		}

		/// <summary>
		/// Font style.
		/// </summary>
		public FontStyle fontStyle
		{
			get => Component.fontStyle;

			set => Component.fontStyle = value;
		}

		/// <summary>
		/// Text alignment.
		/// </summary>
		public TextAnchor alignment
		{
			get => Component.alignment;

			set => Component.alignment = value;
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get => (Component.fontStyle == FontStyle.Bold) || (Component.fontStyle == FontStyle.BoldAndItalic);

			set
			{
				if (Bold == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Normal)
						? FontStyle.Bold
						: FontStyle.BoldAndItalic;
				}
				else
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Bold)
						? FontStyle.Normal
						: FontStyle.Italic;
				}
			}
		}

		/// <summary>
		/// Italic.
		/// </summary>
		public bool Italic
		{
			get => (Component.fontStyle == FontStyle.Italic) || (Component.fontStyle == FontStyle.BoldAndItalic);

			set
			{
				if (Italic == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Normal)
						? FontStyle.Italic
						: FontStyle.BoldAndItalic;
				}
				else
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Italic)
						? FontStyle.Normal
						: FontStyle.Bold;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextUnity"/> class.
		/// </summary>
		/// <param name="component">Component.</param>
		public TextUnity(Text component)
		{
			Component = component;
		}
	}
}