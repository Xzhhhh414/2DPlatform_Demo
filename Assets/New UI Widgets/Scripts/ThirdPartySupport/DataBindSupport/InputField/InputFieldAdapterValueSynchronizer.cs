#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the Value of a InputFieldAdapter.
	/// </summary>
	public class InputFieldAdapterValueSynchronizer : ComponentDataSynchronizer<UIWidgets.InputFieldAdapter, System.String>
	{
		InputFieldAdapterValueObserver observer;

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
				observer = new InputFieldAdapterValueObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.InputFieldAdapter target, System.String newContextValue)
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