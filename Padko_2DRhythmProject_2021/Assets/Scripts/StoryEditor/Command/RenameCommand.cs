using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RenameCommand : ICommand
{

    public string changeName;
    public string resetName;
    public RenameCommand(CommandReceiver receiver, GameObject reNameObj, string changeName , CommandUIType commandType)
    {
        this.receiver = receiver;
        this._gameObject = reNameObj;
        this.changeName = changeName;
        this.commandType = commandType;
    }
    public override void Execute()
    {
        receiver.RenameOperation(this);
    }

    public override void UnExecute()
    {
        receiver.RenameOperation(this, true);
    }
}
