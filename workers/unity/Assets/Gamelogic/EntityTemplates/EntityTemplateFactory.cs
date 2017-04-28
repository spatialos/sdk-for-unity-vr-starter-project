using Assets.Gamelogic.Global;
using Improbable;
using Improbable.General;
using Improbable.Worker;
using Improbable.Global;
using Improbable.Math;
using Improbable.Player;
using Improbable.Unity.Core.Acls;
using Improbable.Collections;
using UnityEngine;
using Quaternion = Improbable.Global.Quaternion;
using Assets.Gamelogic.Utils;

namespace Assets.Gamelogic.EntityTemplates
{
    public class EntityTemplateFactory : MonoBehaviour
    {
        public static SnapshotEntity CreatePlayerCreatorTemplate()
        {
            var entityTemplate = new SnapshotEntity { Prefab = SimulationSettings.PlayerCreatorPrefabName };

            entityTemplate.Add(new WorldTransform.Data(Coordinates.ZERO, new Quaternion(0,0,0,0)));
            entityTemplate.Add(new PlayerCreation.Data());

            var acl = Acl.GenerateServerAuthoritativeAcl(entityTemplate);
            entityTemplate.SetAcl(acl);

            return entityTemplate;
        }

        public static Entity CreateVrPlayerTemplate(string clientId)
        {
            var entityTemplate = new SnapshotEntity { Prefab = SimulationSettings.VrPlayerPrefabName };

            entityTemplate.Add(new WorldTransform.Data(Coordinates.ZERO, new Quaternion(0, 0, 0, 0)));
            entityTemplate.Add(new VRPeripheralOffsets.Data(new TransformOffset(Vector3f.ZERO, Vector3f.ZERO), new TransformOffset(Vector3f.ZERO, Vector3f.ZERO), new TransformOffset(Vector3f.ZERO, Vector3f.ZERO)));
            entityTemplate.Add(new ClientAuthorityCheck.Data());
            entityTemplate.Add(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout));
            entityTemplate.Add(new Grab.Data(new Map<ControllerSide, EntityId>()));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<WorldTransform>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<VRPeripheralOffsets>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<ClientAuthorityCheck>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<ClientConnection>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Grab>(CommonRequirementSets.SpecificClientOnly(clientId));
            entityTemplate.SetAcl(acl);

            return entityTemplate;
        }

        public static Entity CreateSpecatorPlayerTemplate(string clientId)
        {
            var entityTemplate = new SnapshotEntity { Prefab = SimulationSettings.SpectatorPlayerPrefabName };

            entityTemplate.Add(new WorldTransform.Data(Coordinates.ZERO + new Vector3d(0,4,0), new Quaternion(0, 0, 0, 0)));
            entityTemplate.Add(new ClientAuthorityCheck.Data());
            entityTemplate.Add(new ClientConnection.Data(SimulationSettings.TotalHeartbeatsBeforeTimeout));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<WorldTransform>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<ClientAuthorityCheck>(CommonRequirementSets.SpecificClientOnly(clientId))
                .SetWriteAccess<ClientConnection>(CommonRequirementSets.PhysicsOnly);
            entityTemplate.SetAcl(acl);

            return entityTemplate;
        }

        public static SnapshotEntity CreateCubeEntityTemplate(Coordinates spawnPosition)
        {
            var entityTemplate = new SnapshotEntity { Prefab = SimulationSettings.CubePrefabName };

            entityTemplate.Add(new WorldTransform.Data(spawnPosition, new Quaternion(0, 0, 0, 0)));
            entityTemplate.Add(new Grabbable.Data(new Option<CurrentGrabberInfo>()));

            var acl = Acl.GenerateServerAuthoritativeAcl(entityTemplate);
            entityTemplate.SetAcl(acl);

            return entityTemplate;
        }

        public static SnapshotEntity CreateArmchairEntityTemplate(Coordinates spawnPosition, float rotation)
        {
            var entityTemplate = new SnapshotEntity { Prefab = SimulationSettings.ArmchairPrefabName };

            entityTemplate.Add(new WorldTransform.Data(spawnPosition, MathUtils.ToNativeQuaternion(UnityEngine.Quaternion.Euler(0, rotation, 0))));
            entityTemplate.Add(new Grabbable.Data(new Option<CurrentGrabberInfo>()));

            var acl = Acl.GenerateServerAuthoritativeAcl(entityTemplate);
            entityTemplate.SetAcl(acl);

            return entityTemplate;
        }
    }
}
