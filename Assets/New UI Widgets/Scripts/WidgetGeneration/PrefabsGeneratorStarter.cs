namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Prefabs generator.
	/// </summary>
	[ExecuteInEditMode]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class PrefabsGeneratorStarter : MonoBehaviour
	{
		/// <summary>
		/// Full type name of the prefabs generator class.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public string Generator;

		bool isRunning;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start() => Run();

		/// <summary>
		/// Run.
		/// </summary>
		public virtual void Run()
		{
			if (isRunning)
			{
				return;
			}

			if (Application.isPlaying || string.IsNullOrEmpty(Generator))
			{
				Destroy(this);
				return;
			}

			var type = UtilitiesEditor.GetType(Generator);
			if (type == null)
			{
				DestroyImmediate(gameObject);
				return;
			}

			isRunning = true;
			StartCoroutine(RunGeneration(type));
		}

		IEnumerator RunGeneration(Type type)
		{
			yield return null; // delay 1 frame

			var method = type.GetMethod("Run");
			method.Invoke(null, new object[] { });

			DestroyImmediate(gameObject);
		}
	}
}