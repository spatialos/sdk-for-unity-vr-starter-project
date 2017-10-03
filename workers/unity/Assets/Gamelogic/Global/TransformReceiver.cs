using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.General;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Worker;

namespace Assets.Gamelogic.Global
{
    public class TransformReceiver : MonoBehaviour
    {

        [Require] private Position.Reader PositionReader;
        [Require] private Rotation.Reader RotationReader;

        private void OnEnable()
        {
            transform.position = PositionReader.Data.coords.ToUnityVector();
            transform.rotation = RotationReader.Data.rotation.ToUnityQuaternion();

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
            if (PositionReader.Authority == Authority.NotAuthoritative && update.coords.HasValue)
            {
                transform.position = update.coords.Value.ToUnityVector();
            }
        }

        private void OnRotationUpdate(Rotation.Update update)
        {
            if (RotationReader.Authority == Authority.NotAuthoritative && update.rotation.HasValue)
            {
                transform.rotation = update.rotation.Value.ToUnityQuaternion();
            }
        }
    }
}