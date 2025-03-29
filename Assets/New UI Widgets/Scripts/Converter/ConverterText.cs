#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Converter functions to replace component with another component.
	/// </summary>
	public partial class ConverterTMPro
	{
		/// <summary>
		/// Text component converter.
		/// </summary>
		public class ConverterText
		{
			readonly int fontSize;

			readonly FontStyle fontStyle;
			readonly TextAnchor alignment;

			readonly Color color;
			readonly string value;

			readonly bool resizeTextForBestFit;
			readonly int resizeTextMinSize;
			readonly int resizeTextMaxSize;
			readonly float lineSpacing;

			readonly HorizontalWrapMode horizontalWrapMode;
			readonly VerticalWrapMode verticalWrapMode;
			readonly bool supportRichText;

			readonly Vector2 sizeDelta;

			/// <summary>
			/// Initializes a new instance of the <see cref="ConverterText"/> class.
			/// </summary>
			/// <param name="text">Original component.</param>
			public ConverterText(Text text)
			{
				fontSize = text.fontSize;
				fontStyle = text.fontStyle;
				alignment = text.alignment;

				color = text.color;
				value = text.text;

				resizeTextForBestFit = text.resizeTextForBestFit;
				resizeTextMinSize = text.resizeTextMinSize;
				resizeTextMaxSize = text.resizeTextMaxSize;
				lineSpacing = text.lineSpacing;

				horizontalWrapMode = text.horizontalOverflow;
				verticalWrapMode = text.verticalOverflow;
				supportRichText = text.supportRichText;

				sizeDelta = (text.transform as RectTransform).sizeDelta;
			}

			/// <summary>
			/// Set saved values to the new TMP_InputField component.
			/// </summary>
			/// <param name="text">New component.</param>
			public void Set(TMPro.TextMeshProUGUI text)
			{
				text.font = GetTMProFont();

				text.fontSize = fontSize;
				text.fontStyle = Convert(fontStyle);
				text.alignment = Convert(alignment);

				text.color = color;
				text.text = value;

				text.enableAutoSizing = resizeTextForBestFit;
				text.fontSizeMin = resizeTextMinSize;
				text.fontSizeMax = resizeTextMaxSize;
				text.lineSpacing = (lineSpacing - 1) * fontSize * (98f / 36f);

				#if UNITY_2023_2_OR_NEWER || UIWIDGETS_TMPRO_3_2_OR_NEWER
				text.textWrappingMode = horizontalWrapMode == HorizontalWrapMode.Wrap ? TMPro.TextWrappingModes.Normal : TMPro.TextWrappingModes.NoWrap;
				#else
				text.enableWordWrapping = horizontalWrapMode == HorizontalWrapMode.Wrap;
				#endif
				text.overflowMode = verticalWrapMode == VerticalWrapMode.Overflow ? TMPro.TextOverflowModes.Overflow : TMPro.TextOverflowModes.Truncate;
				text.richText = supportRichText;

				(text.transform as RectTransform).sizeDelta = sizeDelta;
			}

			static TMPro.FontStyles Convert(FontStyle style)
			{
				return style switch
				{
					FontStyle.Normal => TMPro.FontStyles.Normal,
					FontStyle.Bold => TMPro.FontStyles.Bold,
					FontStyle.Italic => TMPro.FontStyles.Italic,
					FontStyle.BoldAndItalic => TMPro.FontStyles.Bold | TMPro.FontStyles.Italic,
					_ => TMPro.FontStyles.Normal,
				};
			}

			/// <summary>
			/// Convert text alignment.
			/// </summary>
			/// <param name="align">Original alignment.</param>
			/// <returns>TmPro alignment.</returns>
			static TMPro.TextAlignmentOptions Convert(TextAnchor align)
			{
				var h = align switch
				{
					TextAnchor.UpperLeft => TMPro.HorizontalAlignmentOptions.Left,
					TextAnchor.UpperCenter => TMPro.HorizontalAlignmentOptions.Center,
					TextAnchor.UpperRight => TMPro.HorizontalAlignmentOptions.Right,
					TextAnchor.MiddleLeft => TMPro.HorizontalAlignmentOptions.Left,
					TextAnchor.MiddleCenter => TMPro.HorizontalAlignmentOptions.Center,
					TextAnchor.MiddleRight => TMPro.HorizontalAlignmentOptions.Right,
					TextAnchor.LowerLeft => TMPro.HorizontalAlignmentOptions.Left,
					TextAnchor.LowerCenter => TMPro.HorizontalAlignmentOptions.Center,
					TextAnchor.LowerRight => TMPro.HorizontalAlignmentOptions.Right,
					_ => TMPro.HorizontalAlignmentOptions.Left,
				};

				var v = align switch
				{
					TextAnchor.UpperLeft => TMPro.VerticalAlignmentOptions.Top,
					TextAnchor.UpperCenter => TMPro.VerticalAlignmentOptions.Top,
					TextAnchor.UpperRight => TMPro.VerticalAlignmentOptions.Top,
					TextAnchor.MiddleLeft => TMPro.VerticalAlignmentOptions.Middle,
					TextAnchor.MiddleCenter => TMPro.VerticalAlignmentOptions.Middle,
					TextAnchor.MiddleRight => TMPro.VerticalAlignmentOptions.Middle,
					TextAnchor.LowerLeft => TMPro.VerticalAlignmentOptions.Bottom,
					TextAnchor.LowerCenter => TMPro.VerticalAlignmentOptions.Bottom,
					TextAnchor.LowerRight => TMPro.VerticalAlignmentOptions.Bottom,
					_ => TMPro.VerticalAlignmentOptions.Bottom,
				};

				return (TMPro.TextAlignmentOptions)((int)h | (int)v);
			}
		}
	}
}
#endif