using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string message;
    private Texture2D texture;
    private bool isOver = false;
    private bool isShow = false;
    private float mousePosX, mousePosY;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        StartCoroutine(Delay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        isShow = false;
    }
    void OnGUI()
    {
        if (isShow)
        {
            GUIStyle Style = new GUIStyle("box");

            Style.fontSize = 20;
            Style.normal.textColor = Color.black;

            var contentSize = Style.CalcSize(new GUIContent(message));
            texture = new Texture2D((int)contentSize.x, (int)contentSize.y);
            Style.normal.background = texture;

            GUI.Box(new Rect(mousePosX, mousePosY + 22, contentSize.x, contentSize.y), message, Style);
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.3f);

        if (isOver)
        {
            isShow = true;
            mousePosX = Input.mousePosition.x;
            mousePosY = Screen.height - Input.mousePosition.y;
        }
    }
}
