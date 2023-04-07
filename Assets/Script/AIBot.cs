using System.Collections.Generic;
using UnityEngine;
using Commands;

public class AIBot : MonoBehaviour
{
    [SerializeField] Compiler Compile;
    [SerializeField] private MenuButton MenuButton;
    [HideInInspector] public Vector3 BeginPos;
    [HideInInspector] public Quaternion BeginRotate;
    [HideInInspector] public int indexAI = 0;
    private CommandsAIController CommandAIControl;

    // Use this for initialization
    void Start ()
    {
        CommandAIControl = GetComponent<CommandsAIController>();
        BeginPos = transform.localPosition;
        BeginRotate = transform.rotation;
    }

    public void FixedUpdate()
    {
        if (MenuButton.isAIPlay)
        {
            if (Compile.ListCommandAI.Count == 0)
            {
                MenuButton.isAIPlay = false;
                return;
            }

            var Command = Compile.ListCommandAI[indexAI];

            if (Command.Type == CommandsType.Transform)
            {
                CommandAIControl.TransformMove(Command.Speed, Command.Sleep, Command.ListCondition, Command.Destination);
            }
            else if (Command.Type == CommandsType.Rotation)
            {
                CommandAIControl.RotationMove(Command.Speed, Command.Sleep, Command.ListCondition, Command.Destination);
            }
            else if (Command.conditionType == ConditionType.Color)
            {
                CommandAIControl.Color(Command.Port, Command.conditionValue, Command.Destination, Command.EndIfDestination);
            }
            else if (Command.Id == CommandsId.EndIf)
            {
                CommandAIControl.EndIf(Command.Destination);
            }
            else if (Command.Id == CommandsId.Forever || Command.Id == CommandsId.For)
            {
                CommandAIControl.Loop(Command.Id, Command.LoopCount, Command.Destination, Command.ListCondition);
            }
            else if (Command.Id == CommandsId.EndLoop)
            {
                CommandAIControl.EndLoop(Command.endType, ref Command.LoopCount, Command.TmpLoopCount, Command.Destination, Command.ListCondition, Command.ListEndLoopNext);
            }
        }
        else
        {
            if (MenuButton.isStop)
            {
                gameObject.transform.localPosition = BeginPos;
                gameObject.transform.rotation = BeginRotate;

                CommandAIControl.TimeSleep = 0;

                indexAI = 0;
                Compile.ListCommandAI = new List<StructCommand>();
            }
        }
    }
}
