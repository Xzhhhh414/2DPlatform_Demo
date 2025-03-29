#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an Rating.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] Rating Value Provider")]
	public class RatingValueProvider : ComponentDataProvider<UIWidgets.Rating, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Rating target)
		{

			target.OnChange.AddListener(OnChangeRating);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.Rating target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Rating target)
		{

			target.OnChange.RemoveListener(OnChangeRating);
		}


		void OnChangeRating(System.Int32 arg0)
		{
			OnTargetValueChanged();
		}

	}
}
#endif