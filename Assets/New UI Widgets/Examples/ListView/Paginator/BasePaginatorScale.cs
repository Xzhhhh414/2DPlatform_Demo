namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Helper for Paginator to scale centered slide.
	/// </summary>
	/// <typeparam name="T">Type of slide component.</typeparam>
	public abstract class BasePaginatorScale<T> : MonoBehaviourInitiable
		where T : Component
	{
		/// <summary>
		/// Custom function to set instance state.
		/// </summary>
		public Action<T, float> CustomSetInstanceState;

		/// <summary>
		/// Paginator.
		/// </summary>
		[SerializeField]
		protected ScrollRectPaginator Paginator;

		/// <summary>
		/// Scale.
		/// </summary>
		[SerializeField]
		public float Scale = 1.25f;

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected override void InitOnce()
		{
			base.InitOnce();

			CustomSetInstanceState ??= DefaultSetInstanceState;

			Paginator.OnMovement.AddListener(UpdateState);
			Paginator.Init();
			UpdateState(Paginator.CurrentPage, 0f);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected virtual void OnDestroy()
		{
			if (Paginator != null)
			{
				Paginator.OnMovement.RemoveListener(UpdateState);
			}
		}

		/// <summary>
		/// Default function to set instance state.
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="ratio">Reverse visible ratio (0 at center, 1 at left or right).</param>
		protected virtual void DefaultSetInstanceState(T instance, float ratio)
		{
			var t = instance.transform.GetChild(0);
			var s = Mathf.Lerp(Scale, 1f, ratio);

			t.localScale = new Vector3(s, s, s);
		}

		/// <summary>
		/// Update slide state.
		/// </summary>
		/// <param name="index">Slide index.</param>
		/// <param name="ratio">Reverse visible ratio (0 at center, 1 at left or right).</param>
		protected virtual void UpdateState(int index, float ratio)
		{
			SetInstanceState(index, ratio);
			SetInstanceState(index + 1, 1f - ratio);
			SetInstanceState(index + 2, ratio);
		}

		/// <summary>
		/// Get slide instance by index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Instance.</returns>
		protected abstract T GetInstance(int index);

		/// <summary>
		/// Set instance state.
		/// </summary>
		/// <param name="index">Instance index.</param>
		/// <param name="ratio">Reverse visible ratio (0 at center, 1 at left or right).</param>
		protected virtual void SetInstanceState(int index, float ratio)
		{
			if (CustomSetInstanceState == null)
			{
				return;
			}

			var instance = GetInstance(index);
			if (instance == null)
			{
				return;
			}

			CustomSetInstanceState(instance, ratio);
		}
	}
}