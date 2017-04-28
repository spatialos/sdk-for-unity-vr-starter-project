using System.Collections.Generic;
using System.IO;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor;
using Assets.Gamelogic.Global;
using Improbable.Math;

namespace Assets.Editor
{
    public class SnapshotMenu : MonoBehaviour
    {
        [MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
        [UsedImplicitly]
        private static void GenerateDefaultSnapshot()
        {
            var snapshotEntities = new Dictionary<EntityId, SnapshotEntity>();
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

        private static void SaveSnapshot(IDictionary<EntityId, SnapshotEntity> snapshotEntities)
        {
            File.Delete(SimulationSettings.DefaultSnapshotPath);
            var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

            if (maybeError.HasValue)
            {
                Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
            }
            else
            {
                Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
            }
        }
    }
}
