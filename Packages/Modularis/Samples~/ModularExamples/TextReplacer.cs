using UnityEngine;
using UnityEngine.UI;

namespace Devkit.Modularis.Examples
{
    public class TextReplacer : MonoBehaviour
    {
        public Text Text;

        public StringVariable Variable;

        public bool AlwaysUpdate;
        
        private void OnEnable()
        {
            Text.text = Variable.Value;
        }

        private void Update()
        {
            if (AlwaysUpdate)
            {
                Text.text = Variable.Value;
            }
        }
    }
}