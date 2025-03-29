namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Tooltip viewer.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TTooltip">Tooltip type.</typeparam>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/misc/tooltip.html")]
	public class TooltipViewer<TData, TTooltip> : MonoBehaviourInitiable
		where TTooltip : Tooltip<TData, TTooltip>
	{
		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		protected TTooltip Tooltip;

		/// <summary>
		/// Data.
		/// </summary>
		[SerializeField]
		protected TData data;

		/// <summary>
		/// Data.
		/// </summary>
		public TData Data
		{
			get => data;

			set
			{
				data = value;
				Init();
			}
		}

		/// <summary>
		/// Settings.
		/// </summary>
		[SerializeField]
		protected TooltipSettings settings;

		/// <summary>
		/// Settings.
		/// </summary>
		public TooltipSettings Settings
		{
			get => settings;

			set
			{
				settings = value;
				Init();
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (!Utilities.IsNull(Tooltip))
			{
				Tooltip.Register(gameObject, Data, Settings);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (!Utilities.IsNull(Tooltip))
			{
				Tooltip.Unregister(gameObject);
			}
		}
	}
}