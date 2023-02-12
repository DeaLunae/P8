using UnityEngine;

namespace Devkit.Modularis.Examples
{
    [CreateAssetMenu]
    public class StringVariable : ScriptableObject
    {
        [SerializeField]
        private string value = "";

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}