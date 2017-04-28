using Assets.Gamelogic.Global;
using Assets.Gamelogic.Utils;
using Improbable.General;
using Improbable.Player;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class TeleportationHandler : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;

        [Require]
        private VRPeripheralOffsets.Reader VROffsetsReader;

        private GameObject teleportTargeter;
        private GameObject targetingController;
        private bool teleportTargetingActive;

        [SerializeField]
        private GameObject TeleportTargeterModel;
        [SerializeField]
        private SteamVR_TrackedController LeftController;
        [SerializeField]
        private SteamVR_TrackedController RightController;

        void OnEnable()
        {
            LeftController.PadClicked += ActivateTeleportTargeter;
            RightController.PadClicked += ActivateTeleportTargeter;

            LeftController.PadUnclicked += AttemptToTeleport;
            RightController.PadUnclicked += AttemptToTeleport;
        }

        private void ActivateTeleportTargeter(object sender, ClickedEventArgs e)
        {
            if (!teleportTargetingActive) // other pad already teleport targeting
            {
                CreateTeleportTargeterInstance();
                teleportTargetingActive = true;
            }
            SetTargetingController(e.controllerIndex);
        }

        private void SetTargetingController(uint controllerIndex)
        {
            if (controllerIndex == LeftController.controllerIndex)
            {
                targetingController = LeftController.gameObject;
            }
            else
            {
                targetingController = RightController.gameObject;
            }
        }

        private void AttemptToTeleport(object sender, ClickedEventArgs e)
        {
            if (!teleportTargetingActive)
            {
                return;
            }
            // The player position represents the position of the Vive play area. We need to take into account
            // the position of the head relative to the play area so that the teleport feels intuitive.
            var headGroundOffset = VROffsetsReader.Data.head.position.ToUnityVector();
            headGroundOffset.y = 0;
            var targetTeleportPosition = teleportTargeter.transform.position - headGroundOffset;

            DeactivateTeleportTargeting();
            UpdatePlayerPosition(targetTeleportPosition);
        }

        private void DeactivateTeleportTargeting()
        {
            teleportTargetingActive = false;
            Destroy(teleportTargeter);
        }

        private void UpdatePlayerPosition(Vector3 targetTeleportPosition)
        {
            transform.position = targetTeleportPosition;
            WorldTransformWriter.Send(new WorldTransform.Update().SetPosition(targetTeleportPosition.ToCoordinates()));
        }

        private void CreateTeleportTargeterInstance()
        {
            teleportTargeter = Instantiate(TeleportTargeterModel);
            teleportTargeter.transform.localScale = new Vector3(SimulationSettings.TeleportTargeterDiameter, 1f, SimulationSettings.TeleportTargeterDiameter);
        }

        private void Update()
        {
            if (teleportTargetingActive)
            {
                UpdateTeleportTargetPosition();
            }
        }

        private void UpdateTeleportTargetPosition()
        {
            Ray controllerRay = new Ray(targetingController.transform.position, -targetingController.transform.up);
            var terrainLayerMask = 1 << LayerMask.NameToLayer(SimulationSettings.TerrainLayerName);
            RaycastHit hit;
            if (Physics.Raycast(controllerRay, out hit, SimulationSettings.MaxTeleportDistance, terrainLayerMask))
            {
                teleportTargeter.transform.position = hit.point + new Vector3(0.0f, 0.1f, 0.0f);
            }
        }
    }
}
