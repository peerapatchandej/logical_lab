using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public enum CommandsType
    {
        Transform,
        Rotation,
        Idle,
        Condition,
        Loop,
        Interact
    }

    public enum CommandsId
    {
        Forward,
        Backward,
        TurnLeft,
        TurnRight,
        Stop,
        If,
        ElseIf,
        Else,
        EndIf,
        Forever,
        For,
        EndLoop,
        Break,
        Interact
    }

    public enum EndType
    {
        None,
        If,
        ElseIf,
        Else,
        Forever,
        For
    }

    public class StructCommand : MonoBehaviour
    {
        public CommandsId Id;
        public CommandsType Type;
        public int Sleep;
        public int Speed;
        public int ConditionId;
        public int InnerId;
        public int Port;
        public ConditionType conditionType;
        public ConditionValue conditionValue;
        public int Range;
        public int LoopId;
        public LoopType LoopType;
        public int LoopCount;
        public int TmpLoopCount;
        public EndType endType;
        public int Source;
        public int Destination;
        public int EndIfDestination;
        public List<StructCommand> ListCondition;
        public List<StructCommand> ListEndLoopNext;
    }
}

