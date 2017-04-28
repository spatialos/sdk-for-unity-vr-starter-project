using Assets.Gamelogic.Utils;
using Improbable.General;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class SpectatorFlycam : MonoBehaviour
    {
        [Require] private WorldTransform.Writer WorldTransformWriter;

        private float yaw;
        private float pitch;
        private const float movementSpeed = 10.0f;

        [SerializeField] private GameObject SpectatorCamera;

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            SpectatorCamera.SetActive(true);
        }

        void Update()
        {
            HandleRotationMovement();
            HandlePositionMovement();
            var rot = transform.rotation;
            WorldTransformWriter.Send(new WorldTransform.Update().SetPosition(transform.position.ToCoordinates()).SetRotation(new Improbable.Global.Quaternion(rot.x, rot.y, rot.z, rot.w)));
        }

        private void HandlePositionMovement()
        {
            Vector3 targetDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                targetDirection += transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                targetDirection -= transform.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                targetDirection -= transform.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                targetDirection += transform.right;
            }
            transform.position += targetDirection * movementSpeed * Time.deltaTime;
        }

        private void HandleRotationMovement()
        {
            yaw = (yaw + Input.GetAxis("Mouse X")) % 360f;
            pitch = (pitch - Input.GetAxis("Mouse Y")) % 360f;
            transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
        }
    }
}
