using Improbable.Global;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class ClientCameraEnabler : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer ClientAuthorityCheckWriter;

        [SerializeField] private GameObject CameraRig;

        void OnEnable()
        {
            CameraRig.SetActive(true);
        }
    }
}