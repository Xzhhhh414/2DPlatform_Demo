#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Theme editor window.
	/// </summary>
	public partial class ThemeEditor : EditorWindow
	{
		/// <summary>
		/// Edit block view.
		/// </summary>
		public partial class EditBlockView : BlockView
		{
			/// <summary>
			/// Filter view.
			/// </summary>
			protected class FilterView
			{
				string variationFilter = string.Empty;

				/// <summary>
				/// Variation filter.
				/// </summary>
				protected string VariationFilter
				{
					get => variationFilter;

					set => SetVariationFilter(value, true);
				}

				string optionFilter = string.Empty;

				/// <summary>
				/// Option filter.
				/// </summary>
				protected string OptionFilter
				{
					get => optionFilter;

					set => SetOptionFilter(value, true);
				}

				/// <summary>
				/// Edit view.
				/// </summary>
				protected EditBlockView EditView;

				/// <summary>
				/// Block.
				/// </summary>
				public VisualElement Block
				{
					get;
					protected set;
				}

				/// <summary>
				/// Variation input.
				/// </summary>
				protected TextField VariationInput;

				/// <summary>
				/// Variation placeholder.
				/// </summary>
				protected TextElement VariationPlaceholder;

				/// <summary>
				/// Option input.
				/// </summary>
				protected TextField OptionInput;

				/// <summary>
				/// Option placeholder.
				/// </summary>
				protected TextElement OptionPlaceholder;

				/// <summary>
				/// Initializes a new instance of the <see cref="FilterView"/> class.
				/// </summary>
				/// <param name="editView">Edit view.</param>
				public FilterView(EditBlockView editView)
				{
					EditView = editView;
					Block = Filter();
				}

				/// <summary>
				/// Create filter.
				/// </summary>
				/// <returns>Root element.</returns>
				protected virtual VisualElement Filter()
				{
					var filter = new VisualElement
					{
						name = "theme-filter",
					};

					VariationInput = new TextField
					{
						name = "theme-filter-variation",
						value = VariationFilter,
					};
					VariationInput.RegisterValueChangedCallback(ev => VariationFilter = ev.newValue);

					VariationPlaceholder = new TextElement
					{
						text = "Variation Filter...",
					};
					VariationPlaceholder.AddToClassList("theme-filter-placeholder");
					VariationInput.Add(VariationPlaceholder);

					filter.Add(VariationInput);

					OptionInput = new TextField
					{
						name = "theme-filter-option",
						value = OptionFilter,
					};
					OptionInput.RegisterValueChangedCallback(ev => OptionFilter = ev.newValue);

					OptionPlaceholder = new TextElement
					{
						text = "Option Filter...",
					};
					OptionPlaceholder.AddToClassList("theme-filter-placeholder");
					OptionInput.Add(OptionPlaceholder);

					filter.Add(OptionInput);

					return filter;
				}

				/// <summary>
				/// Reset.
				/// </summary>
				public virtual void Reset()
				{
					SetVariationFilter(string.Empty, false);
					SetOptionFilter(string.Empty, false);
				}

				/// <summary>
				/// Can show variation.
				/// </summary>
				/// <param name="variation">Variation.</param>
				/// <returns>true if variation match filter; otherwise false.</returns>
				public virtual bool CanShowVariation(Variation variation)
				{
					return CanShow(variation, VariationFilter);
				}

				/// <summary>
				/// Can show option.
				/// </summary>
				/// <param name="option">Option.</param>
				/// <returns>true if option match filter; otherwise false.</returns>
				public virtual bool CanShowOption(Option option)
				{
					return CanShow(option, OptionFilter);
				}

				/// <summary>
				/// Can show named parameter.
				/// </summary>
				/// <typeparam name="TId">Type of ID.</typeparam>
				/// <param name="named">Named parameter.</param>
				/// <param name="filter">Filter.</param>
				/// <returns>true if named parameter match filter; otherwise false.</returns>
				protected virtual bool CanShow<TId>(Named<TId> named, string filter)
					where TId : IEquatable<TId>
				{
					if (string.IsNullOrEmpty(filter))
					{
						return true;
					}

					return CultureInfo.InvariantCulture.CompareInfo.IndexOf(named.Name, filter, CompareOptions.IgnoreCase) != -1;
				}

				private void SetVariationFilter(string value, bool refreshGUI)
				{
					if (variationFilter == value)
					{
						return;
					}

					VariationInput.value = value;
					variationFilter = value;

					VariationPlaceholder.style.display = string.IsNullOrEmpty(value) ? DisplayStyle.Flex : DisplayStyle.None;

					if (refreshGUI)
					{
						EditView.RefreshVariations();
						EditView.RefreshValues();
					}
				}

				private void SetOptionFilter(string value, bool refreshGUI)
				{
					if (optionFilter == value)
					{
						return;
					}

					OptionInput.value = value;
					optionFilter = value;

					OptionPlaceholder.style.display = string.IsNullOrEmpty(value) ? DisplayStyle.Flex : DisplayStyle.None;

					if (refreshGUI)
					{
						EditView.RefreshValues();
					}
				}
			}

			/// <summary>
			/// Value view.
			/// </summary>
			public readonly struct ValueView
			{
				/// <summary>
				/// Value type.
				/// </summary>
				public readonly Type Type;

				/// <summary>
				/// Label.
				/// </summary>
				public readonly string Label;

				/// <summary>
				/// Property name.
				/// </summary>
				public readonly string PropertyName;

				/// <summary>
				/// Field view.
				/// </summary>
				public readonly FieldViewBase FieldViewBase;

				/// <summary>
				/// Initializes a new instance of the <see cref="ValueView"/> struct.
				/// </summary>
				/// <param name="type">Value type.</param>
				/// <param name="propertyName">Property name.</param>
				/// <param name="fieldViewBase">Field view.</param>
				public ValueView(Type type, string propertyName, FieldViewBase fieldViewBase)
				{
					Type = type;
					PropertyName = propertyName;
					Label = ObjectNames.NicifyVariableName(propertyName);
					FieldViewBase = fieldViewBase;
				}
			}

			Theme theme;

			/// <summary>
			/// Theme.
			/// </summary>
			public Theme Theme
			{
				get => theme;

				set
				{
					if (theme != value)
					{
						FieldViewsInstances.Clear();

						theme = value;

						Filter.Reset();

						Refresh();
					}
				}
			}

			/// <summary>
			/// Cached instances created by FieldView.
			/// </summary>
			protected Dictionary<ValueKey, VisualElement> FieldViewsInstances = new Dictionary<ValueKey, VisualElement>();

			Type valueType = typeof(Color);

			/// <summary>
			/// Value type.
			/// </summary>
			public Type ValueType
			{
				get => valueType;

				set
				{
					if (valueType != value)
					{
						FieldViewsInstances.Clear();
						valueType = value;
					}

					ShowModes();
					RefreshValues();
				}
			}

			/// <summary>
			/// Header.
			/// </summary>
			protected Label Header;

			/// <summary>
			/// Modes block.
			/// </summary>
			protected VisualElement ModesBlock;

			/// <summary>
			/// Scroll block.
			/// </summary>
			protected ScrollView ScrollBlock;

			/// <summary>
			/// Variations header.
			/// </summary>
			protected VisualElement VariationsHeader;

			/// <summary>
			/// Variations list.
			/// </summary>
			protected VisualElement VariationsList;

			/// <summary>
			/// Options value block.
			/// </summary>
			protected VisualElement OptionsValuesBlock;

			/// <summary>
			/// Value views.
			/// </summary>
			protected List<ValueView> ValueViews = new List<ValueView>();

			/// <summary>
			/// Show values by type.
			/// </summary>
			protected Func<Type, bool> ShowValuesByType;

			/// <summary>
			/// Filter.
			/// </summary>
			protected FilterView Filter;

			/// <summary>
			/// Current value view.
			/// </summary>
			protected ValueView CurrentValueView;

			/// <summary>
			/// Current field view.
			/// </summary>
			protected FieldViewBase CurrentFieldView;

			#if UNITY_2020_3_OR_NEWER
			/// <summary>
			/// Drag&amp;Drop for variations.
			/// </summary>
			protected DragDrop VariationsDragDrop;

			/// <summary>
			/// Drag&amp;Drop for options.
			/// </summary>
			protected DragDrop OptionsDragDrop;
			#endif

			/// <summary>
			/// Initializes a new instance of the <see cref="EditBlockView"/> class.
			/// </summary>
			public EditBlockView()
				: base("theme-edit")
			{
				Filter = new FilterView(this);

				Header = new Label
				{
					name = "theme-header",
				};

				ModesBlock = new VisualElement
				{
					name = "theme-modes",
				};
				Block.Add(ModesBlock);

				ScrollBlock = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
				{
					name = "theme-scroll",
				};

				VariationsHeader = new VisualElement
				{
					name = "theme-variations",
				};
				ScrollBlock.Add(VariationsHeader);

				VariationsList = new VisualElement
				{
					name = "theme-variations-list",
				};

				Block.Add(ScrollBlock);

				OptionsValuesBlock = new VisualElement
				{
					name = "theme-options",
				};
				ScrollBlock.Add(OptionsValuesBlock);

				#if UNITY_2020_3_OR_NEWER
				var variations_uss = new DragDrop.UssClasses(
					"theme-variation-drag-handle",
					"theme-variation-drag-handle-selected",
					"theme-variation-drop-indicator",
					"theme-variation");
				VariationsDragDrop = new DragDrop(VariationsList, variations_uss, true);
				VariationsDragDrop.OnDrop += MoveVariation;

				var options_uss = new DragDrop.UssClasses(
					"theme-option-drag-handle",
					"theme-option-drag-handle-selected",
					"theme-option-drop-indicator",
					"theme-option");
				OptionsDragDrop = new DragDrop(OptionsValuesBlock, options_uss, false);
				OptionsDragDrop.OnDrop += MoveOption;
				#endif
			}

			/// <summary>
			/// Load views.
			/// </summary>
			protected virtual void LoadViews()
			{
				ValueViews.Clear();

				if (Theme == null)
				{
					return;
				}

				var properties = ThemeInfo.Get(Theme);
				foreach (var p in properties.Properties)
				{
					if (!Theme.IsActiveProperty(p.Name))
					{
						continue;
					}

					var view = new ValueView(p.ValueType, p.Name, p.CreateFieldView(Theme));
					var index = FindValueView(view.Type);
					if (index == -1)
					{
						ValueViews.Add(view);
					}
					else
					{
						ValueViews[index] = view;
					}
				}
			}

			/// <summary>
			/// Refresh.
			/// </summary>
			public void Refresh()
			{
				LoadViews();
				Header.text = Theme != null ? Theme.name : string.Empty;

				ShowModes();
				RefreshVariations();

				if ((CurrentFieldView == null) || !CurrentFieldView.ValueChanged)
				{
					RefreshValues();
				}
			}

			/// <summary>
			/// Find value view.
			/// </summary>
			/// <param name="type">Value type.</param>
			/// <returns>Index of value view.</returns>
			protected virtual int FindValueView(Type type)
			{
				for (int i = 0; i < ValueViews.Count; i++)
				{
					if (ValueViews[i].Type == type)
					{
						return i;
					}
				}

				return -1;
			}

			/// <summary>
			/// Create button.
			/// </summary>
			/// <param name="action">Action on click.</param>
			/// <param name="label">Label.</param>
			/// <param name="className">Class name.</param>
			/// <returns>Button.</returns>
			protected virtual Button Button(Action action, string label, string className)
			{
				var button = new Button(action)
				{
					text = label,
				};
				button.AddToClassList(className);

				return button;
			}

			/// <summary>
			/// Show modes.
			/// </summary>
			protected virtual void ShowModes()
			{
				ModesBlock.Clear();

				foreach (var mode in ValueViews)
				{
					var button = Button(() => ValueType = mode.Type, mode.Label, "theme-mode");
					if (ValueType == mode.Type)
					{
						button.AddToClassList("theme-mode-selected");
					}

					ModesBlock.Add(button);
				}

				ModesBlock.Add(Header);
			}

			/// <summary>
			/// Show variations.
			/// </summary>
			protected virtual void RefreshVariations()
			{
				VariationsHeader.Clear();
				VariationsList.Clear();

				if (Theme == null)
				{
					return;
				}

				VariationsHeader.Add(Filter.Block);
				VariationsHeader.Add(VariationsList);

				var variations = Theme.Variations;
				for (var i = 0; i < variations.Count; i++)
				{
					var variation = variations[i];
					if (!Filter.CanShowVariation(variation))
					{
						continue;
					}

					var block = new VisualElement
					{
						userData = i,
					};
					block.AddToClassList("theme-variation");

					var name = new TextField
					{
						value = variation.Name,
					};
					name.AddToClassList("theme-variation-name");
					name.RegisterValueChangedCallback(ev => RenameVariation(variation, ev.newValue));
					block.Add(name);

					var buttons_block = new VisualElement();
					buttons_block.AddToClassList("theme-variation-buttons");

					if (Theme.ActiveVariationId == variation.Id)
					{
						var toggle = new Label("Active");
						toggle.AddToClassList("theme-variation-active-selected");
						buttons_block.Add(toggle);
					}
					else
					{
						var active_block = new VisualElement();
						active_block.AddToClassList("theme-variation-active-button");
						active_block.Add(Button(() => SetActiveVariation(variation, true), "Set Current", "theme-variation-active"));

						buttons_block.Add(active_block);
					}

					var drag = new Label("↔");
					drag.AddToClassList("theme-variation-drag-handle");
					drag.tooltip = "Drag to reorder";
					buttons_block.Add(drag);

					var btn_delete = Button(() => DeleteVariation(variation), "x", "theme-variation-delete");
					btn_delete.tooltip = "Delete Variation";
					btn_delete.SetEnabled(Theme.Variations.Count > 1);
					buttons_block.Add(btn_delete);

					block.Add(buttons_block);

					if (Theme.InitialVariationId == variation.Id)
					{
						var toggle = new Label("Initial");
						toggle.AddToClassList("theme-variation-initial-selected");
						block.Add(toggle);
					}
					else
					{
						var initial_block = new VisualElement();
						initial_block.AddToClassList("theme-variation-initial-button");
						var btn = Button(() => SetInitialVariation(variation, true), "Set Initial", "theme-variation-initial");
						btn.tooltip = "This variation will be used to find and add options by components values when the theme is attached.";
						initial_block.Add(btn);

						block.Add(initial_block);
					}

					block.Add(Button(() => Clone(variation), "Clone", "theme-variation-clone"));

					VariationsList.Add(block);
				}

				var block_add = new VisualElement();
				block_add.AddToClassList("theme-variation");
				block_add.Add(Button(AddVariation, "Add Variation", "theme-variation-add"));

				VariationsHeader.Add(block_add);
			}

			/// <summary>
			/// Get field view instance.
			/// </summary>
			/// <param name="fieldView">Field view.</param>
			/// <param name="variationId">Variation ID.</param>
			/// <param name="optionId">Option ID.</param>
			/// <returns>Instance.</returns>
			protected virtual VisualElement GetFieldInstance(FieldViewBase fieldView, VariationId variationId, OptionId optionId)
			{
				var key = new ValueKey(variationId, optionId);
				if (FieldViewsInstances.TryGetValue(key, out var view))
				{
					fieldView.UpdateValue(view, variationId, optionId);
				}
				else
				{
					view = fieldView.Create(variationId, optionId);
					FieldViewsInstances[new ValueKey(variationId, optionId)] = view;
				}

				return view;
			}

			/// <summary>
			/// Show options values.
			/// </summary>
			/// <param name="fieldView">Field view.</param>
			/// <param name="propertyName">Property name.</param>
			protected virtual void OptionsValues(FieldViewBase fieldView, string propertyName)
			{
				var values = fieldView.Wrapper;
				var total = values.Options.Count;
				for (var i = 0; i < total; i++)
				{
					var option = values.Options[i];
					if (!Filter.CanShowOption(option))
					{
						continue;
					}

					var row = new VisualElement
					{
						userData = i,
					};
					row.AddToClassList("theme-option");

					var name_block = new VisualElement();
					name_block.AddToClassList("theme-option-main");

					var drag = new Label("↕")
					{
						tooltip = "Drag to reorder",
					};
					drag.AddToClassList("theme-option-drag-handle");
					name_block.Add(drag);

					var del = Button(() => DeleteOption(values, option), "x", "theme-option-delete");
					del.tooltip = "Delete Option";
					name_block.Add(del);

					var find = Button(() => FindTargetsWithOption(propertyName, option), "?", "theme-option-find");
					find.tooltip = "Highlight gameobject with this option";
					name_block.Add(find);

					var name = new TextField
					{
						value = option.Name,
						tooltip = option.Name,
					};
					name.AddToClassList("theme-option-name");
					if (option.Name == "None")
					{
						name.AddToClassList("theme-option-name-invalid");
					}

					name.RegisterValueChangedCallback(ev => RenameOption(option, ev.newValue, name));
					name_block.Add(name);

					row.Add(name_block);

					foreach (var variation in Theme.Variations)
					{
						if (!Filter.CanShowVariation(variation))
						{
							continue;
						}

						var input = new VisualElement();
						input.AddToClassList("theme-option-value");

						input.Add(GetFieldInstance(fieldView, variation.Id, option.Id));
						row.Add(input);
					}

					var filler = new VisualElement();
					filler.AddToClassList("theme-option-value");
					row.Add(filler);

					OptionsValuesBlock.Add(row);
				}

				var row_add = new VisualElement();
				row_add.AddToClassList("theme-option-footer");
				row_add.Add(Button(() => AddOption(values), "Add Option", "theme-option-add"));

				var refs = ThemesReferences.Instance;
				if (refs != null)
				{
					var btn = new Button();
					btn.AddToClassList("theme-set-default");

					if (UnityObjectComparer<Theme>.Instance.Equals(refs.Current, Theme))
					{
						btn.text = "Default Theme";
						btn.SetEnabled(false);
						row_add.Add(btn);
					}
					else
					{
						void Click()
						{
							refs.Current = Theme;
							btn.text = "Default Theme";
							btn.SetEnabled(false);
						}

						btn.text = "Set as Default Theme";
						btn.clickable = new Clickable(Click);

						row_add.Add(btn);
					}
				}

				var btn_attach = new Button();
				btn_attach.AddToClassList("theme-attach-to-scene");
				btn_attach.text = "Attach to the Scene";
				btn_attach.clickable = new Clickable(() => ThemeAttach.ReplaceWith(Theme));
				row_add.Add(btn_attach);

				#if UITHEMES_ADDRESSABLE_SUPPORT
				var addressable = new Toggle("Addressable Support");
				addressable.AddToClassList("theme-addressable-support");
				addressable.value = theme.AddressableSupport;
				addressable.tooltip = "Use when the Theme should be included in the build and assets (sprites, textures, fonts, etc) are addressable and loaded by request.";
				addressable.RegisterValueChangedCallback(ev => SetAddressableSupport(ev.newValue));
				row_add.Add(addressable);
				#endif

				OptionsValuesBlock.Add(row_add);
			}

			/// <summary>
			/// Update values view.
			/// </summary>
			protected virtual void RefreshValues()
			{
				OptionsValuesBlock.Clear();

				if (Theme == null)
				{
					return;
				}

				if (ValueType == null)
				{
					var error = new Label("Value type is not specified.")
					{
						name = "theme-options-error",
					};
					OptionsValuesBlock.Add(error);
					return;
				}

				var index = FindValueView(ValueType);
				if (index == -1)
				{
					var error = new Label("Unsupported type: " + ValueType.FullName)
					{
						name = "theme-options-error",
					};
					OptionsValuesBlock.Add(error);
					return;
				}

				CurrentValueView = ValueViews[index];
				CurrentFieldView = CurrentValueView.FieldViewBase;

				OptionsValues(CurrentFieldView, CurrentValueView.PropertyName);
			}

			/// <summary>
			/// Add variation.
			/// </summary>
			protected virtual void AddVariation()
			{
				Undo.RecordObject(Theme, "UI Themes: Add Variation");

				var variation = Theme.AddVariation("temp");
				variation.Name = "Variation " + variation.Id;

				Save(refreshVariations: true, refreshValues: true);
			}

			/// <summary>
			/// Move variation.
			/// </summary>
			/// <param name="oldIndex">Old index.</param>
			/// <param name="newIndex">New index.</param>
			protected virtual void MoveVariation(int oldIndex, int newIndex)
			{
				if (oldIndex == -1)
				{
					return;
				}

				Undo.RecordObject(Theme, "UI Themes: Move Variation " + Theme.Variations[oldIndex].Name);

				Theme.MoveVariation(oldIndex, newIndex);

				Save(refreshVariations: true, refreshValues: true);

				ThemeTargetInspector.RefreshWindow();
			}

			/// <summary>
			/// Delete variation.
			/// </summary>
			/// <param name="variation">Variation.</param>
			protected virtual void DeleteVariation(Variation variation)
			{
				if (!EditorUtility.DisplayDialog("Delete Variation", "Are you sure you want to delete this variation?", "Delete " + variation.Name, "Cancel"))
				{
					return;
				}

				Undo.RecordObject(Theme, "UI Themes: Delete Variation " + variation.Name);

				Theme.DeleteVariation(variation);

				Save(refreshVariations: true, refreshValues: true);
			}

			/// <summary>
			/// Rename variation.
			/// </summary>
			/// <param name="variation">Variation.</param>
			/// <param name="name">Name.</param>
			protected virtual void RenameVariation(Variation variation, string name)
			{
				Undo.RecordObject(Theme, "UI Themes: Rename Variation " + name);

				variation.Name = name;

				Save();
			}

			/// <summary>
			/// Add option.
			/// </summary>
			/// <param name="values">Values wrapper.</param>
			protected virtual void AddOption(Theme.IValuesWrapper values)
			{
				Undo.RecordObject(Theme, "UI Themes: Add Option");

				var option = values.AddOption("temp");
				option.Name = "Option " + option.Id;

				Save(refreshValues: true);
			}

			/// <summary>
			/// Move option.
			/// </summary>
			/// <param name="oldIndex">Old index.</param>
			/// <param name="newIndex">New index.</param>
			protected virtual void MoveOption(int oldIndex, int newIndex)
			{
				if (oldIndex == -1)
				{
					return;
				}

				var values = CurrentValueView.FieldViewBase.Wrapper;

				Undo.RecordObject(Theme, "UI Themes: Move Option " + values.Options[oldIndex].Name);

				values.MoveOption(oldIndex, newIndex);

				Save(refreshValues: true);

				ThemeTargetInspector.RefreshWindow();
			}

			/// <summary>
			/// Delete option.
			/// </summary>
			/// <param name="values">Values wrapper.</param>
			/// <param name="option">Option.</param>
			protected virtual void DeleteOption(Theme.IValuesWrapper values, Option option)
			{
				if (!EditorUtility.DisplayDialog("Delete Option", "Are you sure you want to delete this option?", "Delete " + option.Name, "Cancel"))
				{
					return;
				}

				Undo.RecordObject(Theme, "UI Themes: Delete Option " + option.Name);

				values.DeleteOption(option.Id);

				Save(refreshValues: true);

				ThemeTargetInspector.RefreshWindow();
			}

			/// <summary>
			/// Show a window with a list of targets that use a specified option.
			/// </summary>
			/// <param name="propertyName">Property name.</param>
			/// <param name="option">Option.</param>
			protected virtual void FindTargetsWithOption(string propertyName, Option option)
			{
				ThemeTargetsWindow.Open(Theme, propertyName, option);
			}

			/// <summary>
			/// Rename option.
			/// </summary>
			/// <param name="option">Option.</param>
			/// <param name="name">Name.</param>
			/// <param name="input">Input.</param>
			protected virtual void RenameOption(Option option, string name, TextField input)
			{
				if (name == "None")
				{
					input.AddToClassList("theme-option-name-invalid");
				}
				else
				{
					input.RemoveFromClassList("theme-option-name-invalid");
				}

				input.tooltip = name;

				Undo.RecordObject(Theme, "UI Themes: Rename Option " + name);

				option.Name = name;

				Save(refreshTargets: true);
			}

			/// <summary>
			/// Set active variation.
			/// </summary>
			/// <param name="variation">Variation.</param>
			/// <param name="active">Active.</param>
			protected virtual void SetActiveVariation(Variation variation, bool active)
			{
				if (!active)
				{
					return;
				}

				Undo.RecordObject(Theme, "UI Themes: Set Active Variation " + variation.Name);

				Theme.ActiveVariationId = variation.Id;

				Save(refreshVariations: true);
			}

			/// <summary>
			/// Clone variation.
			/// </summary>
			/// <param name="variation">Variation.</param>
			protected virtual void Clone(Variation variation)
			{
				Undo.RecordObject(Theme, "UI Themes: Clone Variation " + variation.Name);

				Theme.Clone(variation);

				Save(refreshVariations: true, refreshValues: true);
			}

			/// <summary>
			/// Set initial variation.
			/// </summary>
			/// <param name="variation">Variation.</param>
			/// <param name="active">Active.</param>
			protected virtual void SetInitialVariation(Variation variation, bool active)
			{
				if (!active)
				{
					return;
				}

				Undo.RecordObject(Theme, "UI Themes: Set Initial Variation " + variation.Name);

				Theme.InitialVariationId = variation.Id;

				Save(refreshVariations: true);
			}

			/// <summary>
			/// Process theme save.
			/// </summary>
			/// <param name="refreshVariations">Refresh variations view.</param>
			/// <param name="refreshValues">Refresh values view.</param>
			/// <param name="refreshTargets">Refresh targets view.</param>
			protected virtual void Save(bool refreshVariations = false, bool refreshValues = false, bool refreshTargets = false)
			{
				if (refreshVariations)
				{
					Theme.ValidateVariations(Theme);
					RefreshVariations();
				}

				if (refreshValues)
				{
					RefreshValues();
				}

				if (refreshTargets)
				{
					ThemeTargetInspector.RefreshWindow();
				}

				// needed since Undo.RecordObject not enough to ensure save
				EditorUtility.SetDirty(Theme);
			}

			#if UITHEMES_ADDRESSABLE_SUPPORT

			/// <summary>
			/// Set AddressableSupport.
			/// </summary>
			/// <param name="state">State.</param>
			protected virtual void SetAddressableSupport(bool state)
			{
				Undo.RecordObject(Theme, "UI Themes: Toggle Addressable Support");

				Theme.AddressableSupport = state;

				Save();
			}

			#endif
		}
	}
}
#endif