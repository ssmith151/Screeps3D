using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.RoomObjects.Views
{
    class PowerBankView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleVisibility _powerScaleVisibility;
        private PowerBank _powerBank;

        [SerializeField] private float _Power;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _powerBank = roomObject as PowerBank;
        }

        public void Delta(JSONObject data)
        {
            var power = _Power > 0 ? _Power : _powerBank.Power;
            var percentage = power / _powerBank.PowerCapacity;
            Debug.Log("original power " + power);
            Debug.Log("power " + power);
            Debug.Log("percentage " + percentage);

            var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            var maxVisibility = 1f;

            // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            float minimum = Mathf.Exp(minVisibility);
            float maximum = Mathf.Exp(maxVisibility);

            // Scale the visibility in such a way that a lot of the model is rendered above 50% energy

            float current = Mathf.Exp(percentage == 0 ? minVisibility : percentage);

            // Map range to visibility range
            var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            _powerScaleVisibility.SetVisibility(visibility);
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}
