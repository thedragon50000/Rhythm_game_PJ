using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    public int startIndex;
    public int moveIndex;
    public MoveCommand(CommandUIType commandType, CommandReceiver receiver, GameObject moveObj, int startIndex, int moveIndex)
    {
        this.receiver = receiver;
        this._gameObject = moveObj;
        this.startIndex = startIndex;
        this.moveIndex = moveIndex;
        this.commandType = commandType;
    }


    //Execute new command
    public override void Execute()
    {
        receiver.MoveOperation(this);
    }


    //Undo last command
    public override void UnExecute()
    {
        receiver.MoveOperation(this, true);
    }
}
