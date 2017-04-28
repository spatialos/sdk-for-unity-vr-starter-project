using Assets.Gamelogic.Player;
using Improbable;
using Improbable.Collections;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    // Runs on client-side player game object
    [WorkerType(WorkerPlatform.UnityClient)]
    public class GrabbingSender : MonoBehaviour
    {
        [Require] private Grab.Writer GrabWriter;

        [SerializeField] private GameObject LeftHand;
        [SerializeField] private GameObject RightHand;
        [SerializeField] private SteamVR_TrackedController LeftController;
        [SerializeField] private SteamVR_TrackedController RightController;

        private void OnEnable()
        {
            LeftController.TriggerClicked += AttemptPickup;
            RightController.TriggerClicked += AttemptPickup;

            LeftController.TriggerUnclicked += AttemptDrop;
            RightController.TriggerUnclicked += AttemptDrop;
        }

        private void AttemptPickup(object sender, ClickedEventArgs e)
        {
            ControllerSide controllerSide;
            GameObject reachableObject;

            if (ReceivedEventFrom(e, LeftController))
            {
                controllerSide = ControllerSide.LEFT;
                reachableObject = LeftHand.GetComponent<HandCollisionHandler>().GetClosestReachableObject();
            }
            else
            {
                controllerSide = ControllerSide.RIGHT;
                reachableObject = RightHand.GetComponent<HandCollisionHandler>().GetClosestReachableObject();
            }

            if (reachableObject != null)
            {
                SendPickupRequest(controllerSide, reachableObject.EntityId());
            }
        }

        private void SendPickupRequest(ControllerSide controllerSide, EntityId reachableObjectId)
        {
            var update = new Grab.Update();
            var grabRequestEvent = new GrabRequestEvent(reachableObjectId, controllerSide);
            update.AddGrabRequest(grabRequestEvent);
            GrabWriter.Send(update);
        }

        public void UpdateHeldEntity(ControllerSide controllerSide, Option<EntityId> grabbedObjectId)
        {
            var heldEntities = GrabWriter.Data.heldEntities;
            heldEntities.Remove(controllerSide);
            if (grabbedObjectId.HasValue)
            {
                heldEntities.Add(controllerSide, grabbedObjectId.Value);
            }
            GrabWriter.Send(new Grab.Update().SetHeldEntities(heldEntities));
        }

        private void AttemptDrop(object sender, ClickedEventArgs e)
        {
            EntityId heldObjectId;
            if (ReceivedEventFrom(e, LeftController) && HoldingSomethingWith(ControllerSide.LEFT, out heldObjectId))
            {
                SendDropRequest(heldObjectId);
            }
            else if (ReceivedEventFrom(e, RightController) && HoldingSomethingWith(ControllerSide.RIGHT, out heldObjectId))
            {
                SendDropRequest(heldObjectId);
            }
        }

        private bool ReceivedEventFrom(ClickedEventArgs e, SteamVR_TrackedController controller)
        {
            return e.controllerIndex == controller.controllerIndex;
        }

        private void SendDropRequest(EntityId heldObjectId)
        {
            var update = new Grab.Update();
            var dropRequestEvent = new DropRequestEvent(heldObjectId);
            update.AddDropRequest(dropRequestEvent);
            GrabWriter.Send(update);
        }

        private bool HoldingSomethingWith(ControllerSide controllerSide, out EntityId heldItem)
        {
            return GrabWriter.Data.heldEntities.TryGetValue(controllerSide, out heldItem);
        }

        // Provides a way for grabbable objects to inform player they are not holding object
        public void ForceLocalDrop(ControllerSide controllerSide)
        {
            UpdateHeldEntity(controllerSide, new Option<EntityId>());
        }
    }
}

