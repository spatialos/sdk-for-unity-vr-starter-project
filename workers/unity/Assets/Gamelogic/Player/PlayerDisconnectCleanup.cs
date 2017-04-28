using System.Collections.Generic;
using Assets.Gamelogic.Grabbing;
using Improbable;
using Improbable.Player;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class PlayerDisconnectCleanup : MonoBehaviour
    {
        [Require]
        private ClientConnection.Writer ClientConnectionWriter;
        [Require]
        private Grab.Reader GrabbingReader;

        public void CleanUpPlayer()
        {
            foreach (KeyValuePair<ControllerSide, EntityId> heldEntity in GrabbingReader.Data.heldEntities)
            {
                SpatialOS.Universe.Get(heldEntity.Value).UnderlyingGameObject.GetComponent<GrabbableRequestHandler>().HandleDropRequest();
            }
        }
    }
}
