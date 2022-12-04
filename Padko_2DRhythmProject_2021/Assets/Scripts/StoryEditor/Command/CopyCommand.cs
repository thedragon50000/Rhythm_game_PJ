using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCommand : ICommand
{
    public string copyName;
    public Transform parent;
    public CopyCommand(CommandReceiver receiver, GameObject copyObj, CommandUIType commandType)
    {
        this.receiver = receiver;
        this._gameObject = copyObj;
        this.commandType = commandType;
    }
    public override void Execute()
    {
        receiver.CopyOperation(this);
    }

    public override void UnExecute()
    {
        receiver.CopyOperation(this, true);
    }
}
