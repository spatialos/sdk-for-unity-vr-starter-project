using System;
using Assets.Gamelogic.Utils;
using Improbable.Unity.Core;
using UnityEngine;
using UnityEngine.UI;
using Assets.Gamelogic.Global;

namespace Assets.Gamelogic.UI
{
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField]
        private GameObject NotReadyWarning;
        [SerializeField]
        private Button[] ConnectButtons;

        private static SplashScreenController instance;
        private const string GameEntryGameObject = "GameEntry";

        private void Awake()
        {
            instance = this;
        }

        public void AttemptToConnectWithVr()
        {
            DisableConnectButtons();
            instance.AttemptVrConnection();
        }

        public void AttemptToConnectAsSpectator()
        {
            DisableConnectButtons();
            instance.AttemptSpectatorConnection();
        }

        private void EnableConnectButtons()
        {
            foreach (Button button in ConnectButtons)
            {
                button.interactable = true;
                button.GetComponent<CursorHoverEffect>().ShowButtonCursor();
            }
        }

        private void DisableConnectButtons()
        {
            foreach (Button button in ConnectButtons)
            {
                button.interactable = false;
                button.GetComponent<CursorHoverEffect>().ShowDefaultCursor();
            }
        }

        private void AttemptVrConnection()
        {
            GetBootstrapReference().AttemptToConnectVrClient();
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));
        }

        private void AttemptSpectatorConnection()
        {
            GetBootstrapReference().AttemptToConnectSpectatorClient();
            StartCoroutine(TimerUtils.WaitAndPerform(SimulationSettings.ClientConnectionTimeoutSecs, ConnectionTimeout));
        }

        private Bootstrap GetBootstrapReference()
        {
            if (!GameObject.Find(GameEntryGameObject).GetComponent<Bootstrap>())
            {
                throw new Exception("Couldn't find Bootstrap script on GameEntry in ClientScene");
            }
            return GameObject.Find(GameEntryGameObject).GetComponent<Bootstrap>();
        }

        private void ConnectionTimeout()
        {
            if (SpatialOS.IsConnected)
            {
                SpatialOS.Disconnect();
            }
            instance.NotReadyWarning.SetActive(true);
            EnableConnectButtons();
        }

        public static void HideSplashScreen()
        {
            if (instance != null)
            {
                instance.NotReadyWarning.SetActive(false);
                instance.gameObject.SetActive(false);
            }
        }
    }
}
