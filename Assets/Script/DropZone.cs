using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Blank = null;
    public GameObject menuBtn;

    private void Start()
    {
        menuBtn = GameObject.Find("MenuButton");
        if (this.transform.parent.name == "Command Box Body") CreateBlank(this.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuBtn.GetComponent<MenuButton>().isPlay == false)
        {
            if (eventData.pointerDrag == null) return;
            Draggable drag = eventData.pointerDrag.GetComponent<Draggable>();
            if (drag != null) drag.placeholderParent = this.transform;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (menuBtn.GetComponent<MenuButton>().isPlay == false)
        {
            if (eventData.pointerDrag == null) return;
            Draggable drag = eventData.pointerDrag.GetComponent<Draggable>();
            if (drag != null && drag.placeholderParent == this.transform) drag.placeholderParent = drag.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (menuBtn.GetComponent<MenuButton>().isPlay == false)
        {
            Draggable drag = eventData.pointerDrag.GetComponent<Draggable>();
            if (drag != null)
            {
                drag.parentToReturnTo = this.transform;
                if (Blank != null) Destroy(Blank);
                if (this.transform.parent.name == "Command Box Body") CreateBlank(drag.parentToReturnTo);
            }
        }
    }

    private void CreateBlank(Transform parent)
    {
        Blank = new GameObject();
        Blank.gameObject.name = "Blank";
        Blank.AddComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Blank.GetComponent<RectTransform>().sizeDelta = new Vector2(330f, 54f);
        Blank.transform.SetParent(parent);
        Blank.transform.localScale = new Vector3(1, 1, 1);
        LayoutElement le = Blank.AddComponent<LayoutElement>();
        le.preferredWidth = 336;
        le.preferredHeight = 54;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;
    }
}
