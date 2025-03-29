namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class to use MonoBehaviour with Init() method.
	/// </summary>
	public abstract class MonoBehaviourInitiable : MonoBehaviour
	{
		bool isInited;

		/// <summary>
		/// Is this instance inited?
		/// </summary>
		protected bool IsInited => isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start() => Init();

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