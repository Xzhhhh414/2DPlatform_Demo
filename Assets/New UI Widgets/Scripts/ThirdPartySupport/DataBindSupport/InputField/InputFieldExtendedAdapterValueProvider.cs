#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an InputFieldExtendedAdapter.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] InputFieldExtendedAdapter Value Provider")]
	public class InputFieldExtendedAdapterValueProvider : ComponentDataProvider<UIWidgets.InputFieldExtendedAdapter, System.String>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.InputFieldExtendedAdapter target)
		{

			target.onValueChanged.AddListener(OnValueChangedInputFieldExtendedAdapter);
			target.onEndEdit.AddListener(OnEndEditInputFieldExtendedAdapter);
		}

		/// <inheritdoc />
		protected override System.String GetValue(UIWidgets.InputFieldExtendedAdapter target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.InputFieldExtendedAdapter target)
		{

			target.onValueChanged.RemoveListener(OnValueChangedInputFieldExtendedAdapter);
			target.onEndEdit.RemoveListener(OnEndEditInputFieldExtendedAdapter);
		}


		void OnValueChangedInputFieldExtendedAdapter(System.String arg0)
		{
			OnTargetValueChanged();
		}

		void OnEndEditInputFieldExtendedAdapter(System.String arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif