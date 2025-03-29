#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an InputFieldAdapter.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] InputFieldAdapter Value Provider")]
	public class InputFieldAdapterValueProvider : ComponentDataProvider<UIWidgets.InputFieldAdapter, System.String>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.InputFieldAdapter target)
		{

			target.onValueChanged.AddListener(OnValueChangedInputFieldAdapter);
			target.onEndEdit.AddListener(OnEndEditInputFieldAdapter);
		}

		/// <inheritdoc />
		protected override System.String GetValue(UIWidgets.InputFieldAdapter target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.InputFieldAdapter target)
		{

			target.onValueChanged.RemoveListener(OnValueChangedInputFieldAdapter);
			target.onEndEdit.RemoveListener(OnEndEditInputFieldAdapter);
		}


		void OnValueChangedInputFieldAdapter(System.String arg0)
		{
			OnTargetValueChanged();
		}

		void OnEndEditInputFieldAdapter(System.String arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif