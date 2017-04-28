using Assets.Gamelogic.Global;
using Assets.Gamelogic.Player;
using Assets.Gamelogic.Utils;
using Improbable.General;
using Improbable.Global;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    public class GrabbableTransformHandler : MonoBehaviour
    {
        [Require] private Grabbable.Reader GrabbableReader;
        [Require] private WorldTransform.Reader WorldTransformReader;

        private GameObject grabberControllerGameObject;
        private Vector3 smoothedVelocity;
        private Vector3 previousPosition;

        [SerializeField] private Rigidbody modelRigidbody;

        private void OnEnable()
        {
            transform.position = WorldTransformReader.Data.position.ToVector3();
            transform.rotation = MathUtils.ToUnityQuaternion(WorldTransformReader.Data.rotation);

            previousPosition = transform.position;
            smoothedVelocity = Vector3.zero;
            UpdateGrabberObject();

            GrabbableReader.ComponentUpdated.Add(OnGrabbableUpdate);
            WorldTransformReader.ComponentUpdated.Add(OnTransformUpdate);
        }

        private void OnDisable()
        {
            GrabbableReader.ComponentUpdated.Remove(OnGrabbableUpdate);
            WorldTransformReader.ComponentUpdated.Remove(OnTransformUpdate);
        }

        private void OnGrabbableUpdate(Grabbable.Update update)
        {
            UpdateGrabberObject();
        }

        private void OnTransformUpdate(WorldTransform.Update update)
        {
            if (!CurrentlyBeingHeld() && !WorldTransformReader.HasAuthority)
            {
                if (update.position.HasValue)
                {
                    transform.position = update.position.Value.ToVector3();
                }
                if (update.rotation.HasValue)
                {
                    transform.rotation = MathUtils.ToUnityQuaternion(update.rotation.Value);
                }
            }
        }

        private void UpdateGrabberObject()
        {
            if (CurrentlyBeingHeld())
            {
                // May have been passed from one grabber to another
                SetGrabberObject();
            }
            else
            {
                RemoveGrabberObject();
            }
        }

        private bool CurrentlyBeingHeld()
        {
            return GrabbableReader.Data.currentGrabberInfo.HasValue;
        }

        private void SetGrabberObject()
        {
            var grabberEntityId = GrabbableReader.Data.currentGrabberInfo.Value.grabberEntity;
            var controllerSide = GrabbableReader.Data.currentGrabberInfo.Value.controllerSide;

            var grabberObject = SpatialOS.Universe.Get(grabberEntityId).UnderlyingGameObject;
            if (grabberObject != null)
            {
                SetGrabberController(grabberObject.GetComponent<PlayerControllers>().GetController(controllerSide));
            }
        }

        public void RemoveGrabberObject()
        {
            if (grabberControllerGameObject != null)
            {
                grabberControllerGameObject = null;
                if (WorldTransformReader.HasAuthority)
                {
                    modelRigidbody.isKinematic = false;
                    modelRigidbody.useGravity = true;
                    modelRigidbody.velocity = smoothedVelocity;
                }
            }
        }

        private void SetGrabberController(GameObject controllerObject)
        {
            modelRigidbody.isKinematic = true;
            modelRigidbody.useGravity = false;
            grabberControllerGameObject = controllerObject;
        }

        private void Update()
        {
            MatchGrabberControllerTransform();
        }

        private void MatchGrabberControllerTransform()
        {
            if (grabberControllerGameObject != null)
            {
                var grabberInfo = GrabbableReader.Data.currentGrabberInfo.Value;
                var grabberTransform = grabberControllerGameObject.transform;

                transform.position = grabberTransform.position + grabberTransform.rotation * grabberInfo.relativePosition.ToUnityVector();
                transform.rotation = grabberTransform.rotation * MathUtils.ToUnityQuaternion(grabberInfo.relativeOrientation);
                CacheCurrentVelocity(transform.position);
            }
        }

        private void CacheCurrentVelocity(Vector3 currentPosition)
        {
            smoothedVelocity = smoothedVelocity * 0.5f + (currentPosition - previousPosition) * 0.5f / Time.deltaTime;
            previousPosition = currentPosition;
        }
    }
}
