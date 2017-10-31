using System.Collections.Generic;
using System.IO;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using UnityEngine;
using UnityEditor;
using Assets.Gamelogic.Global;

namespace Assets.Editor
{
    public class SnapshotMenu : MonoBehaviour
    {
        [MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
        private static void GenerateDefaultSnapshot()
        {
            var snapshotEntities = new Dictionary<EntityId, Entity>();
            var currentEntityId = 1;

            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());

            // Test Cubes
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeEntityTemplate(new Coordinates(4, 1, 0)));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeEntityTemplate(new Coordinates(-4, 1, 0)));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeEntityTemplate(new Coordinates(0, 1, 4)));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeEntityTemplate(new Coordinates(0, 1, -4)));

            // Armchairs
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateArmchairEntityTemplate(new Coordinates(-2, 0, -2), 45));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateArmchairEntityTemplate(new Coordinates(-2, 0, 2), 135));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateArmchairEntityTemplate(new Coordinates(2, 0, 2), 225));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateArmchairEntityTemplate(new Coordinates(2, 0, -2), 315));

            SaveSnapshot(snapshotEntities);
        }

        private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
        {
            File.Delete(SimulationSettings.DefaultSnapshotPath);
            using (SnapshotOutputStream stream = new SnapshotOutputStream(SimulationSettings.DefaultSnapshotPath))
            {
                foreach (var kvp in snapshotEntities)
                {
                    var error = stream.WriteEntity(kvp.Key, kvp.Value);
                    if (error.HasValue)
                        {
                            Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", error.Value);
                            return;
                        }
                }
            }
                Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
        }
    }
}
