using UnityEngine;

namespace Devkit.Modularis.Examples
{
    public class Item : MonoBehaviour
    {
        public ItemRuntimeSet RuntimeSet;

        private void OnEnable()
        {
            RuntimeSet.Add(this);
        }

        private void OnDisable()
        {
            RuntimeSet.Remove(this);
        }
    }
}