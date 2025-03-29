#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEditor.UIElements;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Combobox field.
	/// </summary>
	public class ComboboxField : VisualElement, INotifyValueChanged<OptionId>
	{
		#if !UNITY_6000_0_OR_NEWER
		/// <summary>
		/// Uxml traits.
		/// </summary>
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		/// <summary>
		/// Uxml factory.
		/// </summary>
		public new class UxmlFactory : UxmlFactory<ComboboxField, UxmlTraits>
		{
		}
		#endif

		[SerializeField]
		OptionId currentValue = new OptionId(-2);

		/// <summary>
		/// Value.
		/// </summary>
		public virtual OptionId value
		{
			get
			{
				return currentValue;
			}

			set
			{
				if (currentValue == value)
				{
					return;
				}

				if (panel != null)
				{
					using var ev = ChangeEvent<OptionId>.GetPooled(currentValue, value);

					ev.target = this;
					SetValueWithoutNotify(value);
					SendEvent(ev);
				}
				else
				{
					SetValueWithoutNotify(value);
				}
			}
		}

		IReadOnlyList<Option> options;

		VisualElement rootElement;

		TextElement textElement;

		VisualElement arrowElement;

		/// <summary>
		/// USS combobox classname.
		/// </summary>
		public const string UssComboboxClassName = "theme-combobox-field";

		/// <summary>
		/// Initializes a new instance of the <see cref="ComboboxField"/> class.
		/// </summary>
		public ComboboxField()
			: this(OptionId.None, new List<Option>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComboboxField"/> class.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		/// <param name="options">Options.</param>
		public ComboboxField(OptionId defaultValue, IReadOnlyList<Option> options)
		{
			var base_type = GetType().BaseType;
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var is_composite_root = base_type.GetProperty("isCompositeRoot", flags);
			is_composite_root?.SetValue(this, true);

			focusable = true;
			tabIndex = 0;

			var exclude_from_focus_ring = base_type.GetProperty("excludeFromFocusRing", flags);
			exclude_from_focus_ring?.SetValue(this, true);

			delegatesFocus = true;

			AddToClassList(UssComboboxClassName);
			AddToClassList(EnumField.ussClassName);
			AddToClassList(EnumField.noLabelVariantUssClassName);

			CreateElements();
			SetOptions(defaultValue, options);
		}

		void CreateElements()
		{
			rootElement = new VisualElement()
			{
				pickingMode = PickingMode.Ignore,
				focusable = true,
			};
			rootElement.AddToClassList(EnumField.inputUssClassName);
			Add(rootElement);

			textElement = new TextElement()
			{
				pickingMode = PickingMode.Ignore,
			};
			textElement.AddToClassList(EnumField.textUssClassName);
			rootElement.Add(textElement);

			arrowElement = new VisualElement()
			{
				pickingMode = PickingMode.Ignore,
			};
			arrowElement.AddToClassList(EnumField.arrowUssClassName);
			rootElement.Add(arrowElement);
		}

		void SetOptions(OptionId defaultValue, IReadOnlyList<Option> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));

			SetValueWithoutNotify(defaultValue);

			if (this.options.Count == 0)
			{
				rootElement.AddToClassList(disabledUssClassName);
				AddToClassList(disabledUssClassName);
			}
		}

		/// <summary>
		/// Set value without notification.
		/// </summary>
		/// <param name="newValue">New value.</param>
		public virtual void SetValueWithoutNotify(OptionId newValue)
		{
			if (currentValue == newValue)
			{
				return;
			}

			currentValue = newValue;
			textElement.text = GetOptionName(currentValue);
		}

		string GetOptionName(OptionId optionId)
		{
			if (options.Count == 0)
			{
				return "List is Empty";
			}

			foreach (var option in options)
			{
				if (option.Id == optionId)
				{
					return option.Name;
				}
			}

			return "None";
		}

		/// <summary>
		/// Execute default action at target.
		/// </summary>
		/// <param name="ev">Event.</param>
		#if UNITY_2023_2_OR_NEWER
		protected override void HandleEventBubbleUp(EventBase ev)
		#else
		protected override void ExecuteDefaultActionAtTarget(EventBase ev)
		#endif
		{
			#if UNITY_2023_2_OR_NEWER
			base.HandleEventBubbleUp(ev);
			#else
			base.ExecuteDefaultActionAtTarget(ev);
			#endif

			if (ShouldShowOptions(ev) && (options.Count > 0))
			{
				ShowOptions();
				ev.StopPropagation();
			}
		}

		bool ShouldShowOptions(EventBase ev)
		{
			if (ev == null)
			{
				return false;
			}

			if (ev is KeyDownEvent keyDownEvent)
			{
				return (keyDownEvent.keyCode == KeyCode.Space)
					|| (keyDownEvent.keyCode == KeyCode.KeypadEnter)
					|| (keyDownEvent.keyCode == KeyCode.Return);
			}

			if (ev is MouseDownEvent mouseDownEvent)
			{
				var point = rootElement.WorldToLocal(mouseDownEvent.mousePosition);
				return (mouseDownEvent.button == 0) && rootElement.ContainsPoint(point);
			}

			return false;
		}

		void ShowOptions()
		{
			void Set(object menuItem) => value = (OptionId)menuItem;

			var menu = new GenericMenu
			{
				allowDuplicateNames = true,
			};

			menu.AddItem(new GUIContent("None"), value == OptionId.None, Set, OptionId.None);

			foreach (var option in options)
			{
				menu.AddItem(new GUIContent(option.Name.Replace("#", "# ")), option.Id == value, Set, option.Id);
			}

			var position = this.LocalToWorld(new Vector2(rootElement.layout.xMin, rootElement.layout.height));
			menu.DropDown(new Rect(position, Vector2.zero));
		}
	}
}
#endif