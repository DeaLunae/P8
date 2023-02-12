using UnityEngine;
using UnityEngine.UI;

namespace Devkit.Modularis.Examples
{
    public class ItemMonitor : MonoBehaviour
    {
        public ItemRuntimeSet Set;
        
        public Text Text;
        
        private void OnEnable()
        {
            Set.RegisterCallback(UpdateText);
        }
        private void OnDisable()
        {
            Set.UnregisterCallback(UpdateText);
        }

        public void UpdateText()
        {
            Text.text = "There are " + Set.Items.Count + " items.";
        }
    }
}