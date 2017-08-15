using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class Spinner : MonoBehaviour
    {

        [SerializeField] private float rotationAmount = 270.0f;

        // Update is called once per frame
        void Update()
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = rot.z + rotationAmount * Time.deltaTime;
            if (rot.z > 360)
                rot.z -= 360;
            else if (rot.z < 360)
                rot.z += 360;

            transform.eulerAngles = rot;
        }
    }
}
