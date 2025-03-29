#if UNITY_EDITOR
namespace UIWidgets
{
	/// <summary>
	/// Menu options to toggle third party packages support.
	/// </summary>
	public static class ThirdPartySupportMenuOptions
	{
		/// <summary>
		/// Upgrade widgets at scene.
		/// </summary>
		// [MenuItem("Window/New UI Widgets/Upgrade Widgets at Scene")]
		public static void Upgrade()
		{
			#if UNITY_2017_1_OR_NEWER
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				if (!scene.IsValid() || !scene.isLoaded)
				{
					continue;
				}

				var gameobjects = scene.GetRootGameObjects();
				foreach (var go in gameobjects)
				{
					var components = go.GetComponentsInChildren<IUpgradeable>();
					foreach (var component in components)
					{
						component.Upgrade();
					}
				}
			}
			#endif
		}
	}
}
#endif