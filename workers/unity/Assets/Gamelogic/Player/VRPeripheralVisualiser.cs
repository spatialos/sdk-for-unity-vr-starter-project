using System.Collections.Generic;
using Improbable;
using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Player
{
    public class VRPeripheralVisualiser : MonoBehaviour
    {
        [Require] private VRPeripheralOffsets.Reader VRPeripheralOffsetsReader;

        [SerializeField] private Transform Head;
        [SerializeField] private Transform LeftHand;
        [SerializeField] private Transform RightHand;

        void OnEnable()
        {
            SetVRPeripheralModelTransform(Head, VRPeripheralOffsetsReader.Data.head);
            SetVRPeripheralModelTransform(LeftHand, VRPeripheralOffsetsReader.Data.leftController);
            SetVRPeripheralModelTransform(RightHand, VRPeripheralOffsetsReader.Data.rightController);

            VRPeripheralOffsetsReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        void OnDisable()
        {
            VRPeripheralOffsetsReader.ComponentUpdated.Remove(OnComponentUpdated);
        }

        private void SetVRPeripheralModelTransform(Transform vrElement, TransformOffset offset)
        {
            vrElement.localPosition = offset.position.ToUnityVector();
            vrElement.localRotation = Quaternion.Euler(offset.rotation.ToUnityVector());
        }

        void OnComponentUpdated(VRPeripheralOffsets.Update update)
        {
            if (update.head.HasValue)
            {
                SetVRPeripheralModelTransform(Head, update.head.Value);
            }
            if (update.leftController.HasValue)
            {
                SetVRPeripheralModelTransform(LeftHand, update.leftController.Value);
            }
            if (update.rightController.HasValue)
            {
                SetVRPeripheralModelTransform(RightHand, update.rightController.Value);
            }
        }
    }
}