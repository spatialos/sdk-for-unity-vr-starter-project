using Improbable.Player;
using UnityEngine;

namespace Assets.Gamelogic.Utils
{
    public static class VRUtils {

        public static ControllerSide OtherController(ControllerSide controllerSide)
        {
            if (controllerSide.Equals(ControllerSide.LEFT))
            {
                return ControllerSide.RIGHT;
            }
            else
            {
                return ControllerSide.LEFT;
            }
        }
    }
}
