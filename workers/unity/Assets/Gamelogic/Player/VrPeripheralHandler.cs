using Assets.Gamelogic.Utils;
using Improbable.Player;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class VrPeripheralHandler : MonoBehaviour
    {
        [Require] private VRPeripheralOffsets.Writer VRPeripheralOffsetsWriter;

        [SerializeField] private GameObject VrCamera;
        [SerializeField] private GameObject LeftController;
        [SerializeField] private GameObject RightController;

        private float LastWarningTimestamp;

        void Update()
        {
            if ((!VrCamera.activeInHierarchy || !LeftController.activeInHierarchy || !RightController.activeInHierarchy) && (Time.time - LastWarningTimestamp) >= 1)
            {
                LastWarningTimestamp = Time.time;
                Debug.LogWarning("One or more VR Peripherals not active in hierarchy!");
            }

            var previousOffsets = VRPeripheralOffsetsWriter.Data;
            var offsetsUpdate = new VRPeripheralOffsets.Update();
            var newOffset = new TransformOffset();

            if (LeftController.activeInHierarchy && ShouldUpdateOffset(LeftController.transform, previousOffsets.leftController, ref newOffset))
            {
                offsetsUpdate.SetLeftController(newOffset);
            }

            if (RightController.activeInHierarchy && ShouldUpdateOffset(RightController.transform, previousOffsets.rightController, ref newOffset))
            {
                offsetsUpdate.SetRightController(newOffset);
            }

            if (VrCamera.activeInHierarchy && ShouldUpdateOffset(VrCamera.transform, previousOffsets.head, ref newOffset))
            {
                offsetsUpdate.SetHead(newOffset);
            }

            VRPeripheralOffsetsWriter.Send(offsetsUpdate);
        }

        private bool ShouldUpdateOffset(Transform currentElementTransform, TransformOffset previousElementOffset, ref TransformOffset newOffset)
        {
            newOffset.position = currentElementTransform.localPosition.ToNativeVector3f();
            newOffset.rotation = currentElementTransform.localRotation.eulerAngles.ToNativeVector3f();
            return !(MathUtils.ApproximatelyEqual(newOffset.position, previousElementOffset.position)
                && MathUtils.ApproximatelyEqual(newOffset.rotation, previousElementOffset.rotation));
        }
    }
}