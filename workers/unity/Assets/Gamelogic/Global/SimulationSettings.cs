using UnityEngine;

namespace Assets.Gamelogic.Global
{
    public static class SimulationSettings
    {
        public static readonly string VrPlayerPrefabName = "Player";
        public static readonly string SpectatorPlayerPrefabName = "Spectator";
        public static readonly string PlayerCreatorPrefabName = "PlayerCreator";
        public static readonly string CubePrefabName = "Cube";
        public static readonly string ArmchairPrefabName = "Armchair";

        public static readonly float ClientConnectionTimeoutSecs = 7;
        public static readonly float HeartbeatCheckIntervalSecs = 3;
        public static readonly uint TotalHeartbeatsBeforeTimeout = 3;
        public static readonly float HeartbeatSendingIntervalSecs = 3;

        public static readonly int TargetClientFramerate = 60;
        public static readonly int TargetServerFramerate = 60;
        public static readonly int FixedFramerate = 20;

        public static readonly float PlayerCreatorQueryRetrySecs = 4;
        public static readonly float PlayerEntityCreationRetrySecs = 4;

        public static readonly string DefaultSnapshotPath = Application.dataPath + "/../../../snapshots/default.snapshot";

        public static readonly string TerrainLayerName = "Terrain";

        public static readonly string PlayerTag = "Player";
        public static readonly string GrabbableEntityTag = "Grabbable";

        public static readonly float MaxTeleportDistance = 40f;
        public static readonly float TeleportTargeterDiameter = 2f;
        public static readonly float TeleportTargeterRotationSpeed = 80f;

        public static readonly float PlayerHandMass = 0.01f;
        public static readonly float PlayerHandDrag = 0f;
        public static readonly float PlayerHandAngularDrag = 0.05f;
    }
}
