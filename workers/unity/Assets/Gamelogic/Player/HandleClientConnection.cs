using Assets.Gamelogic.Global;
using Assets.Gamelogic.Utils;
using Improbable.Entity.Component;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Core;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HandleClientConnection : MonoBehaviour
    {
        [Require]
        private ClientConnection.Writer ClientConnectionWriter;

        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            ClientConnectionWriter.CommandReceiver.OnDisconnectClient.RegisterAsyncResponse(OnDisconnectClient);
            ClientConnectionWriter.CommandReceiver.OnHeartbeat.RegisterResponse(OnHeartbeat);
            heartbeatCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.HeartbeatCheckIntervalSecs, CheckHeartbeat));
        }

        private void OnDisable()
        {
            ClientConnectionWriter.CommandReceiver.OnHeartbeat.DeregisterResponse();
            StopCoroutine(heartbeatCoroutine);
        }

        private void OnDisconnectClient(ResponseHandle<ClientConnection.Commands.DisconnectClient,
                                        ClientDisconnectRequest,
                                        ClientDisconnectResponse> handle)
        {
            DeletePlayerEntity();
        }

        private HeartbeatResponse OnHeartbeat(HeartbeatRequest request, ICommandCallerInfo callerinfo)
        {
            SetHeartbeat(SimulationSettings.TotalHeartbeatsBeforeTimeout);
            return new HeartbeatResponse();
        }

        private void SetHeartbeat(uint beats)
        {
            var update = new ClientConnection.Update();
            update.SetTimeoutBeatsRemaining(beats);
            ClientConnectionWriter.Send(update);
        }

        private void CheckHeartbeat()
        {
            var heartbeatsRemainingBeforeTimeout = ClientConnectionWriter.Data.timeoutBeatsRemaining;
            if (heartbeatsRemainingBeforeTimeout == 0)
            {
                StopCoroutine(heartbeatCoroutine);
                DeletePlayerEntity();
                return;
            }
            SetHeartbeat(heartbeatsRemainingBeforeTimeout - 1);
        }

        private void DeletePlayerEntity()
        {
            if (GetComponent<PlayerDisconnectCleanup>())
            {
                GetComponent<PlayerDisconnectCleanup>().CleanUpPlayer();
            }
            SpatialOS.Commands.DeleteEntity(ClientConnectionWriter, gameObject.EntityId());
        }
    }
}
