using UnityEngine;

public class SceneCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {

        Vector2 centerHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, centerHotspot, CursorMode.Auto);
    }
}
