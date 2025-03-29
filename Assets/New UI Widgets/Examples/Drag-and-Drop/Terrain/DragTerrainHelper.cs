namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Helper for the drag-and-drop to terrain.
	/// </summary>
	public class DragTerrainHelper : MonoBehaviour
	{
		/// <summary>
		/// Images to disable to make terrain visible.
		/// </summary>
		[SerializeField]
		protected Image[] DisableImages = new Image[] { };

		/// <summary>
		/// Dropped objects.
		/// </summary>
		readonly List<GameObject> droppedObjects = new List<GameObject>();

		/// <summary>
		/// Drop2Terrain.
		/// </summary>
		DropGameObject2Terrain drop2Terrain;

		/// <summary>
		/// Terrain.
		/// </summary>
		[SerializeField]
		protected Terrain Terrain;

		/// <summary>
		/// Process enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if ((drop2Terrain == null) && (Terrain != null))
			{
				Terrain.TryGetComponent(out drop2Terrain);
			}

			Toggle(true);
		}

		/// <summary>
		/// Process disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Toggle(false);
		}

		void ProcessDrop(GameObject go)
		{
			droppedObjects.Add(go);
		}

		/// <summary>
		/// Toggle state.
		/// </summary>
		/// <param name="enabled">Enabled.</param>
		protected virtual void Toggle(bool enabled)
		{
			if (drop2Terrain != null)
			{
				if (enabled)
				{
					drop2Terrain.OnDrop += ProcessDrop;
				}
				else
				{
					drop2Terrain.OnDrop -= ProcessDrop;
				}
			}

			foreach (var image in DisableImages)
			{
				image.enabled = !enabled;
			}

			if (Terrain != null)
			{
				Terrain.gameObject.SetActive(enabled);
			}

			foreach (var go in droppedObjects)
			{
				if (go != null)
				{
					go.SetActive(enabled);
				}
			}
		}
	}
}