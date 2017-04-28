using Improbable.Player;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class PlayerControllers : MonoBehaviour
    {
        [SerializeField] private GameObject LeftController;
        [SerializeField] private GameObject RightController;

        public GameObject GetController(ControllerSide controllerSide)
        {
            if (controllerSide.Equals(ControllerSide.LEFT))
            {
                return LeftController;
            }
            else
            {
                return RightController;
            }
        }
    }
}
