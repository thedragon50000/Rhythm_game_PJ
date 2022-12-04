using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCommand : ICommand
{
    public string selectName;
    public string resetName;
    public SelectCommand(CommandUIType commandType, CommandReceiver receiver,GameObject buttonObj, string selectName)
    {
        this.receiver = receiver;
        this._gameObject = buttonObj;
        this.commandType = commandType;
        this.selectName = selectName;
    }
    public override void Execute()
    {
        receiver.SelectOperation(this);
    }

    public override void UnExecute()
    {
        receiver.SelectOperation(this,true);
    }
}
