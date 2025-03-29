#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Customized ListView's editor.
	/// </summary>
	[CustomEditor(typeof(ListViewBase), true)]
	public class ListViewCustomBaseEditor : UIWidgetsMonoEditor
	{
		/// <summary>
		/// Is it ListViewCustom?
		/// </summary>
		protected bool IsListViewCustom = false;

		/// <summary>
		/// Data type.
		/// </summary>
		protected Type DataType;

		/// <summary>
		/// Is it TreeViewCustom?
		/// </summary>
		protected bool IsTreeViewCustom = false;

		/// <summary>
		/// Serialized properties.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Serialized events.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedEvents = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Properties.
		/// </summary>
		protected List<string> Properties = new List<string>()
		{
			"interactable",
			"disableScrollRect",
			"virtualization",
			"listType",
			"ChangeLayoutType",
			"StretchToMaxItemSize",
			"PrecalculateItemSizes",
			"ContainerMaxSize",

			"customItems",
			"reversedOrder",
			"selectedIndex",
			"multipleSelect",
			"MultipleSelectRequireKeys",
			"deselectCollapsedNodes",
			"RangeMode",
			"direction",

			"scrollRect",
			"Container",
			"setContentSizeFitter",
			"defaultItem",
			"destroyDefaultItemsCache",

			"allowColoring",
			"defaultColor",
			"defaultBackgroundColor",
			"highlightedColor",
			"highlightedBackgroundColor",
			"selectedColor",
			"selectedBackgroundColor",
			"disabledColor",

			"coloringStriped",
			"defaultBackgroundColorEven",
			"defaultBackgroundColorOdd",

			"FadeDuration",
			"KeepHighlight",
			"OnlyOneHighlighted",

			// other
			"Navigation",
			"LoopedNavigation",
			"RequireVisible",
			"ToggleOnNavigate",
			"ToggleOnSubmitCancel",

			"ScrollUnscaledTime",
			"ScrollMovement",
			"AutoScrollArea",
			"AutoScrollSpeed",
			"EndScrollDelay",
			"alignCenter",
			"scrollInertiaUntilItemCenter",
			"ScrollInertia",
			"loopedList",
			"ScrollWithIndent",
		};

		/// <summary>
		/// Properties indents.
		/// </summary>
		[DomainReloadExclude]
		protected static IReadOnlyDictionary<string, int> Indents = new Dictionary<string, int>()
		{
			{ "ChangeLayoutType", 1 },
			{ "StretchToMaxItemSize", 1 },
			{ "ContainerMaxSize", 1 },
			{ "destroyDefaultItemsCache", 1 },
			{ "RangeMode", 1 },
			{ "disableScrollRect", 1 },
			{ "PrecalculateItemSizes", 1 },
			{ "setContentSizeFitter", 1 },
			{ "MultipleSelectRequireKeys", 1 },
			{ "LoopedNavigation", 1 },
			{ "RequireVisible", 1 },

			// TreeView
			{ "deselectCollapsedNodes", 1 },
			{ "ToggleOnNavigate", 1 },
			{ "ToggleOnSubmitCancel", 1 },

			/*
			{ "defaultColor", 1 },
			{ "defaultBackgroundColor", 1 },
			{ "highlightedColor", 1 },
			{ "highlightedBackgroundColor", 1 },
			{ "selectedColor", 1 },
			{ "selectedBackgroundColor", 1 },
			{ "disabledColor", 1 },
			{ "FadeDuration", 1 },
			{ "KeepHighlight", 1 },
			{ "OnlyOneHighlighted", 1 },
			*/
		};

		/// <summary>
		/// Scroll properties.
		/// </summary>
		[DomainReloadExclude]
		protected static IReadOnlyList<string> ScrollFields = new List<string>()
		{
			"RetainScrollPosition",
			"alignCenter",
			"loopedList",
			"ScrollUnscaledTime",
			"ScrollMovement",
			"ScrollWithIndent",
			"scrollInertiaUntilItemCenter",
			"ScrollInertia",
			"EndScrollDelay",
			"AutoScrollArea",
			"AutoScrollSpeed",
		};

		/// <summary>
		/// Always allow to edit field.
		/// </summary>
		static Func<ListViewCustomBaseEditor, bool> AllowAlways => editor => true;

		/// <summary>
		/// Field condition.
		/// </summary>
		protected class FieldCondition : Tuple<string, Func<ListViewCustomBaseEditor, bool>>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="FieldCondition"/> class.
			/// </summary>
			/// <param name="field">Field.</param>
			/// <param name="condition">Condition.</param>
			public FieldCondition(string field, Func<ListViewCustomBaseEditor, bool> condition)
				: base(field, condition)
			{
			}
		}

		/// <summary>
		/// Coloring fields.
		/// </summary>
		[DomainReloadExclude]
		protected static IReadOnlyList<FieldCondition> ColoringFields = new List<FieldCondition>()
		{
			new FieldCondition("coloringStriped", AllowAlways),
			new FieldCondition("defaultColor", AllowAlways),
			new FieldCondition("defaultBackgroundColor", editor => !editor.SerializedProperties["coloringStriped"].boolValue),
			new FieldCondition("defaultEvenBackgroundColor", editor => editor.SerializedProperties["coloringStriped"].boolValue),
			new FieldCondition("defaultOddBackgroundColor", editor => editor.SerializedProperties["coloringStriped"].boolValue),
			new FieldCondition("highlightedColor", AllowAlways),
			new FieldCondition("highlightedBackgroundColor", AllowAlways),
			new FieldCondition("selectedColor", AllowAlways),
			new FieldCondition("selectedBackgroundColor", AllowAlways),
			new FieldCondition("disabledColor", AllowAlways),

			new FieldCondition("FadeDuration", AllowAlways),
			new FieldCondition("KeepHighlight", AllowAlways),
			new FieldCondition("OnlyOneHighlighted", AllowAlways),
		};

		/// <summary>
		/// Events.
		/// </summary>
		protected List<string> Events = new List<string>()
		{
			"OnSelect",
			"OnDeselect",
			"OnSelectObject",
			"OnDeselectObject",
			"OnStartScrolling",
			"OnEndScrolling",
		};

		/// <summary>
		/// Exclude properties and events.
		/// </summary>
		[DomainReloadExclude]
		protected static IReadOnlyList<string> Exclude = new List<string>()
		{
			"selectedIndices",
			"sort",
			"KeepSelection",

			// obsolete
			"LimitScrollValue",
			"FixHighlightItemUnderPointer",
		};

		/// <summary>
		/// Hidden properties and events.
		/// </summary>
		[DomainReloadExclude]
		protected static IReadOnlyList<string> Hidden = new List<string>()
		{
			// ListViewBase
			"instances",
			"DestroyGameObjects",
			"disabledContainer",

			// ListViewCustom
			"Instances",
			"OwnTemplates",
			"SharedTemplates",
			"InstancesDisplayedIndices",
			"LayoutElements",
			"listRenderer",
		};

		static bool DetectGenericType(Type type, Type genericType)
		{
			while (type != null)
			{
				if (type.IsConstructedGenericType && (type.GetGenericTypeDefinition() == genericType))
				{
					return true;
				}

				type = type.BaseType;
			}

			return false;
		}

		[DomainReloadExclude]
		static readonly Dictionary<Type, Type> ListView2DataType = new Dictionary<Type, Type>();

		static Type GetDataType(Type baseType)
		{
			var type = baseType;
			if (ListView2DataType.TryGetValue(type, out var data_type))
			{
				return data_type;
			}

			while (type != null)
			{
				if (type.IsConstructedGenericType && (type.GetGenericTypeDefinition() == typeof(ListViewCustom<,>)))
				{
					data_type = type.GetGenericArguments()[1];
					ListView2DataType[baseType] = data_type;
					return data_type;
				}

				type = type.BaseType;
			}

			ListView2DataType[baseType] = null;
			return null;
		}

		/// <summary>
		/// Fill properties list.
		/// </summary>
		protected virtual void FillProperties()
		{
			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}
		}

		/// <summary>
		/// Add property.
		/// </summary>
		/// <param name="property">Property.</param>
		protected void AddProperty(SerializedProperty property)
		{
			if (Exclude.Contains(property.name))
			{
				return;
			}

			if (Events.Contains(property.name) || Properties.Contains(property.name))
			{
				return;
			}

			if (IsEvent(property))
			{
				Events.Add(property.name);
			}
			else
			{
				Properties.Add(property.name);
			}
		}

		/// <summary>
		/// Is it event?
		/// </summary>
		/// <param name="property">Property</param>
		/// <returns>true if property is event; otherwise false.</returns>
		protected virtual bool IsEvent(SerializedProperty property)
		{
			var object_type = property.serializedObject.targetObject.GetType();
			var property_type = object_type.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (property_type == null)
			{
				return false;
			}

			return typeof(UnityEventBase).IsAssignableFrom(property_type.FieldType);
		}

		readonly GUILayoutOption[] toggleOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) };

		/// <summary>
		/// Target.
		/// </summary>
		protected EditorConditional.TypeData DataSource;

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			FillProperties();

			var type = serializedObject.targetObject.GetType();
			if (!IsListViewCustom)
			{
				IsListViewCustom = DetectGenericType(type, typeof(ListViewCustom<,>));
				if (IsListViewCustom)
				{
					DataType = GetDataType(type);
				}
			}

			if (!IsTreeViewCustom)
			{
				IsTreeViewCustom = DetectGenericType(type, typeof(TreeViewCustom<,>));
			}

			if (IsTreeViewCustom)
			{
				Properties.Remove("customItems");
				Properties.Remove("selectedIndex");
				Properties.Remove("loopedList");
			}

			if (IsListViewCustom)
			{
				foreach (var p in Properties)
				{
					var property = serializedObject.FindProperty(p);
					if ((property != null) || (p == "customItems"))
					{
						SerializedProperties[p] = property;
					}
				}

				foreach (var ev in Events)
				{
					var property = serializedObject.FindProperty(ev);
					if (property != null)
					{
						SerializedEvents[ev] = property;
					}
				}
			}
		}

		/// <summary>
		/// Toggle events block.
		/// </summary>
		protected bool ShowEvents;

		/// <summary>
		/// Toggle scroll properties block.
		/// </summary>
		protected bool ShowScrollProperties;

		void Upgrade()
		{
			foreach (var t in targets)
			{
				var lv = t as ListViewBase;
				if (lv != null)
				{
					lv.Upgrade();
				}
			}
		}

		/// <summary>
		/// Is property should be displayed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>true if property should be displayed; otherwise false.</returns>
		protected bool IsShow(string propertyName)
		{
			if (Hidden.Contains(propertyName))
			{
				return false;
			}

			if (propertyName == "StretchToMaxItemSize")
			{
				return ListType == ListViewType.ListViewWithVariableSize;
			}

			if (propertyName == "PrecalculateItemSizes")
			{
				return IsVariableListType;
			}

			if (propertyName == "ContainerMaxSize")
			{
				return IsVariableListType;
			}

			if (propertyName == "InstancesEvents")
			{
				return false;
			}

			if ((propertyName == "MultipleSelectRequireKeys") || (propertyName == "RangeMode"))
			{
				return SerializedProperties["multipleSelect"].boolValue;
			}

			if ((propertyName == "LoopedNavigation") || (propertyName == "RequireVisible"))
			{
				return SerializedProperties["Navigation"].boolValue;
			}

			// TreeView
			if ((propertyName == "ToggleOnNavigate") || (propertyName == "ToggleOnSubmitCancel"))
			{
				return SerializedProperties["Navigation"].boolValue;
			}

			if (propertyName == "deselectCollapsedNodes")
			{
				return SerializedProperties["multipleSelect"].boolValue;
			}

			if (ScrollFields.Contains(propertyName))
			{
				return false;
			}

			foreach (var (field, _) in ColoringFields)
			{
				if (propertyName == field)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Is property should be displayed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>true if property should be displayed; otherwise false.</returns>
		protected bool IsShowScrollField(string propertyName)
		{
			if (propertyName == "loopedList")
			{
				return !IsTileView && !IsTreeViewCustom;
			}

			return true;
		}

		/// <summary>
		/// List type.
		/// </summary>
		protected ListViewType ListType => (ListViewType)SerializedProperties["listType"].enumValueIndex;

		/// <summary>
		/// Is TileView?
		/// </summary>
		protected bool IsTileView => (ListType == ListViewType.TileViewWithFixedSize) || (ListType == ListViewType.TileViewWithVariableSize);

		/// <summary>
		/// Is variable list type?
		/// </summary>
		protected bool IsVariableListType => (ListType == ListViewType.ListViewWithVariableSize) || (ListType == ListViewType.TileViewWithVariableSize);

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			Upgrade();

			if (IsListViewCustom)
			{
				serializedObject.Update();

				foreach (var property_name in Properties)
				{
					if (!IsShow(property_name))
					{
						continue;
					}

					if (!SerializedProperties.TryGetValue(property_name, out var property))
					{
						continue;
					}

					if (!Indents.TryGetValue(property_name, out var indent))
					{
						indent = 0;
					}

					EditorGUI.indentLevel += indent;

					if (property_name == "customItems")
					{
						if (property != null)
						{
							EditorGUILayout.PropertyField(property, new GUIContent("Data Source"), true);
						}
						else if (DataType != null)
						{
							if (DataType.IsInterface)
							{
								EditorGUILayout.HelpBox("DataSource cannot be displayed because the item type is an interface.", MessageType.Info);
							}
							else
							{
								EditorGUILayout.HelpBox("DataSource cannot be displayed because the item type is not serializable.\nAdd [Serializable] attribute if the type is not interface.", MessageType.Info);
							}
						}
					}
					else if (property_name == "allowColoring")
					{
						EditorGUILayout.PropertyField(property, true);

						if (property.boolValue)
						{
							EditorGUI.indentLevel += 1;

							foreach (var (field, condition) in ColoringFields)
							{
								if (condition(this))
								{
									if (!SerializedProperties.ContainsKey(field))
									{
										continue;
									}

									EditorGUILayout.PropertyField(SerializedProperties[field], true);
								}
							}

							EditorGUI.indentLevel -= 1;
						}
					}
					else
					{
						EditorGUILayout.PropertyField(property, true);
					}

					EditorGUI.indentLevel -= indent;
				}

				ShowScrollProperties = GUILayout.Toggle(ShowScrollProperties, "Scroll Settings", EditorStyles.foldout, toggleOptions);

				if (ShowScrollProperties)
				{
					EditorGUI.indentLevel += 1;

					foreach (var field in ScrollFields)
					{
						if (IsShowScrollField(field) && SerializedProperties.TryGetValue(field, out var property))
						{
							EditorGUILayout.PropertyField(property, true);
						}
					}

					EditorGUI.indentLevel -= 1;
				}

				ShowEvents = GUILayout.Toggle(ShowEvents, "Events", EditorStyles.foldout, toggleOptions);

				if (ShowEvents)
				{
					foreach (var se in SerializedEvents)
					{
						EditorGUILayout.PropertyField(se.Value, true);
					}
				}

				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(SerializedProperties["InstancesEvents"], new GUIContent("Instances Events"), true);
				EditorGUI.indentLevel -= 1;

				if (serializedObject.hasModifiedProperties)
				{
					UtilitiesEditor.ApplyModifiedProperties(serializedObject);
				}

				ShowWarnings();

				if (Application.isPlaying && GUILayout.Button("Copy Debug Information"))
				{
					GUIUtility.systemCopyBuffer = ((ListViewBase)target).GetDebugInfo();
				}
			}
			else
			{
				DrawDefaultInspector();
			}

			ValidateTargets();
		}

		/// <summary>
		/// Show warnings.
		/// </summary>
		protected void ShowWarnings()
		{
			var show_warning = false;

			foreach (var t in targets)
			{
				var type = t.GetType();
				var method = type.GetMethod("IsVirtualizationPossible", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (method != null)
				{
					var virtualization = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), t, method);
					show_warning |= !virtualization.Invoke();
				}
			}

			if (!show_warning)
			{
				return;
			}

			if (IsTileView || IsTreeViewCustom)
			{
				EditorGUILayout.HelpBox("Virtualization requires specified ScrollRect and Container should have EasyLayout component.", MessageType.Warning);
			}
			else
			{
				EditorGUILayout.HelpBox("Virtualization requires specified ScrollRect and Container should have EasyLayout or Horizontal or Vertical Layout Group component.", MessageType.Warning);
			}
		}
	}
}
#endif