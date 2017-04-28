using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Global;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Global
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerCreatingBehaviour : MonoBehaviour
    {
        [Require]
        private PlayerCreation.Writer PlayerCreationWriter;

        private void OnEnable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterResponse(OnCreatePlayer);
        }

        private CreatePlayerResponse OnCreatePlayer(CreatePlayerRequest request, ICommandCallerInfo callerinfo)
        {
            CreatePlayerWithReservedId(callerinfo.CallerWorkerId, request.playerType);
            return new CreatePlayerResponse();
        }

        private void CreatePlayerWithReservedId(string clientWorkerId, PlayerType playerType)
        {
            SpatialOS.Commands.ReserveEntityId(PlayerCreationWriter)
                .OnSuccess(reservedEntityId => CreatePlayer(clientWorkerId, reservedEntityId, playerType))
                .OnFailure(failure => OnFailedReservation(failure, clientWorkerId, playerType));
        }

        private void OnDisable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
        }

        private void OnFailedReservation(ICommandErrorDetails response, string clientWorkerId, PlayerType playerType)
        {
            Debug.LogError("Failed to Reserve EntityId for Player: " + response.ErrorMessage + ". Retrying...");
            CreatePlayerWithReservedId(clientWorkerId, playerType);
        }

        private void CreatePlayer(string clientWorkerId, EntityId entityId, PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.VRPLAYER:
                    CreateVrPlayer(clientWorkerId, entityId);
                    break;
                case PlayerType.SPECTATORPLAYER:
                    CreateSpectatorPlayer(clientWorkerId, entityId);
                    break;
            }
        }

        private void CreateVrPlayer(string clientWorkerId, EntityId entityId)
        {
            var vrPlayerEntityTemplate = EntityTemplateFactory.CreateVrPlayerTemplate(clientWorkerId);
            SpatialOS.Commands.CreateEntity(PlayerCreationWriter, entityId, SimulationSettings.VrPlayerPrefabName, vrPlayerEntityTemplate)
                .OnFailure(failure => OnFailedPlayerCreation(failure, clientWorkerId, entityId, PlayerType.VRPLAYER));
        }

        private void CreateSpectatorPlayer(string clientWorkerId, EntityId entityId)
        {
            var spectatorPlayerEntityTemplate = EntityTemplateFactory.CreateSpecatorPlayerTemplate(clientWorkerId);
            SpatialOS.Commands.CreateEntity(PlayerCreationWriter, entityId, SimulationSettings.SpectatorPlayerPrefabName, spectatorPlayerEntityTemplate)
                .OnFailure(failure => OnFailedPlayerCreation(failure, clientWorkerId, entityId, PlayerType.SPECTATORPLAYER));
        }

        private void OnFailedPlayerCreation(ICommandErrorDetails failure, string clientWorkerId, EntityId entityId, PlayerType playerType)
        {
            Debug.LogError("Failed to Create Player Entity: " + failure.ErrorMessage + ". Retrying...");
            CreatePlayer(clientWorkerId, entityId, playerType);
        }
    }
}
