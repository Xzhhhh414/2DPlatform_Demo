#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewString.
	/// </summary>
	public class ListViewStringSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewString, System.String>
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