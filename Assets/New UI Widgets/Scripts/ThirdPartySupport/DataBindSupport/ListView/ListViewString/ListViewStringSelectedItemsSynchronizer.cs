#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the SelectedItems of a ListViewString.
	/// </summary>
	public class ListViewStringSelectedItemsSynchronizer : ComponentDataSynchronizer<UIWidgets.ListViewString, System.Collections.Generic.List<System.String>>
	{
		ListViewStringSelectedItemsObserver observer;

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
				observer = new ListViewStringSelectedItemsObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.ListViewString target, System.Collections.Generic.List<System.String> newContextValue)
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