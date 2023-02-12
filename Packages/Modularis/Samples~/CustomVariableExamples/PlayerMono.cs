using UnityEngine;

namespace Devkit.Modularis.Examples
{
	public class PlayerMono : MonoBehaviour
	{
		public PlayerReference aaa;
		private void OnEnable()
		{
			aaa.RegisterCallback(UpdateStuff);
		}

		private void OnDisable()
		{
			aaa.UnregisterCallback(UpdateStuff);
		}
		private void UpdateStuff()
		{
			Debug.Log(aaa.Value);
		}
	}
}
