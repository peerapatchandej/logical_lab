using System.Collections.Generic;
using UnityEngine;
using Commands;

public class CommandsController : MonoBehaviour
{
    [SerializeField] private MenuButton MenuButton;
    [SerializeField] private Compiler Compile;
    [SerializeField] private GameObject Robot;
    [SerializeField] private GameObject Blade;
    [SerializeField] private ConditionBase conditionBase;
    [SerializeField] private CheckSensor[] CheckSensor;
    [SerializeField] private CheckSensorDistance CheckSensorDistance;
    [SerializeField] private OnLoadLevel LoadLevel;
    [HideInInspector] public float InteractTime = 1f;
    [HideInInspector] public float TimeSleep = 0f;
    private RunCommand RunCmd;
    private Rigidbody rgbd;

    void Start ()
    {
        RunCmd = GetComponent<RunCommand>();
        rgbd = Robot.GetComponent<Rigidbody>();
    }

    public void TransformMove(int Speed, int Sleep,List<StructCommand> condition, int Destination)
    {
        if (TimeSleep < Sleep)
        {
            TimeSleep += (Time.deltaTime * 1050);
            Robot.transform.localEulerAngles = new Vector3(0, 0, Robot.transform.localEulerAngles.z);
            rgbd.AddForce(Robot.transform.up * Speed);
            //Robot.transform.Translate(Vector3.up * Time.deltaTime * -600);
        }
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination);
        }
    }

    public void RotationMove(int Speed, int Sleep, List<StructCommand> condition, int Destination)
    {
        if (TimeSleep < Sleep)
        {
            TimeSleep += (Time.deltaTime * 1050);
            Robot.transform.RotateAround(Robot.transform.position, Vector3.forward, 1 * Speed);
        }
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination);
        }
    }

    public void StopMove(int Sleep, List<StructCommand> condition, int Destination)
    {
        if (TimeSleep < Sleep) TimeSleep += (Time.deltaTime * 1050);
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination);
        }
    }

    public void Touch(int Port, ConditionValue Value, int Destination, int EndIfDestination)
    {
        if (Value == ConditionValue.Touched)
        {
            if (Port == 1 && conditionBase.isTouched && LoadLevel.Level == 1) RunCmd.index++;
            else
            {
                RunCmd.index = EndIfDestination;
                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
            }
        }
        else if(Value == ConditionValue.UnTouched && LoadLevel.Level == 1)
        {
            if (Port == 1 && conditionBase.isTouched == false) RunCmd.index++;
            else
            {
                RunCmd.index = EndIfDestination;
                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
            }
        }
    }

    public void Color(int Port, ConditionValue Value, int Destination, int EndIfDestination)
    {
        if (LoadLevel.Level != 1)
        {
            if (Value == ConditionValue.Black)
            {
                if (Port < 3)
                {
                    if (CheckSensor[Port - 1].isColor && LoadLevel.Level != 1) RunCmd.index++;
                    else
                    {
                        RunCmd.index = EndIfDestination;
                        if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                    }
                }
                else
                {
                    RunCmd.index = EndIfDestination;
                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                }
            }
            else if (Value == ConditionValue.White)
            {
                if (Port < 3)
                {
                    if (CheckSensor[Port - 1].isColor == false && LoadLevel.Level != 1) RunCmd.index++;
                    else
                    {
                        RunCmd.index = EndIfDestination;
                        if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                    }
                }
                else
                {
                    RunCmd.index = EndIfDestination;
                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                }
            }
        }
    }

    public void Distance(int Range, int Port, int Destination, int EndIfDestination)
    {
        if (Port == 3)
        {
            if (CheckSensorDistance != null)
            {
                var isSensor = CheckSensorDistance.CheckDistance(true, Range);

                if (isSensor) RunCmd.index++;
                else
                {
                    RunCmd.index = EndIfDestination;
                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                }
            }
        }
        else
        {
            RunCmd.index = EndIfDestination;
            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
        }
    }

    public void Else()
    {
        RunCmd.index++;
    }

    public void EndIf(int Destination)
    {
        if (Destination != 0)
        {
            RunCmd.index = Destination;
            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
        }
        else NextCommand();
    }

    public void Loop(CommandsId Id, int LoopCount, int Destination, List<StructCommand> condition)
    {
        if (Id == CommandsId.Forever)
        {
            CheckCondition(condition, Destination);
        }
        else
        {
            if (LoopCount > 0) CheckCondition(condition, Destination);
        }
    }

    public void EndLoop(EndType endType, ref int LoopCount, int TmpLoopCount, int Destination, List<StructCommand> condition, List<StructCommand> EndLoopNext)
    {
        if (endType == EndType.Forever)
        {
            CheckCondition(condition, Destination);
        }
        else
        {
            if (LoopCount > 1)
            {
                LoopCount--;
                CheckCondition(condition, Destination);
            }
            else
            {
                LoopCount = TmpLoopCount;
                CheckCondition(EndLoopNext, RunCmd.index + 1);
            }
        }
    }

    public void Break(int Destination, List<StructCommand> condition)
    {
        CheckCondition(condition, Destination);
    }

    public void Interact(int Destination, List<StructCommand> condition)
    {
        if (LoadLevel.Level == 1)
        {
            Blade.GetComponent<MeshCollider>().enabled = true;
            Blade.transform.localPosition = new Vector3(Blade.transform.localPosition.x, 0.0262f, Blade.transform.localPosition.z);

            if (InteractTime > 0)
            {
                InteractTime -= Time.deltaTime;
            }
            else
            {
                Blade.transform.localPosition = new Vector3(Blade.transform.localPosition.x, 0, Blade.transform.localPosition.z);
                Blade.GetComponent<MeshCollider>().enabled = false;
                InteractTime = 1;
                CheckCondition(condition, Destination);
            }
        }
    }

    public void NextCommand()
    {
        if (RunCmd.index < Compile.ListOptimize.Count - 1) RunCmd.index++;
        else MenuButton.isPlay = false;
    }

    private void CheckCondition(List<StructCommand> condition, int Destination)
    {
        if (condition.Count > 0)
        {
            var isSensor = false;
            var CurrentInnerId = condition[0].InnerId;
            var Type = ConditionType.None;
            var Value1 = ConditionValue.None;
            var Value2 = ConditionValue.None;
            var isFoundSensor = false;

            for (int x = 0; x < condition.Count; x++)
            {
                Type = condition[x].conditionType;

                if (Type == ConditionType.Touch && LoadLevel.Level == 1)
                {
                    isSensor = CheckSensor[condition[x].Port - 1].isTouched;
                    Value1 = ConditionValue.Touched;
                    Value2 = ConditionValue.UnTouched;
                }
                else if (Type == ConditionType.Color && LoadLevel.Level != 1)
                {
                    if (condition[x].Port < 3)
                    {
                        isSensor = CheckSensor[condition[x].Port - 1].isColor;
                        Value1 = ConditionValue.Black;
                        Value2 = ConditionValue.White;
                    }
                }
                else if (Type == ConditionType.Distance && LoadLevel.Level == 3)
                {
                    if (condition[x].Port == 3)
                    {
                        isSensor = CheckSensorDistance.CheckDistance(true, condition[x].Range);
                    }
                }
                else if(Type == ConditionType.None)
                {
                    RunCmd.index = condition[x].Source;
                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                    return;
                }

                if ((Type == ConditionType.Touch && LoadLevel.Level == 1) || (Type == ConditionType.Color && LoadLevel.Level != 1))
                {
                    if (condition[x].conditionValue == Value1 && isSensor && condition[x].InnerId == CurrentInnerId)
                    {
                        if (Compile.ListOptimize[condition[x].Source].Id == CommandsId.If)
                        {
                            CurrentInnerId++;
                            continue;
                        }
                        else
                        {
                            RunCmd.index = condition[x].Source;
                            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            return;
                        }
                    }
                    else if (condition[x].conditionValue == Value2 && isSensor == false && condition[x].InnerId == CurrentInnerId)
                    {
                        if (Compile.ListOptimize[condition[x].Source].Id == CommandsId.If)
                        {
                            CurrentInnerId++;
                            continue;
                        }
                        else
                        {
                            RunCmd.index = condition[x].Source;
                            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            return;
                        }
                    }
                    else if(condition[x].EndIfDestination < condition.Count)
                    {
                        if (Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.If && Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.ElseIf &&
                            Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.Else)
                        {
                            RunCmd.index = condition[x].EndIfDestination;
                            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            return;
                        }
                        else
                        {
                            if (x + 1 >= condition.Count)
                            {
                                RunCmd.index = condition[0].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                return;
                            }

                            for (int i = x + 1; i < condition.Count; i++)
                            {
                                if (condition[i].InnerId < condition[x].InnerId) break;
                                if ((condition[i].Id == CommandsId.If || condition[i].Id == CommandsId.ElseIf || condition[i].Id == CommandsId.Else) &&
                                    condition[i].InnerId == condition[x].InnerId)
                                {
                                    x = i - 1;
                                    isFoundSensor = true;
                                    break;
                                }
                            }

                            if (isFoundSensor)
                            {
                                isFoundSensor = false;
                                continue;
                            }
                            else
                            {
                                if (condition[x].InnerId != condition[0].InnerId)
                                {
                                    RunCmd.index = condition[0].Destination;
                                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                    return;
                                }
                                else
                                {
                                    RunCmd.index = condition[0].EndIfDestination;
                                    if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (x + 1 >= condition.Count)
                        {
                            var NextCmd = Compile.ListOptimize[condition[x].EndIfDestination].Id;
                            if (NextCmd != CommandsId.If || NextCmd != CommandsId.ElseIf || NextCmd != CommandsId.Else)
                            {
                                RunCmd.index = condition[x].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            }
                            else
                            {
                                RunCmd.index = condition[0].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            }
                            return;
                        }

                        for(int i = x + 1; i < condition.Count; i++)
                        {
                            if (condition[i].InnerId < condition[x].InnerId) break;
                            if ((condition[i].Id == CommandsId.If || condition[i].Id == CommandsId.ElseIf || condition[i].Id == CommandsId.Else) && 
                                condition[i].InnerId == condition[x].InnerId)
                            {
                                x = i - 1;
                                isFoundSensor = true;
                                break;
                            }
                        }

                        if (isFoundSensor)
                        {
                            isFoundSensor = false;
                            continue;
                        }
                        else
                        {
                            if (condition[x].InnerId != condition[0].InnerId)
                            {
                                RunCmd.index = condition[0].Destination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                return;
                            }
                            else
                            {
                                RunCmd.index = condition[0].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                return;
                            }
                        }  
                    }
                }
                else if (Type == ConditionType.Distance && LoadLevel.Level == 3)
                {
                    if (isSensor && condition[x].InnerId == CurrentInnerId)
                    {
                        if (Compile.ListOptimize[condition[x].Source].Id == CommandsId.If)
                        {
                            CurrentInnerId++;
                            continue;
                        }
                        else
                        {
                            RunCmd.index = condition[x].Source;
                            if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            return;
                        }
                    }
                    else if (Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.If && Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.ElseIf &&
                            Compile.ListOptimize[condition[x].EndIfDestination].Id != CommandsId.Else)
                    {
                        RunCmd.index = condition[x].EndIfDestination;
                        if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                        return;
                    }
                    else
                    {
                        if (x + 1 >= condition.Count)
                        {
                            var NextCmd = Compile.ListOptimize[condition[x].EndIfDestination].Id;
                            if (NextCmd != CommandsId.If || NextCmd != CommandsId.ElseIf || NextCmd != CommandsId.Else)
                            {
                                RunCmd.index = condition[x].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            }
                            else
                            {
                                RunCmd.index = condition[0].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                            }
                            return;
                        }

                        for (int i = x + 1; i < condition.Count; i++)
                        {
                            if (condition[i].InnerId < condition[x].InnerId) break;
                            if ((condition[i].Id == CommandsId.If || condition[i].Id == CommandsId.ElseIf || condition[i].Id == CommandsId.Else) &&
                                condition[i].InnerId == condition[x].InnerId)
                            {
                                x = i - 1;
                                isFoundSensor = true;
                                break;
                            }
                        }

                        if (isFoundSensor)
                        {
                            isFoundSensor = false;
                            continue;
                        }
                        else
                        {
                            if (condition[x].InnerId != condition[0].InnerId)
                            {
                                RunCmd.index = condition[0].Destination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                return;
                            }
                            else
                            {
                                RunCmd.index = condition[0].EndIfDestination;
                                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
                                return;
                            }
                        }
                    }
                }

                CurrentInnerId = condition[0].InnerId;
            }

            if (Destination != 0)
            {
                RunCmd.index = Destination;
                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
            }
            else NextCommand();
        }
        else
        {
            if (Destination != 0)
            {
                RunCmd.index = Destination;
                if (RunCmd.index >= Compile.ListOptimize.Count) MenuButton.lastCmd = true;
            }
            else NextCommand();
        }
    }
}
