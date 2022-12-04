/// <summary>
/// A simple example of a class inheriting from a command pattern
/// This handles execution of the command as well as unexecution of the command
/// </summary>

using UnityEngine;

// A basic enum to describe our movement
public enum CommandUIType { none, chara, img, attr, item };

public class AddCommand : ICommand
{

    
    //private float _distance;
    public Transform parent;
    public string objName;

    public GameObject createObj;
    

    //Constructor
    public AddCommand(CommandReceiver receiver, GameObject addObj, CommandUIType commandType) 
    {
        this.receiver = receiver;
        this.createObj = addObj;
        objName = null;
        this.commandType = commandType;
    }


    //Execute new command
    public override void Execute()
    {   
        receiver.AddOperation(this);
    }


    //Undo last command
    public override void UnExecute()
    {
        receiver.AddOperation(this, true);
        //receiver.AddOperation(_gameObject, InverseDirection(commandType), _distance);
    }


    //invert the direction for undo
    private CommandUIType InverseDirection(CommandUIType direction)
    {
        switch (direction)
        {
            case CommandUIType.chara:
                return CommandUIType.img;
            case CommandUIType.img:
                return CommandUIType.chara;
            case CommandUIType.attr:
                return CommandUIType.item;
            case CommandUIType.item:
                return CommandUIType.attr;
            default:
                Debug.LogError("Unknown MoveDirection");
                return CommandUIType.chara;
        }
    }


    //So we can show this command in debug output easily
    public override string ToString()
    {
        //return _gameObject.name + " : " + MoveDirectionString(commandType) + " : " + _distance.ToString();
        return null;
    }


    //Convert the MoveDirection enum to a string for debug
    public string MoveDirectionString(CommandUIType direction)
    {
        switch (direction)
        {
            case CommandUIType.chara:
                return "up";
            case CommandUIType.img:
                return "down";
            case CommandUIType.attr:
                return "left";
            case CommandUIType.item:
                return "right";
            default:
                return "unkown";
        }
    }
}
