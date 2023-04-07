using System.Collections.Generic;
using UnityEngine;
using Commands;

public class Compiler : MonoBehaviour
{
    [SerializeField] private GameObject CommandBox;
    [SerializeField] private BoxCollider SensorDistance;
    [SerializeField] private GameObject CommandAI;
    [SerializeField] private ErrorControl ErrorControl;
    [HideInInspector] public bool ErrorCompile = false;
    private GameObject Command;
    private List<StructCommand> ListCommand;
    public List<StructCommand> ListOptimize;
    public List<StructCommand> ListCommandAI;
    private StructCommand Struct;
    
    public void Compile()
    {
        ListCommand = new List<StructCommand>();
        ResetDestination();

        for (int i = 0; i < CommandBox.transform.childCount - 1; i++)
        {
            Command = CommandBox.transform.GetChild(i).gameObject;
            Struct = Command.GetComponent<StructCommand>();

            if (Struct.Type == CommandsType.Transform || Struct.Type == CommandsType.Rotation || Struct.Type == CommandsType.Idle)
            {
                var moveControl = Command.GetComponent<MovementController>();
                Struct.Sleep = moveControl.GetSleep();

                if (Struct.Id == CommandsId.Forward) Struct.Speed = -moveControl.GetSpeedMove();
                else if (Struct.Id == CommandsId.Backward) Struct.Speed = moveControl.GetSpeedMove();
                else if (Struct.Id == CommandsId.TurnLeft) Struct.Speed = moveControl.GetSpeedRotate();
                else if (Struct.Id == CommandsId.TurnRight) Struct.Speed = -moveControl.GetSpeedRotate();

                ListCommand.Add(Struct);
            }
            else if (Struct.Type == CommandsType.Condition)
            {
                var ConditionControl = Command.GetComponent<ConditionController>();

                if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf || Struct.Id == CommandsId.Else)
                {
                    if (Struct.Id == CommandsId.ElseIf || Struct.Id == CommandsId.Else)
                    {
                        if(i == 0)
                        {
                            ErrorCompile = true;
                            StartCoroutine(ErrorControl.ErrorDialog(Command));
                            return;
                        }
                        else if (ListCommand[ListCommand.Count - 1].Id != CommandsId.EndIf && ListCommand.Count > 0)
                        {
                            ErrorCompile = true;
                            StartCoroutine(ErrorControl.ErrorDialog(Command));
                            return;
                        }
                        else if (ListCommand[ListCommand.Count - 1].Id == CommandsId.EndIf && ListCommand[ListCommand.Count - 1].endType == EndType.Else && ListCommand.Count > 0)
                        {
                            ErrorCompile = true;
                            StartCoroutine(ErrorControl.ErrorDialog(Command));
                            return;
                        }
                    }

                    //Set Inner number
                    if(ListCommand.Count > 0)
                    {
                        //If
                        for (int x = i - 1; x >= 0; x--)
                        {
                            if (ListCommand[x].Id == CommandsId.EndIf)
                            {
                                Struct.InnerId = ListCommand[x].InnerId;
                                break;
                            }
                            else if (ListCommand[x].Id == CommandsId.If || ListCommand[x].Id == CommandsId.ElseIf || ListCommand[x].Id == CommandsId.Else)
                            {
                                Struct.InnerId = ListCommand[x].InnerId + 1;
                                break;
                            }
                        }

                        //EndIf
                        for(int x = i; x < CommandBox.transform.childCount; x++)
                        {
                            var cmd = CommandBox.transform.GetChild(x).GetComponent<StructCommand>();
                            if (cmd.Id == CommandsId.EndIf && cmd.ConditionId == Struct.ConditionId)
                            {
                                cmd.InnerId = Struct.InnerId;
                                break;
                            }
                        }
                    }

                    if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf)
                    {
                        Struct.Port = ConditionControl.GetPort();
                        ConditionControl.GetCondition(ref Struct.conditionType, ref Struct.conditionValue);
                        if (ConditionControl.Input.activeSelf == true) Struct.Range = ConditionControl.GetRange();
                    }
                    ListCommand.Add(Struct);
                }
                else if (Struct.Id == CommandsId.EndIf)
                {
                    for (int x = ListCommand.Count - 1; x >= 0; x--)
                    {
                        if ((ListCommand[x].Id == CommandsId.If || ListCommand[x].Id == CommandsId.ElseIf || ListCommand[x].Id == CommandsId.Else) 
                            && ListCommand[x].ConditionId == Struct.ConditionId)
                        {
                            ListCommand[x].Destination = ListCommand.Count;
                            ListCommand.Add(Struct);
                            break;
                        }
                    }
                }
            }
            else if (Struct.Type == CommandsType.Loop)
            {
                if (Struct.Id == CommandsId.Forever)
                {
                    Struct.Source = ListCommand.Count;
                    ListCommand.Add(Struct);
                }
                else if (Struct.Id == CommandsId.For)
                {
                    var LoopControl = Command.GetComponent<LoopController>();

                    Struct.Source = ListCommand.Count;
                    Struct.LoopCount = LoopControl.GetCount();
                    ListCommand.Add(Struct);
                }
                else if (Struct.Id == CommandsId.EndLoop)
                {
                    for (int x = ListCommand.Count - 1; x >= 0; x--)
                    {
                        if ((ListCommand[x].Id == CommandsId.Forever || ListCommand[x].Id == CommandsId.For) && ListCommand[x].LoopId == Struct.LoopId)
                        {
                            Struct.Destination = ListCommand[x].Source + 1;
                            Struct.LoopCount = ListCommand[x].LoopCount;
                            Struct.TmpLoopCount = Struct.LoopCount;
                            ListCommand[x].Destination = x + 1;
                            ListCommand.Add(Struct);
                            break;
                        }
                    }
                }
                else if (Struct.Id == CommandsId.Break)
                {
                    var isFound = false;

                    for (int x = ListCommand.Count - 1; x >= 0; x--)
                    {
                        if ((ListCommand[x].Id == CommandsId.Forever || ListCommand[x].Id == CommandsId.For))
                        {
                            ListCommand.Add(Struct);
                            isFound = true;
                            break;
                        }
                    }

                    if (isFound == false)
                    {
                        ErrorCompile = true;
                        StartCoroutine(ErrorControl.ErrorDialog(Command));
                        return;
                    }
                }
            }
            else if(Struct.Id == CommandsId.Interact) ListCommand.Add(Struct);
        }
        Optimize(ListCommand);
    }

    public void Optimize(List<StructCommand> ListCommand)
    {
        ListOptimize = new List<StructCommand>();

        for (int i = 0; i < ListCommand.Count; i++)
        {
            if (ListCommand[i].Type == CommandsType.Transform || ListCommand[i].Type == CommandsType.Rotation)
            {
                ListOptimize.Add(ListCommand[i]);
                for (int a = i + 1; a < ListCommand.Count; a++)
                {
                    if (ListCommand[a].Id == ListCommand[i].Id)
                    {
                        if (ListCommand[a].Speed == ListOptimize[ListOptimize.Count - 1].Speed)
                        {
                            ListOptimize[ListOptimize.Count - 1].Sleep += ListCommand[a].Sleep;
                            i = a;
                        }
                    }
                    else break;
                }
            }
            else if (ListCommand[i].Type == CommandsType.Idle)
            {
                ListOptimize.Add(ListCommand[i]);
                for (int a = i + 1; a < ListCommand.Count; a++)
                {
                    if (ListCommand[a].Id == ListCommand[i].Id)
                    {
                        ListOptimize[ListOptimize.Count - 1].Sleep += ListCommand[a].Sleep;
                        i = a;
                    }
                    else break;
                }
            }
            else if (ListCommand[i].Id == CommandsId.If || ListCommand[i].Id == CommandsId.ElseIf || ListCommand[i].Id == CommandsId.Else)
            {
                ListOptimize.Add(ListCommand[i]);
                ListOptimize[ListOptimize.Count - 1].Source = ListOptimize.Count;
            }
            else if (ListCommand[i].Id == CommandsId.EndIf)
            {
                bool haveDes = false;
                ListOptimize.Add(ListCommand[i]);

                for (int x = ListOptimize.Count - 2; x >= 0; x--)   //init x is EndIf
                {
                    if ((ListOptimize[x].Id == CommandsId.If || ListOptimize[x].Id == CommandsId.ElseIf || ListOptimize[x].Id == CommandsId.Else) 
                        && ListOptimize[x].ConditionId == ListCommand[i].ConditionId)
                    {
                        ListOptimize[x].EndIfDestination = ListOptimize.Count;  //EndIfDestination is next EndIf
                        for (int a = i + 1; a < ListCommand.Count; a++)         // init is command next Endif
                        {
                            if ((ListCommand[a].Id == CommandsId.ElseIf || ListCommand[a].Id == CommandsId.Else) && ListCommand[i].InnerId == ListCommand[a].InnerId)
                            {
                                ListOptimize[x].Destination = ListCommand[a].Destination + 1;                   //If or ElseIf
                                ListOptimize[ListOptimize.Count - 1].Destination = ListOptimize[x].Destination; //EndIf
                                haveDes = true;
                            }
                            else if ((ListCommand[a].Id == CommandsId.If || ListCommand[a].endType == EndType.If) && ListCommand[i].InnerId == ListCommand[a].InnerId) break;
                        }

                        if(haveDes == false)
                        {
                            ListOptimize[x].Destination++;                                                  //If or ElseIf
                            ListOptimize[ListOptimize.Count - 1].Destination = ListOptimize[x].Destination; //EndIf
                        }
                        break;
                    }
                }
            }
            else if (ListCommand[i].Id == CommandsId.Forever || ListCommand[i].Id == CommandsId.For)
            {
                for (int x = i + 1; x < ListCommand.Count; x++)
                {
                    if (ListCommand[x].Id == CommandsId.Forever || ListCommand[x].Id == CommandsId.For) x = ListCommand[x].Destination;
                    else if (ListCommand[x].Id == CommandsId.EndLoop) break;
                }
                ListOptimize.Add(ListCommand[i]);
            }
            else if (ListCommand[i].Id == CommandsId.EndLoop) ListOptimize.Add(ListCommand[i]);
            else if (ListCommand[i].Id == CommandsId.Break)
            {
                for (int x = i + 1; x < ListCommand.Count; x++)
                {
                    if (ListCommand[x].Id == CommandsId.EndLoop)
                    {
                        ListCommand[i].Destination = x + 1;
                        break;
                    }
                }
                ListOptimize.Add(ListCommand[i]);
            }
            else if (ListCommand[i].Id == CommandsId.Interact) ListOptimize.Add(ListCommand[i]);
        }

        FutureMovement();
        //menuButton.OnOffParameter(false);
    }

    private void FutureMovement()
    {
        for (int i = 0; i < ListOptimize.Count; i++)
        {
            if (ListOptimize[i].Type == CommandsType.Transform || ListOptimize[i].Type == CommandsType.Rotation || 
                ListOptimize[i].Type == CommandsType.Idle || ListOptimize[i].Type == CommandsType.Interact)
            {
                ListOptimize[i].ListCondition = new List<StructCommand>();

                if (i + 1 >= ListOptimize.Count) continue;
                if (ListOptimize[i + 1].Id == CommandsId.If) //Case have If after Movement 
                {
                    AddConditionList(ListOptimize[i].ListCondition, i, i + 1);
                }
                else if (ListOptimize[i + 1].Id == CommandsId.EndIf) //Case Movement before EndIf
                {
                    if (i + 2 >= ListOptimize.Count) continue;
                    if (ListOptimize[i + 2].Id == CommandsId.If)
                    {
                        AddConditionList(ListOptimize[i].ListCondition, i, i + 2);
                    }
                    else if(ListOptimize[i + 2].Id == CommandsId.ElseIf || ListOptimize[i + 2].Id == CommandsId.Else)
                    {
                        for (int x = i; x >= 0; x--)
                        {
                            if ((ListOptimize[x].Id == CommandsId.If || ListOptimize[x].Id == CommandsId.ElseIf || ListOptimize[x].Id == CommandsId.Else) 
                                && ListOptimize[i + 1].ConditionId == ListOptimize[x].ConditionId)
                            {
                                ListOptimize[i].Destination = ListOptimize[x].Destination;

                                if (ListOptimize[i].Destination >= ListOptimize.Count) continue;
                                if(ListOptimize[ListOptimize[i].Destination].Id == CommandsId.If)
                                {
                                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i].Destination);
                                }
                                else if (ListOptimize[ListOptimize[i].Destination].Id == CommandsId.EndLoop)
                                {
                                    var Des = ListOptimize[ListOptimize[ListOptimize[i].Destination].Destination];

                                    if (Des.Id == CommandsId.If)
                                    {
                                        AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[ListOptimize[i].Destination].Destination);
                                        break;
                                    }
                                    else
                                    {
                                        if (ListOptimize[ListOptimize[i].Destination].endType == EndType.Forever)
                                        {
                                            ListOptimize[i].Destination = ListOptimize[ListOptimize[i].Destination].Destination;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else if(ListOptimize[i + 2].Id == CommandsId.EndIf)
                    {
                        for(int x = i + 3; x < ListOptimize.Count; x++)
                        {
                            if (ListOptimize[x].Id == CommandsId.EndIf) continue;
                            else if (ListOptimize[x].Id == CommandsId.If)
                            {
                                AddConditionList(ListOptimize[i].ListCondition, i, x);
                                break;
                            }
                            else if (ListOptimize[x].Id == CommandsId.ElseIf || ListOptimize[x].Id == CommandsId.Else)
                            {
                                if (ListOptimize[x].Destination >= ListOptimize.Count)
                                {
                                    ListOptimize[i].Destination = ListOptimize[x].Destination;
                                    break;
                                }

                                var cmd = ListOptimize[ListOptimize[x].Destination];

                                if (cmd.Id == CommandsId.EndIf) continue;
                                else if (cmd.Id == CommandsId.If)
                                {
                                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[x].Destination);
                                    break;
                                }
                                else if (cmd.Id == CommandsId.Forever || cmd.Id == CommandsId.For)
                                {
                                    if(ListOptimize[cmd.Destination].Id == CommandsId.If)
                                    {
                                        AddConditionList(ListOptimize[i].ListCondition, i, cmd.Destination);
                                        break;
                                    }
                                    else
                                    {
                                        ListOptimize[i].Destination = cmd.Destination;
                                        break;
                                    }
                                }
                                else if(cmd.Id == CommandsId.EndLoop)
                                {
                                    var Des = ListOptimize[cmd.Destination];

                                    if (Des.Id == CommandsId.If)
                                    {
                                        AddConditionList(ListOptimize[i].ListCondition, i, cmd.Destination);
                                        break;
                                    }
                                    else
                                    {
                                        if (cmd.endType == EndType.Forever)
                                        {
                                            ListOptimize[i].Destination = cmd.Destination;
                                            break;
                                        }
                                        else
                                        {
                                            ListOptimize[i].Destination = ListOptimize[x].Destination;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    ListOptimize[i].Destination = ListOptimize[x].Destination;
                                    break;
                                }
                            }
                            else if (ListOptimize[x].Id == CommandsId.Forever || ListOptimize[x].Id == CommandsId.For)
                            {
                                if (ListOptimize[ListOptimize[x].Destination].Id == CommandsId.If)
                                {
                                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[x].Destination);
                                    break;
                                }
                                else
                                {
                                    ListOptimize[i].Destination = ListOptimize[x].Destination;
                                    break;
                                }
                            }
                            else if (ListOptimize[x].Id == CommandsId.EndLoop)
                            {
                                var Des = ListOptimize[ListOptimize[x].Destination];

                                if (Des.Id == CommandsId.If)
                                {
                                    if (ListOptimize[x].endType == EndType.Forever)
                                        AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[x].Destination);
                                    else ListOptimize[i].Destination = x;
                                    break;
                                }
                                else
                                {
                                    ListOptimize[i].Destination = ListOptimize[x].Destination;
                                }
                            }
                            else
                            {
                                ListOptimize[i].Destination = x;
                                break;
                            }
                        } 
                    }
                    else if(ListOptimize[i + 2].Id == CommandsId.EndLoop)
                    {
                        var Des = ListOptimize[ListOptimize[i + 2].Destination];

                        if (Des.Id == CommandsId.If)
                        {
                            if(ListOptimize[i + 2].endType == EndType.Forever)
                                AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i + 2].Destination);
                            else ListOptimize[i].Destination = i + 2;
                        }
                        else
                        {
                            if (ListOptimize[i + 2].endType == EndType.Forever)
                                ListOptimize[i].Destination = ListOptimize[i + 2].Destination;
                            else ListOptimize[i].Destination = i + 2;
                        }
                    }
                    else ListOptimize[i].Destination = i + 2;
                }
                else if (ListOptimize[i + 1].Id == CommandsId.Forever || ListOptimize[i + 1].Id == CommandsId.For)  //Case Movement before Loop
                {
                    if (i + 2 >= ListOptimize.Count) continue;
                    if (ListOptimize[i + 2].Id == CommandsId.If)
                    {
                        AddConditionList(ListOptimize[i].ListCondition, i, i + 2);
                    }
                    else if (ListOptimize[i + 2].Id == CommandsId.ElseIf || ListOptimize[i + 2].Id == CommandsId.Else)
                    {
                        for (int x = i; x >= 0; x--)
                        {
                            if ((ListOptimize[x].Id == CommandsId.If || ListOptimize[x].Id == CommandsId.ElseIf ) && ListOptimize[i + 1].ConditionId == ListOptimize[x].ConditionId)
                            {
                                ListOptimize[i].Destination = ListOptimize[x].Destination;

                                if (ListOptimize[i].Destination >= ListOptimize.Count) continue;
                                if (ListOptimize[ListOptimize[i].Destination].Id == CommandsId.If)
                                {
                                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i].Destination);
                                }
                                break;
                            }
                        }
                    }
                    else ListOptimize[i].Destination = i + 2;
                }
                else if (ListOptimize[i + 1].Id == CommandsId.EndLoop)  //Case Movement before EndLoop
                {
                    var Des = ListOptimize[ListOptimize[i + 1].Destination];

                    if (Des.Id == CommandsId.If)
                    {
                        if (ListOptimize[i + 1].endType == EndType.Forever)
                            AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i + 1].Destination);
                        else ListOptimize[i].Destination = i + 1;
                    }
                    else
                    {
                        if(ListOptimize[i + 1].endType == EndType.Forever)
                            ListOptimize[i].Destination = ListOptimize[i + 1].Destination;
                        else ListOptimize[i].Destination = i + 1;
                    }
                }
                else
                {
                    ListOptimize[i].Destination = i + 1;
                }
            }
            else if (ListOptimize[i].Id == CommandsId.Forever || ListOptimize[i].Id == CommandsId.For)
            {
                ListOptimize[i].ListCondition = new List<StructCommand>();

                if (ListOptimize[i + 1].Id == CommandsId.If) //Case have If after Loop 
                {
                    AddConditionList(ListOptimize[i].ListCondition, i, i + 1);
                }
            }
            else if (ListOptimize[i].Id == CommandsId.EndLoop)  
            {
                ListOptimize[i].ListCondition = new List<StructCommand>();
                
                var Des1 = ListOptimize[ListOptimize[i].Destination];

                if (Des1.Id == CommandsId.If)
                {
                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i].Destination);
                }

                if(ListOptimize[i].endType == EndType.For)
                {
                    ListOptimize[i].ListEndLoopNext = new List<StructCommand>();

                    if (i + 1 >= ListOptimize.Count) continue;
                    var Des2 = ListOptimize[i + 1];

                    if (Des2.Id == CommandsId.If)
                    {
                        AddConditionList(ListOptimize[i].ListEndLoopNext, i, i + 1);
                    }
                }
            }
            else if(ListOptimize[i].Id == CommandsId.Break)
            {
                ListOptimize[i].ListCondition = new List<StructCommand>();
                
                if (ListOptimize[i].Destination >= ListOptimize.Count) continue;
                var Des = ListOptimize[ListOptimize[i].Destination];

                if (Des.Id == CommandsId.If)    //Case destination Break is If
                {
                    AddConditionList(ListOptimize[i].ListCondition, i, ListOptimize[i].Destination);
                }
            }
        }
    }

    private void AddConditionList(List<StructCommand> conditionList, int index1, int index2)
    {
        conditionList.Add(ListOptimize[index2]);

        //Check Inner
        if(ListOptimize[index2 + 1].Id == CommandsId.If)
        {
            for (int i = index2 + 1; i < ListOptimize.Count; i++)
            {
                if (ListOptimize[i].Id == CommandsId.EndIf && ListOptimize[i].ConditionId == ListOptimize[index2].ConditionId) break;
                if (ListOptimize[i].Id == CommandsId.If || ListOptimize[i].Id == CommandsId.ElseIf || ListOptimize[i].Id == CommandsId.Else)
                {
                    conditionList.Add(ListOptimize[i]);
                }
            }
        }

        //Set Destination when don't have next If or ElseIf
        if (ListOptimize[index2].EndIfDestination >= ListOptimize.Count) return;

        var id = ListOptimize[ListOptimize[index2].EndIfDestination].Id;

        if (id != CommandsId.If && id != CommandsId.ElseIf && id != CommandsId.Else)
        {
            ListOptimize[index1].Destination = ListOptimize[index2].EndIfDestination;
        }
        else if (id == CommandsId.If || id == CommandsId.ElseIf || id == CommandsId.Else)
        {
            AddConditionList(conditionList, index1, ListOptimize[index2].EndIfDestination);
        }
    }

    public void CompileAI()
    {
        for (int i = 0; i < CommandAI.transform.childCount; i++)
        {
            ListCommandAI.Add(CommandAI.transform.GetChild(i).GetComponent<StructCommand>());
        }
    }

    private void ResetDestination()
    {
        for (int i = 0; i < CommandBox.transform.childCount - 1; i++)
        {
            Command = CommandBox.transform.GetChild(i).gameObject;
            Struct = Command.GetComponent<StructCommand>();

            Struct.Destination = 0;
            Struct.EndIfDestination = 0;
        }
    }
}
