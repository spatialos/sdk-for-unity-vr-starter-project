using Assets.Gamelogic.Global;
using UnityEngine;
using Assets.Gamelogic.Utils;
using Improbable.Global;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Player
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SendClientConnection : MonoBehaviour
    {
        [Require]
        private ClientAuthorityCheck.Writer ClientAuthorityCheckWriter;

        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            heartbeatCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.HeartbeatSendingIntervalSecs, SendHeartbeat));
        }

        private void OnDisable()
        {
            StopCoroutine(heartbeatCoroutine);
        }

        private void SendHeartbeat()
        {
            SpatialOS.Commands.SendCommand(ClientAuthorityCheckWriter, ClientConnection.Commands.Heartbeat.Descriptor, new HeartbeatRequest(), gameObject.EntityId());
        }

        private void OnApplicationQuit()
        {
            if (SpatialOS.IsConnected)
            {
                SpatialOS.Commands.SendCommand(ClientAuthorityCheckWriter, ClientConnection.Commands.DisconnectClient.Descriptor, new ClientDisconnectRequest(), gameObject.EntityId());
            }
        }
    }
}

