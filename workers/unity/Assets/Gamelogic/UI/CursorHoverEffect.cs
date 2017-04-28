using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class CursorHoverEffect : MonoBehaviour
    {
        [SerializeField]
        Texture2D HandCursor;

        public void ShowButtonCursor()
        {
            if (GetComponent<Button>().interactable)
            {
                ShowHandCursor();
            }
            else
            {
                ShowDefaultCursor();
            }
        }

        public void ShowHandCursor()
        {
            Cursor.SetCursor(HandCursor, new Vector2(HandCursor.width / 2, HandCursor.height / 2), CursorMode.ForceSoftware);
        }

        public void ShowDefaultCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
