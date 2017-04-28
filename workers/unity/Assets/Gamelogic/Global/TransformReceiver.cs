using Assets.Gamelogic.Utils;
using Improbable.General;
using UnityEngine;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Global
{
    public class TransformReceiver : MonoBehaviour
    {
        [Require]
        private WorldTransform.Reader WorldTransformReader;

        private void OnEnable()
        {
            transform.rotation = MathUtils.ToUnityQuaternion(WorldTransformReader.Data.rotation);
            transform.position = WorldTransformReader.Data.position.ToVector3();

            WorldTransformReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        private void OnDisable()
        {
            WorldTransformReader.ComponentUpdated.Remove(OnComponentUpdated);
        }

        private void OnComponentUpdated(WorldTransform.Update update)
        {
            if (!WorldTransformReader.HasAuthority)
            {
                if (update.position.HasValue)
                {
                    transform.position = update.position.Value.ToVector3();
                }
                if (update.rotation.HasValue)
                {
                    var rotation = update.rotation.Value;
                    transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
                }
            }
        }
    }
}