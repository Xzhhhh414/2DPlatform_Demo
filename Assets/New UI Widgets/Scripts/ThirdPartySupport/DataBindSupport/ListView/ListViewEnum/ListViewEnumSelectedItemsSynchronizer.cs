#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the SelectedItems of a ListViewEnum.
	/// </summary>
	public class ListViewEnumSelectedItemsSynchronizer : ComponentDataSynchronizer<UIWidgets.ListViewEnum, System.Collections.Generic.List<UIWidgets.ListViewEnum.Item>>
	{
		ListViewEnumSelectedItemsObserver observer;

		/// <inheritdoc />
		public override void Disable()
		{
			base.Disable();
			
			if (observer != null)
			{
				observer.ValueChanged -= OnObserverValueChanged;
				observer = null;
			}
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();

			var target = Target;
			if (target != null)
			{
				observer = new ListViewEnumSelectedItemsObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.ListViewEnum target, System.Collections.Generic.List<UIWidgets.ListViewEnum.Item> newContextValue)
		{
			target.SelectedItems = newContextValue;
		}

		void OnObserverValueChanged()
		{
			OnComponentValueChanged(Target.SelectedItems);
		}
	}
}
#endif