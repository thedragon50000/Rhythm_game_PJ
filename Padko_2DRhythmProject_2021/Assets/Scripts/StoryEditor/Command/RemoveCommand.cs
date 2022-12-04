using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCommand : ICommand
{
    // Start is called before the first frame update
    public string removeName;
    public Transform parent;
    //public Transform createParent;
    public RemoveCommand(CommandReceiver receiver, GameObject removeObj, CommandUIType commandType)
    {
        this.receiver = receiver;
        this._gameObject = removeObj;
        this.commandType = commandType;
    }
    public override void Execute()
    {
        receiver.RemoveOperation(this);
    }

    public override void UnExecute()
    {
        receiver.RemoveOperation(this, true);
    }
}