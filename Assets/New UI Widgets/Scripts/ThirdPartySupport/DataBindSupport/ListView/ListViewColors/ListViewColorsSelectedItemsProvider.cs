#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListViewColors.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewColors SelectedItems Provider")]
	public class ListViewColorsSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListViewColors, System.Collections.Generic.List<UnityEngine.Color>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewColors target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewColors);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewColors);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UnityEngine.Color> GetValue(UIWidgets.ListViewColors target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewColors target)
		{

			target.OnSelectObject.RemoveListener(OnSelectObjectListViewColors);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewColors);
		}


		void OnSelectObjectListViewColors(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewColors(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif