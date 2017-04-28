using Assets.Gamelogic.UI;
using Improbable;
using Improbable.Global;
using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using UnityEngine;

namespace Assets.Gamelogic.Global
{
    public class Bootstrap : MonoBehaviour
    {
        public WorkerConfigurationData Configuration = new WorkerConfigurationData();

        private static PlayerType playerCreationType;

        public void Start()
        {
            SpatialOS.ApplyConfiguration(Configuration);

            Time.fixedDeltaTime = 1.0f / SimulationSettings.FixedFramerate;

            switch (SpatialOS.Configuration.WorkerPlatform)
            {
                case WorkerPlatform.UnityWorker:
                    Application.targetFrameRate = SimulationSettings.TargetServerFramerate;
                    SpatialOS.OnDisconnected += reason => Application.Quit();
                    SpatialOS.Connect(gameObject);
                    break;
                case WorkerPlatform.UnityClient:
                    Application.targetFrameRate = SimulationSettings.TargetClientFramerate;
                    SpatialOS.OnConnected += CreatePlayer;
                    break;
            }
        }

        public void AttemptToConnectVrClient()
        {
            playerCreationType = PlayerType.VRPLAYER;
            SpatialOS.Connect(gameObject);
        }

        public void AttemptToConnectSpectatorClient()
        {
            playerCreationType = PlayerType.SPECTATORPLAYER;
            SpatialOS.Connect(gameObject);
        }

        public static void CreatePlayer()
        {
            var playerCreatorQuery = Query.HasComponent<PlayerCreation>().ReturnOnlyEntityIds();
            SpatialOS.WorkerCommands.SendQuery(playerCreatorQuery)
                .OnSuccess(OnSuccessfulPlayerCreatorQuery)
                .OnFailure(OnFailedPlayerCreatorQuery);
        }

        private static void OnSuccessfulPlayerCreatorQuery(EntityQueryResult queryResult)
        {
            if (queryResult.EntityCount < 1)
            {
                Debug.LogError("Failed to find PlayerCreator. SpatialOS probably hadn't finished loading the initial snapshot. Try again in a few seconds.");
                return;
            }

            var playerCreatorEntityId = queryResult.Entities.First.Value.Key;
            RequestPlayerCreation(playerCreatorEntityId);
        }

        private static void OnFailedPlayerCreatorQuery(ICommandErrorDetails _)
        {
            Debug.LogError("PlayerCreator query failed. SpatialOS workers probably haven't started yet. Try again in a few seconds.");
        }

        private static void RequestPlayerCreation(EntityId playerCreatorEntityId)
        {
            SpatialOS.WorkerCommands.SendCommand(PlayerCreation.Commands.CreatePlayer.Descriptor, new CreatePlayerRequest(playerCreationType), playerCreatorEntityId)
                .OnSuccess(HideSplashScreen)
                .OnFailure(response => OnCreatePlayerFailure(response, playerCreatorEntityId));
        }

        private static void HideSplashScreen(CreatePlayerResponse _)
        {
            SplashScreenController.HideSplashScreen();
        }

        private static void OnCreatePlayerFailure(ICommandErrorDetails _, EntityId playerCreatorEntityId)
        {
            Debug.LogWarning("CreatePlayer command failed - you probably tried to connect too soon. Try again in a few seconds.");
        }
    }
}