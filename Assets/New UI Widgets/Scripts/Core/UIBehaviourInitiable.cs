namespace UIWidgets
{
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class to use UIBehaviour with Init() method.
	/// </summary>
	public abstract class UIBehaviourInitiable : UIBehaviour
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
		/// Unmark instance as inited.
		/// Used in editor only.
		/// </summary>
		protected void DisableInit() => isInited = false;

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected virtual void InitOnce()
		{
		}
	}
}