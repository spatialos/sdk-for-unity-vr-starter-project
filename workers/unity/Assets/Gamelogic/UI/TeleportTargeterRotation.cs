using Assets.Gamelogic.Global;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class TeleportTargeterRotation : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(Vector3.up * SimulationSettings.TeleportTargeterRotationSpeed * Time.deltaTime);
        }
    }
}
