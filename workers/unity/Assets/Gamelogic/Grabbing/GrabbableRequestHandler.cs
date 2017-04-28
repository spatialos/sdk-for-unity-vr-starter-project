using Improbable;
using Improbable.Global;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Collections;
using UnityEngine;
using Assets.Gamelogic.Utils;

namespace Assets.Gamelogic.Grabbing
{
    // Runs on server-side grabbable entity game object
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class GrabbableRequestHandler : MonoBehaviour
    {
        [Require] private Grabbable.Writer GrabbableWriter;

        public void HandleGrabRequest(EntityId grabberEntityId, ControllerSide controllerSide, GameObject controllerGameObject)
        {
            if (controllerGameObject == null)
            {
                return;
            }

            // Check collisions to determine grab legality
            Collider controllerCollider = controllerGameObject.GetComponent<Collider>();
            if (controllerCollider == null)
            {
                return;
            }

            if (AnyCollision(controllerCollider, gameObject.GetComponentsInChildren<Collider>()))
            {
                var inverseControllerOrientation = UnityEngine.Quaternion.Inverse(controllerGameObject.transform.rotation);
                var relativeOrientation = inverseControllerOrientation * gameObject.transform.rotation;
                var relativePosition = inverseControllerOrientation * (gameObject.transform.position - controllerGameObject.transform.position);

                var grabberInfo = new CurrentGrabberInfo(grabberEntityId, controllerSide, relativePosition.ToVector3f(), MathUtils.ToNativeQuaternion(relativeOrientation));
                GrabbableWriter.Send(new Grabbable.Update().SetCurrentGrabberInfo(grabberInfo));
            }
        }

        private bool AnyCollision(Collider singleCollider, Collider[] otherColliders)
        {
            foreach (var collider in otherColliders)
            {
                if (collider.bounds.Intersects(singleCollider.bounds))
                {
                    return true;
                }
            }
            return false;
        }

        public void HandleDropRequest()
        {
            GrabbableWriter.Send(new Grabbable.Update().SetCurrentGrabberInfo(new Option<CurrentGrabberInfo>()));
        }
    }
}
