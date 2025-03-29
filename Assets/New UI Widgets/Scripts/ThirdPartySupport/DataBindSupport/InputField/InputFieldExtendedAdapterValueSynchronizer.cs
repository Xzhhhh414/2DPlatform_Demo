#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Value of a InputFieldExtendedAdapter.
	/// </summary>
	public class InputFieldExtendedAdapterValueSynchronizer : ComponentDataSynchronizer<UIWidgets.InputFieldExtendedAdapter, System.String>
	{
		InputFieldExtendedAdapterValueObserver observer;

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
				observer = new InputFieldExtendedAdapterValueObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.InputFieldExtendedAdapter target, System.String newContextValue)
		{
			target.Value = newContextValue;
		}

		void OnObserverValueChanged()
		{
			OnComponentValueChanged(Target.Value);
		}
	}
}
#endif