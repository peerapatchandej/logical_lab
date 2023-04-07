using System.Collections.Generic;
using UnityEngine;
using Commands;

public class CommandsAIController : MonoBehaviour
{
    [SerializeField] private MenuButton MenuButton;
    [SerializeField] private ConditionBase conditionBase;
    [SerializeField] private CheckSensor[] CheckSensor;
    [HideInInspector] public float TimeSleep = 0f;
    private ConditionValue Value1;
    private ConditionValue Value2;
    private ConditionType Type;
    private AIBot RunCmd;
    private Rigidbody rgbd;

    void Start ()
    {
        RunCmd = GetComponent<AIBot>();
        rgbd = GetComponent<Rigidbody>();

        Value1 = ConditionValue.Black;
        Value2 = ConditionValue.White;
        Type = ConditionType.Color;
    }

    public void TransformMove(int Speed, int Sleep,List<StructCommand> condition,int Destination)
    {
        if (TimeSleep < Sleep)
        {
            TimeSleep += (Time.deltaTime * 1050);
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
            rgbd.AddForce(transform.up * Speed);
        }
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination, Value1, Value2, Type);
        }
    }

    public void RotationMove(int Speed, int Sleep, List<StructCommand> condition, int Destination)
    {
        if (TimeSleep < Sleep)
        {
            TimeSleep += (Time.deltaTime * 1050);
            transform.Rotate(new Vector3(0, 0, 1) * Speed);
        }
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination, Value1, Value2, Type);
        }
    }

    public void StopMove(int Sleep, List<StructCommand> condition, int Destination)
    {
        if (TimeSleep < Sleep) TimeSleep += (Time.deltaTime * 1050);
        else
        {
            TimeSleep = 0;
            CheckCondition(condition, Destination, Value1, Value2, Type);
        }
    }

    public void Color(int Port, ConditionValue Value, int Destination, int EndIfDestination)
    {
        if (Value == ConditionValue.Black)
        {
            if ((Port == 1 || Port == 2) && conditionBase.isTouched) RunCmd.indexAI++;
            else
            {
                RunCmd.indexAI = EndIfDestination;
            }
        }
        else if (Value == ConditionValue.White)
        {
            if ((Port == 1 || Port == 2) && conditionBase.isTouched == false) RunCmd.indexAI++;
            else
            {
                RunCmd.indexAI = EndIfDestination;
            }
        }
    }

    public void EndIf(int Destination)
    {
        if (Destination != 0)
        {
            RunCmd.indexAI = Destination;
        }
        else NextCommand();
    }

    public void Loop(CommandsId Id, int LoopCount, int Destination, List<StructCommand> condition)
    {
        if (Id == CommandsId.Forever)
        {
            CheckCondition(condition, Destination, Value1, Value2, Type);
        }
        else
        {
            if (LoopCount > 0) CheckCondition(condition, Destination, Value1, Value2, Type);
        }
    }

    public void EndLoop(EndType endType, ref int LoopCount, int TmpLoopCount, int Destination, List<StructCommand> condition, List<StructCommand> EndLoopNext)
    {
        if (endType == EndType.Forever)
        {
            CheckCondition(condition, Destination, Value1, Value2, Type);
        }
        else
        {
            if (LoopCount > 1)
            {
                LoopCount--;
                CheckCondition(condition, Destination, Value1, Value2, Type);
            }
            else
            {
                LoopCount = TmpLoopCount;
                CheckCondition(EndLoopNext, RunCmd.indexAI + 1, Value1, Value2, Type);
            }
        }
    }

    public void NextCommand()
    {
        RunCmd.indexAI++;
    }

    private void CheckCondition(List<StructCommand> condition, int Destination, ConditionValue Value1, ConditionValue Value2, ConditionType Type)
    {
        if (condition.Count > 0)
        {
            bool isSensor = false;
            foreach (var i in condition)
            {
                if (Type == ConditionType.Touch) isSensor = CheckSensor[i.Port - 1].isTouched;
                else if (Type == ConditionType.Color) isSensor = CheckSensor[i.Port - 1].isColor;

                if (i.conditionValue == Value1 && isSensor)
                {
                    RunCmd.indexAI = i.Source;
                    return;
                }
                else if (i.conditionValue == Value2 && isSensor == false)
                {
                    RunCmd.indexAI = i.Source;
                    return;
                }
            }
            if(Destination != 0)
            {
                RunCmd.indexAI = Destination;
            }
            else NextCommand();
        }
        else
        {
            if (Destination != 0)
            {
                RunCmd.indexAI = Destination;
            }
            else NextCommand();
        }
    }
}
