using Improbable;
using Improbable.Collections;
using Improbable.Global;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class GrabbableVisualizer : MonoBehaviour
    {
        [Require] private Grabbable.Reader GrabbableReader;

        private int grabbersInRange;

        private Material DefaultMaterial;
        [SerializeField] private Material HighlightedMaterial;

        // Store reference to entity rather than local underlying game object
        private Option<EntityId> grabberEntityId;
        private ControllerSide grabberControllerSide;

        private void OnEnable()
        {
            DefaultMaterial = GetComponent<Renderer>().material;
            GrabbableReader.ComponentUpdated.Add(OnGrabbableUpdated);
            grabberEntityId = new Option<EntityId>();
        }

        private void OnGrabbableUpdated(Grabbable.Update update)
        {
            if (!update.currentGrabberInfo.HasValue)
            {
                return;
            }

            var previousGrabberEntityId = grabberEntityId;
            ControllerSide previousGrabberControllerSide = grabberControllerSide;
            UpdateLocalCurrentGrabberInfo(update.currentGrabberInfo.Value);

            if (grabberEntityId != previousGrabberEntityId ||
                grabberControllerSide != previousGrabberControllerSide)
            {
                // Current grabber has changed or side
                if (previousGrabberEntityId.HasValue)
                {
                    // Notify the previous grabber entity it no longer holds the item
                    GameObject previousControllerGameObject = SpatialOS.Universe.Get(previousGrabberEntityId.Value).UnderlyingGameObject;
                    GrabbingSender grabbingSender = previousControllerGameObject.GetComponent<GrabbingSender>();

                    // GrabbingSender will be disabled on workers not authoritative over dropped object's Grabbable 
                    if (grabbingSender != null && grabbingSender.isActiveAndEnabled)
                    {
                        grabbingSender.ForceLocalDrop(previousGrabberControllerSide);
                    }
                }

                if (grabberEntityId.HasValue)
                {
                    // Update grabbing on the new game object
                    GameObject controllerGameObject = SpatialOS.Universe.Get(grabberEntityId.Value).UnderlyingGameObject;
                    GrabbingSender grabbingSender = controllerGameObject.GetComponent<GrabbingSender>();
                    if (grabbingSender != null && grabbingSender.isActiveAndEnabled)
                    {
                        grabbingSender.UpdateHeldEntity(grabberControllerSide, gameObject.EntityId());
                    }
                }
            }
        }

        private void UpdateLocalCurrentGrabberInfo(Option<CurrentGrabberInfo> infoOption)
        {
            CurrentGrabberInfo currentGrabberInfo;
            if (infoOption.TryGetValue(out currentGrabberInfo))
            {
                // Update local variables with new grabber
                grabberEntityId = currentGrabberInfo.grabberEntity;
                grabberControllerSide = currentGrabberInfo.controllerSide;
            }
            else
            {
                // No current grabber, so indicate this with an empty option
                grabberEntityId.Clear();
            }
        }

        public void UpdateGrabbersInRange(int deltaCount)
        {
            grabbersInRange += deltaCount;
            SetHighlight(grabbersInRange > 0);
        }

        private void SetHighlight(bool highlighted)
        {
            GetComponent<Renderer>().material = highlighted ? HighlightedMaterial : DefaultMaterial;
        }
    }
}
