using UnityEngine;
using Commands;
using System.Collections.Generic;

public class RunCommand : MonoBehaviour
{
    [SerializeField] private MenuButton MenuButton;
    [SerializeField] private Compiler Compile;
    [SerializeField] private GameObject Robot;
    [HideInInspector] public Vector3 BeginPos;
    [HideInInspector] public Quaternion BeginRotate;
    public int index = 0;
    private CommandsController CommandControl;
    private StructCommand Command;

    void Start()
    {
        CommandControl = GetComponent<CommandsController>();
        BeginPos = Robot.transform.localPosition;
        BeginRotate = Robot.transform.rotation;
    }

    public void FixedUpdate()
    {
        if (MenuButton.isPlay && MenuButton.lastCmd == false)
        {
            if(Compile.ListOptimize.Count == 0)
            {
                MenuButton.isPlay = false;
                return;
            }

            Command = Compile.ListOptimize[index];

            if (Command.Type == CommandsType.Transform)
            {
                CommandControl.TransformMove(Command.Speed, Command.Sleep, Command.ListCondition, Command.Destination);
            }
            else if (Command.Type == CommandsType.Rotation)
            {
                CommandControl.RotationMove(Command.Speed, Command.Sleep, Command.ListCondition, Command.Destination);
            }
            else if (Command.Type == CommandsType.Idle)
            {
                CommandControl.StopMove(Command.Sleep, Command.ListCondition, Command.Destination);
            }
            else if (Command.conditionType == ConditionType.Touch)
            {
                CommandControl.Touch(Command.Port, Command.conditionValue, Command.Destination, Command.EndIfDestination);
            }
            else if (Command.conditionType == ConditionType.Color)
            {
                CommandControl.Color(Command.Port, Command.conditionValue, Command.Destination, Command.EndIfDestination);
            }
            else if (Command.conditionType == ConditionType.Distance)
            {
                CommandControl.Distance(Command.Range, Command.Port, Command.Destination, Command.EndIfDestination);
            }
            else if (Command.Id == CommandsId.Else)
            {
                CommandControl.Else();
            }
            else if (Command.Id == CommandsId.EndIf)
            {
                CommandControl.EndIf(Command.Destination);
            }
            else if (Command.Id == CommandsId.Forever || Command.Id == CommandsId.For)
            {
                CommandControl.Loop(Command.Id, Command.LoopCount, Command.Destination, Command.ListCondition);
            }
            else if (Command.Id == CommandsId.EndLoop)
            {
                CommandControl.EndLoop(Command.endType, ref Command.LoopCount, Command.TmpLoopCount, Command.Destination, Command.ListCondition, Command.ListEndLoopNext);
            }
            else if (Command.Id == CommandsId.Break)
            {
                CommandControl.Break(Command.Destination, Command.ListCondition);
            }
            else if (Command.Id == CommandsId.Interact)
            {
                CommandControl.Interact(Command.Destination, Command.ListCondition);
            }
        }
        else
        {
            if (MenuButton.isStop)
            {
                Robot.transform.localPosition = BeginPos;
                Robot.transform.rotation = BeginRotate;

                index = 0;
                Compile.ListOptimize = new List<StructCommand>();
            }
        }
    }
}
