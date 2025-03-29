#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListViewString.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewString SelectedItems Provider")]
	public class ListViewStringSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListViewString, System.Collections.Generic.List<System.String>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewString target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewString);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewString);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<System.String> GetValue(UIWidgets.ListViewString target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewString target)
		{

			target.OnSelectObject.RemoveListener(OnSelectObjectListViewString);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewString);
		}


		void OnSelectObjectListViewString(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewString(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif