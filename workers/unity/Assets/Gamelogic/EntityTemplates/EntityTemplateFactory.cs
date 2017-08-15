using Assets.Gamelogic.Global;
using Improbable;
using Improbable.General;
using Improbable.Worker;
using Improbable.Global;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Unity.Entity;
using Improbable.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Assets.Gamelogic.Utils;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static Entity CreatePlayerCreatorTemplate()
        {
            var entityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.PlayerCreatorPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new PlayerCreation.Data(), CommonRequirementSets.PhysicsOnly)
                .Build();

            return entityTemplate;
        }

        public static Entity CreateVrPlayerTemplate(string clientId)
        {
            var entityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(Improbable.Coordinates.ZERO.ToUnityVector(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddMetadataComponent(entityType: SimulationSettings.VrPlayerPrefabName)
                .SetPersistence(false)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new VRPeripheralOffsets.Data(
                    head: new TransformOffset(Improbable.Vector3f.ZERO, Improbable.Vector3f.ZERO),
                    leftController: new TransformOffset(Improbable.Vector3f.ZERO, Improbable.Vector3f.ZERO),
                    rightController: new TransformOffset(Improbable.Vector3f.ZERO, Improbable.Vector3f.ZERO)
                ), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Grab.Data(new Map<ControllerSide, EntityId>()), CommonRequirementSets.SpecificClientOnly(clientId))
                .Build();

            return entityTemplate;
        }

        public static Entity CreateSpectatorPlayerTemplate(string clientId)
        {
            var entityTemplate = EntityBuilder.Begin()
                .AddPositionComponent((Improbable.Coordinates.ZERO + new Vector3d(0, 4, 0)).ToUnityVector(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddMetadataComponent(entityType: SimulationSettings.SpectatorPlayerPrefabName)
                .SetPersistence(false)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientAuthorityCheck.Data(), CommonRequirementSets.SpecificClientOnly(clientId))
                .AddComponent(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout), CommonRequirementSets.PhysicsOnly)
                .Build();

            return entityTemplate;
        }

        public static Entity CreateCubeEntityTemplate(Improbable.Coordinates spawnPosition)
        {
            var entityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(spawnPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.CubePrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.identity.ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Grabbable.Data(new Option<CurrentGrabberInfo>()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return entityTemplate;
        }

        public static Entity CreateArmchairEntityTemplate(Improbable.Coordinates spawnPosition, float rotation)
        {
            var entityTemplate = EntityBuilder.Begin()
                .AddPositionComponent(spawnPosition.ToUnityVector(), CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: SimulationSettings.ArmchairPrefabName)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(Quaternion.Euler(0, rotation, 0).ToNativeQuaternion()), CommonRequirementSets.PhysicsOnly)
                .AddComponent(new Grabbable.Data(new Option<CurrentGrabberInfo>()), CommonRequirementSets.PhysicsOnly)
                .Build();

            return entityTemplate;
        }
    }
}
