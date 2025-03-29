#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItem of an ListViewString.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewString SelectedItem Provider")]
	public class ListViewStringSelectedItemProvider : ComponentDataProvider<UIWidgets.ListViewString, System.String>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewString target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewString);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewString);
		}

		/// <inheritdoc />
		protected override System.String GetValue(UIWidgets.ListViewString target)
		{
			return target.SelectedItem;
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