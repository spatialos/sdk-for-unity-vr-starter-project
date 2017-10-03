using Assets.Gamelogic.Player;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.General;
using Improbable.Global;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    public class GrabbableTransformHandler : MonoBehaviour
    {
        [Require] private Grabbable.Reader GrabbableReader;
        [Require] private Position.Reader PositionReader;
        [Require] private Rotation.Reader RotationReader;

        private GameObject grabberControllerGameObject;
        private Vector3 smoothedVelocity;
        private Vector3 previousPosition;

        [SerializeField] private Rigidbody modelRigidbody;

        private void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = RotationReader.Data.rotation.ToUnityQuaternion();

            previousPosition = transform.position;
            smoothedVelocity = Vector3.zero;
            UpdateGrabberObject();

            GrabbableReader.ComponentUpdated.Add(OnGrabbableUpdate);
            PositionReader.ComponentUpdated.Add(OnPositionUpdate);
            RotationReader.ComponentUpdated.Add(OnRotationUpdate);
        }

        private void OnDisable()
        {
            GrabbableReader.ComponentUpdated.Remove(OnGrabbableUpdate);
            PositionReader.ComponentUpdated.Remove(OnPositionUpdate);
            RotationReader.ComponentUpdated.Remove(OnRotationUpdate);
        }

        private void OnGrabbableUpdate(Grabbable.Update update)
        {
            UpdateGrabberObject();
        }

        private void OnPositionUpdate(Position.Update update)
        {
            if (!CurrentlyBeingHeld() && PositionReader.Authority == Authority.NotAuthoritative)
            {
                if (update.coords.HasValue)
                {
                    transform.position = update.coords.Value.ToUnityVector();
                }
            }
        }

        private void OnRotationUpdate(Rotation.Update update)
        {
            if (!CurrentlyBeingHeld() && RotationReader.Authority == Authority.NotAuthoritative)
            {
                if (update.rotation.HasValue)
                {
                    transform.rotation = update.rotation.Value.ToUnityQuaternion();
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
                if (PositionReader.Authority == Authority.Authoritative)
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
                transform.rotation = grabberTransform.rotation * grabberInfo.relativeOrientation.ToUnityQuaternion();
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
