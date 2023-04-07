using System.Collections.Generic;
using UnityEngine;
using Commands;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private Transform ParentObject;
    private ConditionBase conditionBase;
    private LoopBase loopBase;

    private void Start()
    {
        conditionBase = GetComponent<ConditionBase>();
        loopBase = GetComponent<LoopBase>();
    }

    public void CreateEndCommand(string path, Transform transformObject, Transform parentToReturnTo, GameObject placeholder, StructCommand Struct)
    {
        GameObject endObj;

        endObj = Instantiate(Resources.Load("Prefab/" + path), transformObject.position, transformObject.rotation) as GameObject;
        endObj.transform.SetParent(parentToReturnTo);
        placeholder.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex() + 1);
        endObj.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex() - 1);
        endObj.transform.localScale = new Vector3(1, 1, 1);
        endObj.transform.localPosition = Vector3.zero;

        if (Struct.Type == CommandsType.Condition)
        {
            Struct.ConditionId = conditionBase.ConditionNum;
            endObj.GetComponent<StructCommand>().ConditionId = conditionBase.ConditionNum;
            endObj.GetComponent<StructCommand>().endType = Struct.endType;
            conditionBase.ConditionNum++;
        }
        else
        {
            Struct.LoopId = loopBase.LoopNum;
            endObj.GetComponent<StructCommand>().LoopId = loopBase.LoopNum;
            endObj.GetComponent<StructCommand>().endType = Struct.endType;
            loopBase.LoopNum++;
        }
    }

    public void DestroyOldEndCommand(Transform parent, bool MoveCmd, int OldEndIfId, int OldEndLoopId)
    {
        if (MoveCmd)
        {
            for (int i = 0; i < parent.transform.childCount - 1; i++)
            {
                var OldEnd = parent.transform.GetChild(i).GetComponent<StructCommand>();

                if (OldEnd == null) continue;
                if ((OldEnd.Id == CommandsId.EndIf && OldEnd.ConditionId == OldEndIfId) || (OldEnd.Id == CommandsId.EndLoop && OldEnd.LoopId == OldEndLoopId))
                {
                    Destroy(parent.transform.GetChild(i).gameObject);
                    break;
                }
            }
        }
    }

    public void SetInnerCommand(Transform parent, StructCommand Struct, int index)
    {
        //Begin Command
        for (int x = index; x >= 0; x--)
        {
            if (x == 0)
            {
                Struct.InnerId = 0;
                break;
            }

            var cmd = parent.transform.GetChild(x - 1).GetComponent<StructCommand>();

            if (cmd == null) break;
            if (cmd.Id == CommandsId.EndIf || cmd.Id == CommandsId.EndLoop)
            {
                Struct.InnerId = cmd.InnerId;
                break;
            }
            else if (cmd.Id == CommandsId.If || cmd.Id == CommandsId.ElseIf || cmd.Id == CommandsId.Else || cmd.Id == CommandsId.For || cmd.Id == CommandsId.Forever)
            {
                Struct.InnerId = cmd.InnerId + 1;
                break;
            }
        }

        //End Commmand
        for (int x = index; x < parent.transform.childCount - 1; x++)
        {
            var cmd = parent.transform.GetChild(x).GetComponent<StructCommand>();

            if (cmd == null) break;
            if ((cmd.Id == CommandsId.EndIf && cmd.ConditionId == Struct.ConditionId) || (cmd.Id == CommandsId.EndLoop && cmd.LoopId == Struct.LoopId))
            {
                cmd.InnerId = Struct.InnerId;
                break;
            }
        }
    }

    public void TabCommand(Transform parent, GameObject cmdObject)
    {
        SetTab(cmdObject);

        //Check Other Command
        List<GameObject> ListCommand = new List<GameObject>();

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var cmd = parent.transform.GetChild(i);
            var StructCmd = cmd.GetComponent<StructCommand>();

            if (StructCmd == null) continue;
            else ListCommand.Add(cmd.gameObject);
        }

        for (int i = 0; i < ListCommand.Count; i++)
        {
            var cmd = ListCommand[i];
            var StructCmd = ListCommand[i].GetComponent<StructCommand>();

            if (StructCmd.InnerId != 0)
            {
                if (i - 1 < 0)
                {
                    StructCmd.InnerId = 0;
                    SetTab(cmd.gameObject);
                    continue;
                }

                var AfterCmd = ListCommand[i - 1];
                var AfterStructCmd = AfterCmd.GetComponent<StructCommand>();

                if (AfterStructCmd.Id == CommandsId.If || AfterStructCmd.Id == CommandsId.ElseIf || AfterStructCmd.Id == CommandsId.Else ||
                    AfterStructCmd.Id == CommandsId.For || AfterStructCmd.Id == CommandsId.Forever)
                {
                    if (StructCmd.Id != CommandsId.EndIf && StructCmd.Id != CommandsId.EndLoop)
                    {
                        StructCmd.InnerId = AfterStructCmd.InnerId + 1;
                    }
                    else if (StructCmd.Id == CommandsId.EndIf || StructCmd.Id == CommandsId.EndLoop)
                    {
                        SearchBlockCommand(i, ListCommand, StructCmd);
                    }
                }
                else if (AfterStructCmd.Id == CommandsId.EndIf || AfterStructCmd.Id == CommandsId.EndLoop)
                {
                    if (StructCmd.Id != CommandsId.EndIf && StructCmd.Id != CommandsId.EndLoop)
                    {
                        StructCmd.InnerId = AfterStructCmd.InnerId;
                    }
                    else SearchBlockCommand(i, ListCommand, StructCmd);
                }
                else
                {
                    if (StructCmd.Id == CommandsId.EndIf || StructCmd.Id == CommandsId.EndLoop)
                    {
                        SearchBlockCommand(i, ListCommand, StructCmd);
                    }
                    else StructCmd.InnerId = AfterStructCmd.InnerId;
                }

                SetTab(cmd.gameObject);
            }
        }

        //Set PreWidth Blank
        float PreWidthMax = 336;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var cmd = parent.transform.GetChild(i);
            var StructCmd = cmd.GetComponent<StructCommand>();
            var PreWidth = cmd.GetComponent<LayoutElement>().preferredWidth;

            if (StructCmd == null) continue;
            else if (PreWidth > PreWidthMax) PreWidthMax = PreWidth;
        }

        var obj = parent.transform.GetChild(parent.transform.childCount - 1);
        obj.GetComponent<LayoutElement>().preferredWidth = PreWidthMax;
    }

    public void SetTab(GameObject CmdObject)
    {
        var PreWidth = CmdObject.GetComponent<LayoutElement>();
        var StructCmd = CmdObject.GetComponent<StructCommand>();
        RectTransform Pos;
        var InitPreWidth = 0;
        var ChildParam = 0;

        if (StructCmd.Id == CommandsId.If || StructCmd.Id == CommandsId.ElseIf || StructCmd.Type == CommandsType.Transform
           || StructCmd.Type == CommandsType.Rotation)
        {
            InitPreWidth = 336;
            ChildParam = 1;
        }
        else if (StructCmd.Type == CommandsType.Idle) { InitPreWidth = 206; ChildParam = 1; }
        else if (StructCmd.Id == CommandsId.Forever) { InitPreWidth = 129; ChildParam = 1; }
        else if (StructCmd.Id == CommandsId.For) { InitPreWidth = 206; ChildParam = 1; }
        else if (StructCmd.Id == CommandsId.EndIf) { InitPreWidth = 129; ChildParam = 0; }
        else if (StructCmd.Id == CommandsId.EndLoop) { InitPreWidth = 129; ChildParam = 0; }
        else if (StructCmd.Id == CommandsId.Break) { InitPreWidth = 129; ChildParam = 1; }
        else if (StructCmd.Id == CommandsId.Interact) { InitPreWidth = 129; ChildParam = 1; }
        else if (StructCmd.Id == CommandsId.Else) { InitPreWidth = 129; ChildParam = 1; }

        Pos = CmdObject.transform.GetChild(ChildParam).GetComponent<RectTransform>();
        PreWidth.preferredWidth = InitPreWidth + 50 * StructCmd.InnerId;
        Pos.localPosition = new Vector2(25 * StructCmd.InnerId, Pos.localPosition.y);
    }

    public void SearchBlockCommand(int StartLoop, List<GameObject> CmdObject, StructCommand CurrentCmd)
    {
        for (int x = StartLoop; x >= 0; x--)
        {
            var BeginCmd = CmdObject[x];
            var BeginStructCmd = BeginCmd.GetComponent<StructCommand>();

            if (((BeginStructCmd.Id == CommandsId.If || BeginStructCmd.Id == CommandsId.ElseIf || BeginStructCmd.Id == CommandsId.Else) && CurrentCmd.ConditionId == BeginStructCmd.ConditionId) ||
               ((BeginStructCmd.Id == CommandsId.Forever || BeginStructCmd.Id == CommandsId.For) && CurrentCmd.LoopId == BeginStructCmd.LoopId))
            {
                CurrentCmd.InnerId = BeginStructCmd.InnerId;
                break;
            }
        }
    }

    public void SetLineNumber()
    {
        int linenum = 1;

        for (int i = 0; i < ParentObject.transform.childCount - 1; i++)
        {
            //Debug.Log("set"+ ParentObject.transform.GetChild(i));
            var cmd = ParentObject.transform.GetChild(i);
            var Struct = cmd.GetComponent<StructCommand>();
            
            if (Struct == null) continue;

            if (Struct.Type == CommandsType.Transform || Struct.Type == CommandsType.Rotation)
            {
                cmd.GetChild(1).GetChild(5).GetComponent<Text>().text = linenum.ToString();
            }
            else if (Struct.Type == CommandsType.Idle)
            {
                cmd.GetChild(1).GetChild(3).GetComponent<Text>().text = linenum.ToString();
            }
            else if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf)
            {
                cmd.GetChild(1).GetChild(4).GetComponent<Text>().text = linenum.ToString();
            }
            else if (Struct.Id == CommandsId.Forever || Struct.Id == CommandsId.Interact || Struct.Id == CommandsId.Break || Struct.Id == CommandsId.Else)
            {
                cmd.GetChild(1).GetChild(0).GetComponent<Text>().text = linenum.ToString();
            }
            else if (Struct.Id == CommandsId.For)
            {
                cmd.GetChild(1).GetChild(2).GetComponent<Text>().text = linenum.ToString();
            }
            else if(Struct.Id == CommandsId.EndIf || Struct.Id == CommandsId.EndLoop)
            {
                cmd.GetChild(0).GetChild(0).GetComponent<Text>().text = linenum.ToString();
            }
            linenum++;
        }
    }
}
