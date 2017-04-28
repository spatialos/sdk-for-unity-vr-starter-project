using Assets.Gamelogic.Utils;
using Improbable.General;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Global
{
    public class TransformSender : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;

        private void Update()
        {
            var transformUpdate = new WorldTransform.Update();
            var newPosition = transform.position.ToCoordinates();
            var newRotation = transform.rotation;

            var updatedTransform = false;
            if (PositionNeedsUpdate(newPosition))
            {
                transformUpdate.SetPosition(newPosition);
                updatedTransform = true;
            }
            if (RotationNeedsUpdate(newRotation))
            {
                transformUpdate.SetRotation(MathUtils.ToNativeQuaternion(transform.rotation));
                updatedTransform = true;
            }

            if (updatedTransform)
            {
                WorldTransformWriter.Send(transformUpdate);
            }
        }

        private bool PositionNeedsUpdate(Coordinates newPosition)
        {
            return !MathUtils.ApproximatelyEqual(newPosition, WorldTransformWriter.Data.position);
        }

        private bool RotationNeedsUpdate(Quaternion newRotation)
        {
            return !MathUtils.ApproximatelyEqual(newRotation, MathUtils.ToUnityQuaternion(WorldTransformWriter.Data.rotation));
        }
    }
}
