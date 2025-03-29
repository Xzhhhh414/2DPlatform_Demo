#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
		static void Create(GameObject prefab) => Widgets.CreateFromPrefab(prefab);

		static PrefabsMenu Prefabs
		{
			get
			{
				var p = WidgetsReferences.Instance.Current;
				if (p != null)
				{
					return p;
				}

				return PrefabsMenu.Instance;
			}
		}

		#region Collections

		/// <summary>
		/// Create AutocompleteCombobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutocompleteCombobox", false, 1000)]
		public static void CreateAutocompleteCombobox()
		{
			Create(Prefabs.AutocompleteCombobox);
		}

		/// <summary>
		/// Create AutoComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutoComboboxIcons", false, 1002)]
		public static void CreateAutoComboboxIcons()
		{
			Create(Prefabs.AutoComboboxIcons);
		}

		/// <summary>
		/// Create AutoComboboxString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/AutoComboboxString", false, 1004)]
		public static void CreateAutoComboboxString()
		{
			Create(Prefabs.AutoComboboxString);
		}

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/Combobox", false, 1005)]
		public static void CreateCombobox()
		{
			Create(Prefabs.Combobox);
		}

		/// <summary>
		/// Create ComboboxEnum.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxEnum", false, 1007)]
		public static void CreateComboboxEnum()
		{
			Create(Prefabs.ComboboxEnum);
		}

		/// <summary>
		/// Create ComboboxEnumMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxEnumMultiselect", false, 1008)]
		public static void CreateComboboxEnumMultiselect()
		{
			Create(Prefabs.ComboboxEnumMultiselect);
		}

		/// <summary>
		/// Create ComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Create(Prefabs.ComboboxIcons);
		}

		/// <summary>
		/// Create ComboboxIconsMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Create(Prefabs.ComboboxIconsMultiselect);
		}

		/// <summary>
		/// Create ComboboxInputField.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ComboboxInputField", false, 1005)]
		public static void CreateComboboxInputField()
		{
			Create(Prefabs.ComboboxInputField);
		}

		/// <summary>
		/// Create DirectoryTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Create(Prefabs.DirectoryTreeView);
		}

		/// <summary>
		/// Create FileListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Create(Prefabs.FileListView);
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Create(Prefabs.ListView);
		}

		/// <summary>
		/// Create istViewColors.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewColors", false, 1055)]
		public static void CreateListViewColors()
		{
			Create(Prefabs.ListViewColors);
		}

		/// <summary>
		/// Create ListViewEnum.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewEnum", false, 1057)]
		public static void CreateListViewEnum()
		{
			Create(Prefabs.ListViewEnum);
		}

		/// <summary>
		/// Create ListViewInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Create(Prefabs.ListViewInt);
		}

		/// <summary>
		/// Create ListViewHeight.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Create(Prefabs.ListViewHeight);
		}

		/// <summary>
		/// Create ListViewIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Create(Prefabs.ListViewIcons);
		}

		/// <summary>
		/// Create ListViewPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Create(Prefabs.ListViewPaginator);
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Create(Prefabs.TreeView);
		}
		#endregion

		#region Containers

		/// <summary>
		/// Create Accordion.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Create(Prefabs.Accordion);
		}

		/// <summary>
		/// Create SliderHorizontal.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/SliderHorizontal", false, 2010)]
		public static void CreateSliderHorizontal()
		{
			Create(Prefabs.SliderHorizontal);
		}

		/// <summary>
		/// Create SliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/SliderVertical", false, 2020)]
		public static void CreateSliderVertical()
		{
			Create(Prefabs.SliderVertical);
		}

		/// <summary>
		/// Create Tabs.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/Tabs", false, 2030)]
		public static void CreateTabs()
		{
			Create(Prefabs.Tabs);
		}

		/// <summary>
		/// Create TabsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsLeft", false, 2040)]
		public static void CreateTabsLeft()
		{
			Create(Prefabs.TabsLeft);
		}

		/// <summary>
		/// Create TabsIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIcons", false, 2050)]
		public static void CreateTabsIcons()
		{
			Create(Prefabs.TabsIcons);
		}

		/// <summary>
		/// Create TabsIconsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsIconsLeft", false, 2060)]
		public static void CreateTabsIconsLeft()
		{
			Create(Prefabs.TabsIconsLeft);
		}

		/// <summary>
		/// Create TabsSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Containers/TabsSlider", false, 2070)]
		public static void CreateTabsSlider()
		{
			Create(Prefabs.TabsSlider);
		}
		#endregion

		#region Controls

		/// <summary>
		/// Create ButtonBig.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ButtonBig", false, 2500)]
		public static void CreateButtonBig()
		{
			Create(Prefabs.ButtonBig);
		}

		/// <summary>
		/// Create ButtonSmall.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ButtonSmall", false, 2510)]
		public static void CreateButtonSmall()
		{
			Create(Prefabs.ButtonSmall);
		}

		/// <summary>
		/// Create ContextMenu template.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ContextMenu Template", false, 2520)]
		public static void CreateContextMenu()
		{
			Create(Prefabs.ContextMenuTemplate);
		}

		/// <summary>
		/// Create ScrollRectPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ScrollRectPaginator", false, 2530)]
		public static void CreateScrollRectPaginator()
		{
			Create(Prefabs.ScrollRectPaginator);
		}

		/// <summary>
		/// Create ScrollRectNumericPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/ScrollRectNumericPaginator", false, 2540)]
		public static void CreateScrollRectNumericPaginator()
		{
			Create(Prefabs.ScrollRectNumericPaginator);
		}

		/// <summary>
		/// Create Sidebar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/Sidebar", false, 2550)]
		public static void CreateSidebar()
		{
			Create(Prefabs.Sidebar);
		}

		/// <summary>
		/// Create SplitButton.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/SplitButton", false, 2560)]
		public static void CreateSplitButton()
		{
			Create(Prefabs.SplitButton);
		}

		#if UIWIDGETS_TMPRO_SUPPORT
		/// <summary>
		/// Create TextMeshProPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Controls/TextMeshProPaginator", false, 2570)]
		public static void CreateTextMeshProPaginator()
		{
			Create(Prefabs.TextMeshProPaginator);
		}
		#endif

		#endregion

		#region Dialogs

		/// <summary>
		/// Create ColorPickerDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/ColorPickerDialog", false, 3000)]
		public static void CreateColorPickerDialog()
		{
			Create(Prefabs.ColorPickerDialog);
		}

		/// <summary>
		/// Create DatePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/DatePicker", false, 3004)]
		public static void CreateDatePicker()
		{
			Create(Prefabs.DatePicker);
		}

		/// <summary>
		/// Create DateTimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/DateTimePicker", false, 3007)]
		public static void CreateDateTimePicker()
		{
			Create(Prefabs.DateTimePicker);
		}

		/// <summary>
		/// Create Dialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/Dialog", false, 3010)]
		public static void CreateDialog()
		{
			Create(Prefabs.DialogTemplate);
		}

		/// <summary>
		/// Create FileDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Create(Prefabs.FileDialog);
		}

		/// <summary>
		/// Create FolderDialog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Create(Prefabs.FolderDialog);
		}

		/// <summary>
		/// Create Notify.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/Notification", false, 3040)]
		public static void CreateNotify()
		{
			Create(Prefabs.NotifyTemplate);
		}

		/// <summary>
		/// Create PickerBool.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Create(Prefabs.PickerBool);
		}

		/// <summary>
		/// Create PickerIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/PickerListViewIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Create(Prefabs.PickerIcons);
		}

		/// <summary>
		/// Create PickerInt.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Create(Prefabs.PickerInt);
		}

		/// <summary>
		/// Create PickerString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Create(Prefabs.PickerString);
		}

		/// <summary>
		/// Create Popup.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Create(Prefabs.Popup);
		}

		/// <summary>
		/// Create TimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Dialog Templates/TimePicker", false, 3100)]
		public static void CreateTimePicker()
		{
			Create(Prefabs.TimePicker);
		}
		#endregion

		#region Input

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Create(Prefabs.Autocomplete);
		}

		/// <summary>
		/// Create AutocompleteIcons.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Create(Prefabs.AutocompleteIcons);
		}

		/// <summary>
		/// Create Calendar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Create(Prefabs.Calendar);
		}

		/// <summary>
		/// Create CenteredSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Create(Prefabs.CenteredSlider);
		}

		/// <summary>
		/// Create CenteredSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Create(Prefabs.CenteredSliderVertical);
		}

		/// <summary>
		/// Create CircularSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CircularSlider", false, 4043)]
		public static void CircularSlider()
		{
			Create(Prefabs.CircularSlider);
		}

		/// <summary>
		/// Create CircularSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/CircularSliderFloat", false, 4046)]
		public static void CircularSliderFloat()
		{
			Create(Prefabs.CircularSliderFloat);
		}

		/// <summary>
		/// Create ColorPicker.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Create(Prefabs.ColorPicker);
		}

		/// <summary>
		/// Create ColorPickerRange.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Create(Prefabs.ColorPickerRange);
		}

		/// <summary>
		/// Create ColorPickerRangeHSV.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorPickerRangeHSV", false, 4063)]
		public static void CreateColorPickerRangeHSV()
		{
			Create(Prefabs.ColorPickerRangeHSV);
		}

		/// <summary>
		/// Create ColorsList.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ColorsList", false, 4065)]
		public static void CreateColorsList()
		{
			Create(Prefabs.ColorsList);
		}

		/// <summary>
		/// Create DateTime.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTime", false, 4067)]
		public static void CreateDateTime()
		{
			Create(Prefabs.DateTime);
		}

		/// <summary>
		/// Create DateScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateScroller", false, 4068)]
		public static void CreateDateScroller()
		{
			Create(Prefabs.DateScroller);
		}

		/// <summary>
		/// Create DateTimeScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTimeScroller", false, 4069)]
		public static void CreateDateTimeScroller()
		{
			Create(Prefabs.DateTimeScroller);
		}

		/// <summary>
		/// Create DateTimeScrollerSeparate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/DateTimeScrollerSeparate", false, 4070)]
		public static void CreateDateTimeScrollerSeparate()
		{
			Create(Prefabs.DateTimeScrollerSeparate);
		}

		/// <summary>
		/// Create RangeSlider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSlider", false, 4071)]
		public static void CreateRangeSlider()
		{
			Create(Prefabs.RangeSlider);
		}

		/// <summary>
		/// Create RangeSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Create(Prefabs.RangeSliderFloat);
		}

		/// <summary>
		/// Create RangeSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderVertical", false, 4090)]
		public static void CreateRangeSliderVertical()
		{
			Create(Prefabs.RangeSliderVertical);
		}

		/// <summary>
		/// Create RangeSliderFloatVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Create(Prefabs.RangeSliderFloatVertical);
		}

		/// <summary>
		/// Create Rating.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Rating", false, 4102)]
		public static void CreateRating()
		{
			Create(Prefabs.Rating);
		}

		/// <summary>
		/// Create ScaleHorizontal.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ScaleHorizontal", false, 4104)]
		public static void CreateScaleHorizontal()
		{
			Create(Prefabs.ScaleHorizontal);
		}

		/// <summary>
		/// Create ScaleVertical.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/ScaleVertical", false, 4107)]
		public static void CreateScaleVertical()
		{
			Create(Prefabs.ScaleVertical);
		}

		/// <summary>
		/// Create Spinner.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Create(Prefabs.Spinner);
		}

		/// <summary>
		/// Create SpinnerFloat.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Create(Prefabs.SpinnerFloat);
		}

		/// <summary>
		/// Create SpinnerVector3.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/SpinnerVector3", false, 4125)]
		public static void CreateSpinnerVector3()
		{
			Create(Prefabs.SpinnerVector3);
		}

		/// <summary>
		/// Create Switch.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Create(Prefabs.Switch);
		}

		/// <summary>
		/// Create Time12.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time12", false, 4140)]
		public static void CreateTime12()
		{
			Create(Prefabs.Time12);
		}

		/// <summary>
		/// Create Time24.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/Time24", false, 4150)]
		public static void CreateTime24()
		{
			Create(Prefabs.Time24);
		}

		/// <summary>
		/// Create TimeAnalog.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/TimeAnalog", false, 4155)]
		public static void CreateTimeAnalog()
		{
			Create(Prefabs.TimeAnalog);
		}

		/// <summary>
		/// Create TimeScroller.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Input/TimeScroller", false, 4160)]
		public static void CreateTimeScroller()
		{
			Create(Prefabs.TimeScroller);
		}

		#endregion

		#region DefaultWidgets
		#if !UIWIDGETS_LEGACY_STYLE
		/// <summary>
		/// Create Button.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Button", false, 4500)]
		public static void CreateDefaultButton()
		{
			Create(Prefabs.Button);
		}

		/// <summary>
		/// Create InputField.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/InputField", false, 4500)]
		public static void CreateDefaultInputField()
		{
			Create(Prefabs.InputField);
		}

		/// <summary>
		/// Create Panel.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Panel", false, 4500)]
		public static void CreateDefaultPanel()
		{
			Create(Prefabs.Panel);
		}

		/// <summary>
		/// Create Scroll View.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Scroll View", false, 4500)]
		public static void CreateDefaultScrollView()
		{
			Create(Prefabs.ScrollView);
		}

		/// <summary>
		/// Create Scrollbar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Scrollbar", false, 4500)]
		public static void CreateDefaultScrollbar()
		{
			Create(Prefabs.Scrollbar);
		}

		/// <summary>
		/// Create Slider.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Slider", false, 4500)]
		public static void CreateDefaultSlider()
		{
			Create(Prefabs.Slider);
		}

		/// <summary>
		/// Create Slider with Scale.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Slider with Scale", false, 4500)]
		public static void CreateDefaultSliderWithScale()
		{
			Create(Prefabs.SliderWithScale);
		}

		/// <summary>
		/// Create Text.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Text", false, 4500)]
		public static void CreateDefaultText()
		{
			Create(Prefabs.Text);
		}

		/// <summary>
		/// Create Toggle.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Default/Toggle", false, 4500)]
		public static void CreateDefaultToggle()
		{
			Create(Prefabs.Toggle);
		}
		#endif
		#endregion

		#region Miscellaneous

		/// <summary>
		/// Create AudioPlayer.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Create(Prefabs.AudioPlayer);
		}

		/// <summary>
		/// Create LoadingAnimation.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/Loading Animation", false, 5005)]
		public static void CreateLoadingAnimation()
		{
			Create(Prefabs.LoadingAnimation);
		}

		/// <summary>
		/// Create Progressbar.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/(obsolete) Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Create(Prefabs.Progressbar);
		}

		/// <summary>
		/// Create ProgressbarDeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/ProgressbarDeterminate", false, 5014)]
		public static void CreateProgressbarDeterminate()
		{
			Create(Prefabs.ProgressbarDeterminate);
		}

		/// <summary>
		/// Create ProgressbarCircular.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/ProgressbarCircular", false, 5015)]
		public static void CreateProgressbarCircular()
		{
			Create(Prefabs.ProgressbarCircular);
		}

		/// <summary>
		/// Create ProgressbarIndeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/ProgressbarIndeterminate", false, 5017)]
		public static void CreateProgressbarIndeterminate()
		{
			Create(Prefabs.ProgressbarIndeterminate);
		}

		/// <summary>
		/// Create Tooltip.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/Simple Tooltip", false, 5020)]
		public static void CreateSimpleTooltip()
		{
			Create(Prefabs.Tooltip);
		}

		/// <summary>
		/// Create TooltipString.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Miscellaneous/TooltipString", false, 5030)]
		public static void CreateTooltipString()
		{
			Create(Prefabs.TooltipString);
		}
		#endregion
	}
}
#endif