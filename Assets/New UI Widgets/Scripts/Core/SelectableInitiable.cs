namespace UIWidgets
{
	using UnityEngine.UI;

	/// <summary>
	/// Base class to use Selectable with Init() method.
	/// </summary>
	public abstract class SelectableInitiable : Selectable
	{
		bool isInited;

		/// <summary>
		/// Is this instance inited?
		/// </summary>
		protected bool IsInited => isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			InitOnce();
		}

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected virtual void InitOnce()
		{
		}
	}
}