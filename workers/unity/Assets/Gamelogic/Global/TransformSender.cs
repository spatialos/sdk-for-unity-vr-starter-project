using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.General;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Global
{
    public class TransformSender : MonoBehaviour
    {
        [Require] private Position.Writer PositionWriter;
        [Require] private Rotation.Writer RotationWriter;

        private void Update()
        {
            var newCoords = transform.position.ToCoordinates();
            if (PositionNeedsUpdate(newCoords)) {
                PositionWriter.Send(new Position.Update().SetCoords(newCoords));
            }

            var newRotation = transform.rotation;
            if (RotationNeedsUpdate(newRotation))
            {
                RotationWriter.Send(new Rotation.Update().SetRotation(MathUtils.ToSpatialQuaternion(transform.rotation)));
            }
        }

        private bool PositionNeedsUpdate(Coordinates newCoords)
        {
            return !MathUtils.ApproximatelyEqual(newCoords, PositionWriter.Data.coords);
        }
        private bool RotationNeedsUpdate(Quaternion newRotation)
        {
            return !MathUtils.ApproximatelyEqual(newRotation, MathUtils.ToUnityQuaternion(RotationWriter.Data.rotation));
        }
    }
}
