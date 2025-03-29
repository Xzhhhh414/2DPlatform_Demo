#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the SelectedItems of a ListViewColors.
	/// </summary>
	public class ListViewColorsSelectedItemsSynchronizer : ComponentDataSynchronizer<UIWidgets.ListViewColors, System.Collections.Generic.List<UnityEngine.Color>>
	{
		ListViewColorsSelectedItemsObserver observer;

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
				observer = new ListViewColorsSelectedItemsObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.ListViewColors target, System.Collections.Generic.List<UnityEngine.Color> newContextValue)
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