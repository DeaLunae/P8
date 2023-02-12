using UnityEngine;

namespace Devkit.Modularis.Examples
{
    public class SimpleUnitHealth : MonoBehaviour
    {
        public FloatVariable HP;

        public bool ResetHP;

        public FloatReference StartingHP;

        private void Start()
        {
            if (ResetHP)
                HP.SetValue(StartingHP);
        }

        private void OnTriggerEnter(Collider other)
        {
            DamageDealer damage = other.gameObject.GetComponent<DamageDealer>();
            if (damage != null)
                HP.ApplyChange(-damage.DamageAmount);
        }
    }
}