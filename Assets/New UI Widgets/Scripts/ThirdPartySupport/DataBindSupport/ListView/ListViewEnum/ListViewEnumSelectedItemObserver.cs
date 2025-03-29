#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewEnum.
	/// </summary>
	public class ListViewEnumSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewEnum, UIWidgets.ListViewEnum.Item>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewEnum target)
		{

			target.OnSelectObject.AddListener(OnSelectObjectListViewEnum);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewEnum);
		}

		/// <inheritdoc />
		protected override UIWidgets.ListViewEnum.Item GetValue(UIWidgets.ListViewEnum target)
		{
			return target.SelectedItem;
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