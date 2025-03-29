#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.Reflection;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Converter functions to replace component with another component.
	/// </summary>
	public partial class ConverterTMPro
	{
		/// <summary>
		/// InputField component converter.
		/// </summary>
		public class ConverterInputField
		{
			readonly Text text;
			readonly Graphic placeholder;

			readonly bool interactable;

			readonly Selectable.Transition transition;
			readonly ColorBlock colors;
			readonly SpriteState spriteState;
			readonly AnimationTriggers animationTriggers;
			readonly GameObject imageGO;
			readonly GameObject targetGraphicGO;

			readonly Navigation navigation;
			readonly string value;
			readonly int characterLimit;

			readonly InputField.ContentType contentType;
			readonly InputField.LineType lineType;
			readonly InputField.InputType inputType;
			readonly TouchScreenKeyboardType keyboardType;
			readonly InputField.CharacterValidation characterValidation;

			readonly float caretBlinkRate;
			readonly int caretWidth;
			readonly bool customCaretColor;
			readonly Color caretColor;
			readonly Color selectionColor;

			readonly bool hideMobileInput;
			readonly bool readOnly;

			readonly object onValueChangedData;
			readonly object onEndEditData;

			readonly List<string> inputFieldRefs = new List<string>();
			readonly List<string> textRefs = new List<string>();
			readonly List<string> placeholderRefs = new List<string>();

			readonly List<RectTransform> children = new List<RectTransform>();

			readonly RectTransform textRectTransform;

			readonly SerializedObjectCache cache;

			readonly bool hasTextArea;

			[DomainReloadExclude]
			static readonly string[] ValueChangedEvent = new string[]
			{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				"m_OnValueChanged",
#else
				"m_OnValueChange",
#endif
			};

			[DomainReloadExclude]
			static readonly string[] EndEditFields = new string[] { "m_OnEndEdit", "m_OnDidEndEdit" };

			/// <summary>
			/// Initializes a new instance of the <see cref="ConverterInputField"/> class.
			/// </summary>
			/// <param name="input">Original component.</param>
			/// <param name="cache">Cache.</param>
			public ConverterInputField(InputField input, SerializedObjectCache cache)
			{
				this.cache = cache;

				var text_parent = input.textComponent.transform.parent;
				hasTextArea = text_parent.parent == input.transform;

				text = input.textComponent;
				placeholder = input.placeholder;

				interactable = input.interactable;

				transition = input.transition;
				colors = input.colors;
				spriteState = input.spriteState;
				animationTriggers = input.animationTriggers;
				imageGO = Component2GameObject(input.image);
				targetGraphicGO = Component2GameObject(input.targetGraphic);

				navigation = input.navigation;
				value = input.text;
				characterLimit = input.characterLimit;

				contentType = input.contentType;
				lineType = input.lineType;
				inputType = input.inputType;
				keyboardType = input.keyboardType;
				characterValidation = input.characterValidation;

				caretBlinkRate = input.caretBlinkRate;
				caretWidth = input.caretWidth;
				customCaretColor = input.customCaretColor;
				caretColor = input.caretColor;
				selectionColor = input.selectionColor;

				hideMobileInput = GetValue<bool>(input, "m_HideMobileInput");

				readOnly = input.readOnly;

				onValueChangedData = FieldData.GetEventData(input, ValueChangedEvent, cache);
				onEndEditData = FieldData.GetEventData(input, EndEditFields, cache);

				FindReferencesInComponent(input, input, inputFieldRefs);
				FindReferencesInComponent(input, input.textComponent, textRefs);
				if (input.placeholder != null)
				{
					FindReferencesInComponent(input, input.placeholder, placeholderRefs);
				}

				var t = input.transform;
				for (int i = 0; i < t.childCount; i++)
				{
					children.Add(t.GetChild(i) as RectTransform);
				}

				textRectTransform = input.textComponent.transform as RectTransform;
			}

			/// <summary>
			/// Get value of the protected or private field of the specified component.
			/// </summary>
			/// <typeparam name="T">Value type.</typeparam>
			/// <param name="component">Component.</param>
			/// <param name="fieldName">Field name.</param>
			/// <returns>Value.</returns>
			protected static T GetValue<T>(Component component, string fieldName)
			{
				var type = component.GetType();

				FieldInfo field;
				do
				{
					field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
					type = type.BaseType;
					if (type == null)
					{
						return default;
					}
				}
				while (field == null);

				return (T)field.GetValue(component);
			}

			/// <summary>
			/// Find references to the component in the specified source and fill replaces list.
			/// </summary>
			/// <param name="source">Component with references.</param>
			/// <param name="reference">Reference component.</param>
			/// <param name="replaces">List of the properties with references to the deleted component.</param>
			protected void FindReferencesInComponent(Component source, Component reference, List<string> replaces)
			{
				var serialized = cache.Get(source);
				var property = serialized.GetIterator();

				while (property.NextVisible(true))
				{
					if (property.propertyType != SerializedPropertyType.ObjectReference)
					{
						continue;
					}

					if (property.objectReferenceValue == null)
					{
						continue;
					}

					if (reference == (property.objectReferenceValue as Component))
					{
						replaces.Add(property.propertyPath);
					}
				}
			}

			/// <summary>
			/// Set saved values to the new TMP_InputField component.
			/// </summary>
			/// <param name="input">New component.</param>
			/// <param name="converter">Converter.</param>
			public void Set(TMPro.TMP_InputField input, ConverterTMPro converter)
			{
				RectTransform viewport;
				Vector2 viewportSizeDelta;

				if (hasTextArea)
				{
					viewport = textRectTransform.parent as RectTransform;
					viewportSizeDelta = viewport.sizeDelta;
				}
				else
				{
					var textarea = converter.CreateGameObject("Text Area");
					converter.SetParent(textarea.transform, input.transform);

					viewport = textarea.transform as RectTransform;

					viewport.localRotation = textRectTransform.localRotation;
					viewport.localPosition = textRectTransform.localPosition;
					viewport.localScale = textRectTransform.localScale;
					viewport.anchorMin = textRectTransform.anchorMin;
					viewport.anchorMax = textRectTransform.anchorMax;
					viewport.anchoredPosition = textRectTransform.anchoredPosition;
					viewport.sizeDelta = textRectTransform.sizeDelta;
					viewportSizeDelta = textRectTransform.sizeDelta;
					viewport.pivot = textRectTransform.pivot;

					foreach (var child in children)
					{
						converter.SetParent(child, textarea.transform);
					}
				}

				input.textViewport = viewport;
				foreach (var child in children)
				{
					child.localRotation = Quaternion.identity;
					child.localPosition = Vector3.zero;
					child.localScale = Vector3.one;
					child.anchorMin = Vector2.zero;
					child.anchorMax = Vector2.one;
					child.anchoredPosition = Vector2.zero;
					child.sizeDelta = Vector2.zero;
					child.pivot = new Vector2(0.5f, 0.5f);
				}

				input.textComponent = converter.Replace(text);
				var placeholder_text = placeholder as Text;
				input.placeholder = (placeholder_text != null)
					? converter.Replace(placeholder_text)
					: placeholder;

				input.interactable = interactable;

				input.transition = transition;
				input.colors = colors;
				input.spriteState = spriteState;
				input.animationTriggers = animationTriggers;
				input.image = GameObject2Component<Image>(imageGO);
				input.targetGraphic = GameObject2Component<Graphic>(targetGraphicGO);

				input.navigation = navigation;
				input.text = value;
				input.characterLimit = characterLimit;

				input.contentType = Convert(contentType);
				input.lineType = Convert(lineType);
				input.inputType = Convert(inputType);
				input.keyboardType = keyboardType;
				input.characterValidation = Convert(characterValidation);

				input.caretBlinkRate = caretBlinkRate;
				input.caretWidth = caretWidth;
				input.customCaretColor = customCaretColor;
				input.caretColor = caretColor;
				input.selectionColor = selectionColor;

				input.shouldHideMobileInput = hideMobileInput;
				input.readOnly = readOnly;

				input.fontAsset = GetTMProFont();

				viewport.sizeDelta = viewportSizeDelta;

				FieldData.SetEventData(input, ValueChangedEvent, onValueChangedData, cache);
				FieldData.SetEventData(input, EndEditFields, onEndEditData, cache);

				var s_input = cache.Get(input);

				foreach (var input_path in inputFieldRefs)
				{
					s_input.FindProperty(input_path).objectReferenceValue = input;
				}

				foreach (var text_path in textRefs)
				{
					s_input.FindProperty(text_path).objectReferenceValue = input.textComponent;
				}

				foreach (var placeholder_path in placeholderRefs)
				{
					s_input.FindProperty(placeholder_path).objectReferenceValue = input.placeholder;
				}

				UtilitiesEditor.ApplyModifiedProperties(s_input);
			}

			static GameObject Component2GameObject<T>(T component)
				where T : Component
			{
				return Utilities.IsNull(component) ? null : component.gameObject;
			}

			static T GameObject2Component<T>(GameObject go)
				where T : Component
			{
				return (go == null) ? null : go.GetComponent<T>();
			}

			static TMPro.TMP_InputField.CharacterValidation Convert(InputField.CharacterValidation validation)
			{
				return validation switch
				{
					InputField.CharacterValidation.None => TMPro.TMP_InputField.CharacterValidation.None,
					InputField.CharacterValidation.Integer => TMPro.TMP_InputField.CharacterValidation.Integer,
					InputField.CharacterValidation.Decimal => TMPro.TMP_InputField.CharacterValidation.Decimal,
					InputField.CharacterValidation.Alphanumeric => TMPro.TMP_InputField.CharacterValidation.Alphanumeric,
					InputField.CharacterValidation.Name => TMPro.TMP_InputField.CharacterValidation.Name,
					InputField.CharacterValidation.EmailAddress => TMPro.TMP_InputField.CharacterValidation.EmailAddress,
					_ => TMPro.TMP_InputField.CharacterValidation.None,
				};
			}

			static TMPro.TMP_InputField.InputType Convert(InputField.InputType inputType)
			{
				return inputType switch
				{
					InputField.InputType.Standard => TMPro.TMP_InputField.InputType.Standard,
					InputField.InputType.AutoCorrect => TMPro.TMP_InputField.InputType.AutoCorrect,
					InputField.InputType.Password => TMPro.TMP_InputField.InputType.Password,
					_ => TMPro.TMP_InputField.InputType.Standard,
				};
			}

			static TMPro.TMP_InputField.LineType Convert(InputField.LineType lineType)
			{
				return lineType switch
				{
					InputField.LineType.SingleLine => TMPro.TMP_InputField.LineType.SingleLine,
					InputField.LineType.MultiLineNewline => TMPro.TMP_InputField.LineType.MultiLineNewline,
					InputField.LineType.MultiLineSubmit => TMPro.TMP_InputField.LineType.MultiLineSubmit,
					_ => TMPro.TMP_InputField.LineType.SingleLine,
				};
			}

			static TMPro.TMP_InputField.ContentType Convert(InputField.ContentType contentType)
			{
				return contentType switch
				{
					InputField.ContentType.Standard => TMPro.TMP_InputField.ContentType.Standard,
					InputField.ContentType.Autocorrected => TMPro.TMP_InputField.ContentType.Autocorrected,
					InputField.ContentType.IntegerNumber => TMPro.TMP_InputField.ContentType.IntegerNumber,
					InputField.ContentType.DecimalNumber => TMPro.TMP_InputField.ContentType.DecimalNumber,
					InputField.ContentType.Alphanumeric => TMPro.TMP_InputField.ContentType.Alphanumeric,
					InputField.ContentType.Name => TMPro.TMP_InputField.ContentType.Name,
					InputField.ContentType.EmailAddress => TMPro.TMP_InputField.ContentType.EmailAddress,
					InputField.ContentType.Password => TMPro.TMP_InputField.ContentType.Password,
					InputField.ContentType.Pin => TMPro.TMP_InputField.ContentType.Pin,
					InputField.ContentType.Custom => TMPro.TMP_InputField.ContentType.Custom,
					_ => TMPro.TMP_InputField.ContentType.Standard,
				};
			}
		}
	}
}
#endif