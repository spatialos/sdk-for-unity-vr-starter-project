using Assets.Gamelogic.Player;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    // Runs on server-side player game object
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class GrabbingReceiver : MonoBehaviour
    {
        [Require] private Grab.Reader GrabReader;

        private void OnEnable()
        {
            GrabReader.GrabRequestTriggered.Add(AttemptGrab);
            GrabReader.DropRequestTriggered.Add(AttemptDrop);
        }

        private void OnDisable()
        {
            GrabReader.GrabRequestTriggered.Remove(AttemptGrab);
            GrabReader.DropRequestTriggered.Remove(AttemptDrop);
        }

        private void AttemptGrab(GrabRequestEvent request)
        {
            var grabbableGameObject = SpatialOS.Universe.Get(request.grabbedEntityId).UnderlyingGameObject;
            if (grabbableGameObject == null)
            {
                Debug.LogWarning("Player grab attempt couldn't find targeted grabbable entity object with id: " + request.grabbedEntityId);
                return;
            }

            GameObject controllerGameObject = gameObject.GetComponent<PlayerControllers>().GetController(request.controllerSide);
            if (controllerGameObject == null)
            {
                Debug.LogWarning("Player grab attempt failed; no controller game object for " + request.controllerSide + " controller side");
                return;
            }

            // Make grab request
            GrabbableRequestHandler grabbableRequestHandler = grabbableGameObject.GetComponent<GrabbableRequestHandler>();

            // GrabbableRequestHandler will be disabled on workers not authoritative over grabbed object's Grabbable component
            if (grabbableRequestHandler != null && grabbableRequestHandler.isActiveAndEnabled)
            {                
                grabbableRequestHandler.HandleGrabRequest(gameObject.EntityId(), request.controllerSide, controllerGameObject);
            }
        }

        private void AttemptDrop(DropRequestEvent request)
        {
            GameObject droppedGameObject = SpatialOS.Universe.Get(request.droppedEntityId).UnderlyingGameObject;
            if (droppedGameObject == null)
            {
                Debug.LogWarning("Player drop attempt couldn't find targeted grabbable entity object with id: " + request.droppedEntityId);
                return;
            }

            // Make drop request
            GrabbableRequestHandler grabbableRequestHandler = droppedGameObject.GetComponent<GrabbableRequestHandler>();

            // GrabbableRequestHandler will be disabled on workers not authoritative over dropped object's Grabbable component
            if (grabbableRequestHandler != null && grabbableRequestHandler.isActiveAndEnabled)
            {                
                grabbableRequestHandler.HandleDropRequest();
            }
        }
    }
}
