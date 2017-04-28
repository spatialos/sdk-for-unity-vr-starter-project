using System.Collections.Generic;
using Assets.Gamelogic.Grabbing;
using Improbable.Global;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Unity.Core;
using Assets.Gamelogic.Utils;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class HandCollisionHandler : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer ClientAuthorityCheckWriter;

        private HashSet<GameObject> reachableObjects;
        private GameObject previousClosestObject;

        [SerializeField] private HandGrabbingVisualiser handGrabbingVisualiser;

        void OnEnable()
        {
            reachableObjects = new HashSet<GameObject>();
            previousClosestObject = null;
        }

        private static GameObject GetRootGameObject(GameObject gameObject)
        {
            var entity = gameObject.GetSpatialOsEntity();
            if (entity == null)
            {
                return null;
            }
            return entity.UnderlyingGameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ClientAuthorityCheckWriter != null)
            {
                reachableObjects.Add(GetRootGameObject(other.gameObject));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ClientAuthorityCheckWriter != null)
            {
                reachableObjects.Remove(GetRootGameObject(other.gameObject));
            }
        }

        public GameObject GetClosestReachableObject()
        {
            return previousClosestObject;
        }

        private void Update()
        {
            var closestObject = GetClosestReachableObject(reachableObjects);
            UpdateHighlights(previousClosestObject, closestObject);
            previousClosestObject = closestObject;
        }

        private GameObject GetClosestReachableObject(HashSet<GameObject> currentReachableObjects)
        {
            GameObject closestGameObject = null;
            float closestDistance = 0;
            foreach (GameObject reachableObject in currentReachableObjects)
            {
                float reachableObjectDistance = Vector3.Distance(transform.position, reachableObject.transform.position);
                if (closestGameObject == null || reachableObjectDistance < closestDistance)
                {
                    reachableObjectDistance = closestDistance;
                    closestGameObject = reachableObject;
                }
            }
            return closestGameObject;
        }

        private void UpdateHighlights(GameObject previousClosestObject, GameObject closestObject)
        {
            handGrabbingVisualiser.SetGrabbableInRange(closestObject != null);
            if (previousClosestObject != null && previousClosestObject != closestObject)
            {
                previousClosestObject.GetComponent<GrabbableVisualizer>().UpdateGrabbersInRange(-1);
            }
            if (closestObject != null && previousClosestObject != closestObject)
            {
                closestObject.GetComponent<GrabbableVisualizer>().UpdateGrabbersInRange(+1);
            }            
        }
    }
}
