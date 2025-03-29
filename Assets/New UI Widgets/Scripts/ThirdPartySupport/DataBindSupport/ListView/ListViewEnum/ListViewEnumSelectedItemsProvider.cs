#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListViewEnum.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewEnum SelectedItems Provider")]
	public class ListViewEnumSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListViewEnum, System.Collections.Generic.List<UIWidgets.ListViewEnum.Item>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewEnum target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewEnum);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewEnum);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UIWidgets.ListViewEnum.Item> GetValue(UIWidgets.ListViewEnum target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewEnum target)
		{

			target.OnSelectObject.RemoveListener(OnSelectObjectListViewEnum);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewEnum);
		}


		void OnSelectObjectListViewEnum(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewEnum(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif