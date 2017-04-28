using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class HandGrabbingVisualiser : MonoBehaviour
    {
        private bool handHighlighted;

        [SerializeField] private GameObject Palm;
        [SerializeField] private GameObject RelaxedFingers;
        [SerializeField] private Material RelaxedHandMaterial;
        [SerializeField] private Material HighlightedHandMaterial;

        void OnEnable()
        {
            SetFingerMaterial(RelaxedFingers, RelaxedHandMaterial);            
        }

        private void SetFingerMaterial(GameObject fingers, Material material)
        {
            foreach (Transform finger in fingers.GetComponentsInChildren<Transform>())
            {
                foreach (Renderer fingerRenderer in finger.GetComponentsInChildren<Renderer>())
                {
                    fingerRenderer.material = material;
                }
            }
        }

        public void SetGrabbableInRange(bool inRange)
        {
            if (inRange != handHighlighted)
            {
                handHighlighted = inRange;
                SetHighlight(handHighlighted);
            }
        }

        private void SetHighlight(bool highlight)
        {
            var material = highlight ? HighlightedHandMaterial : RelaxedHandMaterial;
            SetFingerMaterial(RelaxedFingers, material);
            Palm.GetComponent<Renderer>().material = material;
        }
    }
}