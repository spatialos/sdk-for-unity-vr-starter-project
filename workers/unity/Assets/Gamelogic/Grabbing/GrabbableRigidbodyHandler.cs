using Improbable.General;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Grabbing
{
    public class GrabbableRigidbodyHandler : MonoBehaviour {

        [Require] private WorldTransform.Writer WorldTransformWriter;

        [SerializeField] private Rigidbody modelRigidbody;

        private void OnEnable()
        {
            InitialiseAuthoritativeRigidbody();
        }

        private void InitialiseAuthoritativeRigidbody()
        {
            modelRigidbody.isKinematic = false;
            modelRigidbody.useGravity = true;
        }
    }
}
