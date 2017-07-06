using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.General;
using UnityEngine;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Global
{
    public class TransformReceiver : MonoBehaviour
    {

        [Require] private Position.Reader PositionReader;
        [Require] private Rotation.Reader RotationReader;

        private void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = MathUtils.ToUnityQuaternion(RotationReader.Data.rotation);

            PositionReader.ComponentUpdated.Add(OnPositionUpdate);
            RotationReader.ComponentUpdated.Add(OnRotationUpdate);
        }

        private void OnDisable()
        {
            PositionReader.ComponentUpdated.Remove(OnPositionUpdate);
            RotationReader.ComponentUpdated.Remove(OnRotationUpdate);
        }
        private void OnPositionUpdate(Position.Update update)
        {
            if (!PositionReader.HasAuthority && update.coords.HasValue)
            {
                transform.position = update.coords.Value.ToUnityVector();
            }
        }

        private void OnRotationUpdate(Rotation.Update update)
        {
            if (!RotationReader.HasAuthority && update.rotation.HasValue)
            {
                transform.rotation = MathUtils.ToUnityQuaternion(update.rotation.Value);
            }
        }
    }
}