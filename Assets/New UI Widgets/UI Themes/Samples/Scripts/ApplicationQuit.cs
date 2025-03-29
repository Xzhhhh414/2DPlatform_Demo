namespace UIThemes.Samples
{
	using UnityEngine;

	/// <summary>
	/// Quit option for standalone build.
	/// </summary>
	public class ApplicationQuit : MonoBehaviour
	{
#if !UNITY_STANDALONE
		/// <summary>
		/// Disable gameobject if not standalone build.
		/// </summary>
		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
#endif

		/// <summary>
		/// Quit.
		/// </summary>
		public virtual void Quit()
		{
			Application.Quit();
		}
	}
}