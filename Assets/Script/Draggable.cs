using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Commands;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject MiniCommand;
    [SerializeField] private GameObject FullCommand;
    [HideInInspector] public Transform parentToReturnTo = null;
    [HideInInspector] public Transform placeholderParent = null;
    [HideInInspector] public GameObject placeholder = null;
    private CommandManager CmdManager;
    private GameObject CloneCommand = null;
    private GameObject menuBtn;
    private StructCommand Struct;
    private bool MoveCmd = false;
    private int OldEndIfId, OldEndLoopId;
    private SoundControl PlaySound;

    public void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        menuBtn = GameObject.Find("MenuButton");
        CmdManager = GameObject.Find("GameControl").GetComponent<CommandManager>();
        Struct = GetComponent<StructCommand>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PlaySound.PlayClick();
        var btn = menuBtn.GetComponent<MenuButton>();
        if (btn.isPlay == false && btn.isCompile == false && btn.isClick == false)
        {
            var parentName = this.transform.parent.name;
            if (parentName == "List Box")
            {
                CloneCommand = Instantiate(Resources.Load("Prefab/" + transform.name), transform.position, transform.rotation) as GameObject;
                CloneCommand.transform.name = transform.name;
                CloneCommand.transform.SetParent(transform.parent);
                CloneCommand.transform.SetSiblingIndex(transform.GetSiblingIndex());
                CloneCommand.transform.localScale = new Vector3(1, 1, 1);

                MiniCommand.SetActive(false);
                FullCommand.SetActive(true);
                FullCommand.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (parentName == "Command Box")
            {
                transform.SetParent(GameObject.Find("Canvas/Command Box Parent/Command Box Body").transform);
                MoveCmd = true;
            }

            placeholder = new GameObject();
            placeholder.transform.SetParent(transform.parent);
            LayoutElement le = placeholder.AddComponent<LayoutElement>();
            le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
            le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

            parentToReturnTo = transform.parent;
            placeholderParent = parentToReturnTo;
            transform.SetParent(transform.parent.parent);

            btn.isDrop = false;

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var btn = menuBtn.GetComponent<MenuButton>();
        if (btn.isPlay == false && btn.isCompile == false && btn.isClick == false)
        {
            transform.position = eventData.position;

            if (placeholder.transform.parent.name == "List Box" || placeholder.transform.parent.name == "BG") placeholder.SetActive(false);
            else placeholder.SetActive(true);

            if (placeholder.transform.parent != placeholderParent) placeholder.transform.SetParent(placeholderParent);

            int newSiblingIndex = placeholderParent.childCount;

            for (int i = 0; i < placeholderParent.childCount; i++)
            {
                if (transform.position.y > placeholderParent.GetChild(i).position.y)
                {
                    newSiblingIndex = i;

                    if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) newSiblingIndex--;
                    break;
                }
            }
            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var btn = menuBtn.GetComponent<MenuButton>();
        if (btn.isPlay == false && btn.isCompile == false && btn.isClick == false)
        {
            transform.SetParent(parentToReturnTo);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            Destroy(placeholder);

            var parent = this.transform.parent;

            if (parent.name != "Command Box")
            {
                btn.isDrop = true;
                //Remove End Command
                var Struct = gameObject.GetComponent<StructCommand>();
                var CmdBox = GameObject.Find("Canvas/Command Box Parent/Command Box Body/Command Box").gameObject;

                if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf || Struct.Id == CommandsId.Else || Struct.Id == CommandsId.For || Struct.Id == CommandsId.Forever)
                {
                    if(CmdBox.transform.childCount > 1)
                    {
                        for (int i = 0; i < CmdBox.transform.childCount - 1; i++)
                        {
                            var cmd = CmdBox.transform.GetChild(i);
                            var StructCmd = cmd.GetComponent<StructCommand>();

                            if (StructCmd == null) continue;
                            if ((StructCmd.Id == CommandsId.EndIf && StructCmd.ConditionId == Struct.ConditionId) ||
                                (StructCmd.Id == CommandsId.EndLoop && StructCmd.LoopId == Struct.LoopId))
                            {
                                DestroyImmediate(cmd.gameObject);
                                break;
                            }
                        }
                    }
                }
                else if (Struct.Id == CommandsId.EndIf || Struct.Id == CommandsId.EndLoop)
                {
                    for (int i = 0; i < CmdBox.transform.childCount - 1; i++)
                    {
                        var cmd = CmdBox.transform.GetChild(i);
                        var StructCmd = cmd.GetComponent<StructCommand>();

                        if (StructCmd == null) continue;
                        if (((StructCmd.Id == CommandsId.If || StructCmd.Id == CommandsId.ElseIf || Struct.Id == CommandsId.Else) && StructCmd.ConditionId == Struct.ConditionId) ||
                            ((Struct.Id == CommandsId.For || Struct.Id == CommandsId.Forever) && StructCmd.LoopId == Struct.LoopId))
                        {
                            Destroy(cmd.gameObject);
                            break;
                        }
                    }
                }

                /*Destroy(gameObject);
                Destroy(placeholder);
                CmdManager.SetLineNumber();*/

                //Update Tab Command
                if (MoveCmd == false)
                {
                    Destroy(gameObject);
                    Destroy(placeholder);
                    return;
                }
                else
                {
                    CmdManager.TabCommand(CmdBox.transform, gameObject);
                    Destroy(gameObject);
                    Destroy(placeholder);
                    CmdManager.SetLineNumber();
                    MoveCmd = false;
                    return;
                }
            }

            transform.localScale = new Vector3(1, 1, 1);
            
            if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf || Struct.Id == CommandsId.Else)
            {
                OldEndIfId = Struct.ConditionId;
                CmdManager.CreateEndCommand("EndIFMini", transform, parentToReturnTo, placeholder, Struct);
            }
            else if (Struct.Id == CommandsId.Forever || Struct.Id == CommandsId.For)
            {
                OldEndLoopId = Struct.LoopId;
                CmdManager.CreateEndCommand("EndLoopMini", transform, parentToReturnTo, placeholder, Struct);
            }

            CmdManager.DestroyOldEndCommand(parent, MoveCmd, OldEndIfId, OldEndLoopId);
            CmdManager.SetInnerCommand(parent, Struct, transform.GetSiblingIndex());
            CmdManager.TabCommand(parent, gameObject);
            CmdManager.SetLineNumber();

            MoveCmd = false;
            btn.isDrop = true;
        }
    }
}
