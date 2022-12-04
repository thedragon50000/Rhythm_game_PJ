/// <summary>
/// The 'Invoker' class that makes calls to execute the commands
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DefinitCommandSystemHandler
{


    private CommandReceiver commandReciever;
    public List<ICommand> commands = new List<ICommand>();
    public Stack<ICommand> commandsStack = new Stack<ICommand>();
    public Stack<ICommand> undoCommandsStack = new Stack<ICommand>();
    public Stack<ICommand> redoCommandsStack = new Stack<ICommand>();
    public int currentCommandNum = 0;
    public float timer;
    public bool removeFlag;
    public void Start()
    {
        commandReciever = new CommandReceiver();
        /*
        if (objectToMove == null)
        {
            Debug.LogError("objectToMove must be assigned via inspector");
            this.enabled = false;
        }*/
    }

    public void handle(ActionType actType, ICommand command = null)
    {
        if (actType == ActionType.exe)
        {
            command.Execute();
            undoCommandsStack.Push(command);
            redoCommandsStack.Clear();
        }
        if (actType == ActionType.undo && undoCommandsStack.Count > 0)
        {
            ICommand comm = undoCommandsStack.Pop();
            comm.UnExecute();
            redoCommandsStack.Push(comm);
        }
        if (actType == ActionType.redo && redoCommandsStack.Count > 0)
        {
            ICommand comm = redoCommandsStack.Pop();
            comm.Execute();
            undoCommandsStack.Push(comm);
        }
    }



    public void Undo()
    {
        if (currentCommandNum > 0)
        {
            currentCommandNum--;
            ICommand command = commands[currentCommandNum];
            command.UnExecute();
        }
    }

    public void Redo()
    {
        if (currentCommandNum < commands.Count)
        {
            ICommand command = commands[currentCommandNum];
            currentCommandNum++;
            command.Execute();
        }
    }
    public enum ActionType
    {
        exe,undo,redo
    }




    public void Add(CommandUIType commandType, GameObject addObj)
    {
        //List
        //if(commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}
        //ICommand command = new AddCommand(commandReciever, addObj, commandType);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new AddCommand(commandReciever, addObj, commandType);
        handle(ActionType.exe, command);

        addCommandStrings("Add");
    }
    public void Select(CommandUIType commandType, GameObject buttonObj, string selectObjectName)
    {
        //if (commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}
        //ICommand command = new SelectCommand(commandType, commandReciever, buttonObj, selectObjectName);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new SelectCommand(commandType, commandReciever, buttonObj, selectObjectName);
        handle(ActionType.exe, command);

        addCommandStrings("Select");
    }
    public void Rename(CommandUIType commandType, GameObject reNameObj, string changeName)
    {
        //if (removeFlag)
        //    return;
        //var facade = StoryEditorFacade.Instance;
        //string clickKey = null;
        //switch(commandType)
        //{
        //    case CommandUIType.chara:
        //        clickKey = UnityTool.KeyByValue(facade.definitUI.charasListObjDic, reNameObj);
        //        break;
        //    case CommandUIType.attr:
        //        clickKey = UnityTool.KeyByValue(facade.definitUI.attributeObjDic, reNameObj);
        //        break;
        //}
        //if (changeName == clickKey)
        //    return;
        //if (commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}

        //ICommand command = new RenameCommand(commandReciever, reNameObj, changeName, commandType);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new RenameCommand(commandReciever, reNameObj, changeName, commandType);
        handle(ActionType.exe, command);

        addCommandStrings("Rename");
    }

    public void Move(CommandUIType commandType, GameObject moveObj, int startIndex, int moveIndex)
    {
        //if (commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}
        //ICommand command = new MoveCommand(commandType, commandReciever, moveObj, startIndex, moveIndex);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new MoveCommand(commandType, commandReciever, moveObj, startIndex, moveIndex);
        handle(ActionType.exe, command);

        addCommandStrings("Move");
    }
    public void Copy(CommandUIType commandType, GameObject copyObj)
    {
        //if (commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}
        //ICommand command = new CopyCommand(commandReciever, copyObj ,commandType);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new CopyCommand(commandReciever, copyObj, commandType);
        handle(ActionType.exe, command);

        addCommandStrings("Copy");
    }
    public void Remove(CommandUIType commandType, GameObject removeObj)
    {
        //if (commands.Count >= 50)
        //{
        //    commands.RemoveAt(0);
        //    currentCommandNum--;
        //}
        //ICommand command = new RemoveCommand(commandReciever, removeObj, commandType);
        //command.Execute();
        //commands.Add(command);
        //currentCommandNum++;

        //Stack
        ICommand command = new RemoveCommand(commandReciever, removeObj, commandType);
        handle(ActionType.exe, command);

        addCommandStrings("Remove");
    }
    public void addCommandStrings(string name)
    {
        var facade = StoryEditorFacade.Instance;
        facade.definitUI.commandString_debug.Add(name);
    }


    public void Update()
    {
        var facade = StoryEditorFacade.Instance;
        if (Input.GetKeyDown(KeyCode.R))
        {
            handle(ActionType.redo);
            // Redo();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            handle(ActionType.undo);
            //Undo();
        }
        if(Input.GetKeyDown(KeyCode.D) && facade.definitUI.selectCharaName.Length > 0)
        {
            Copy(CommandUIType.chara, facade.definitUI.charasListObjDic[facade.definitUI.selectCharaName]);
        }
        if (Input.GetKeyDown(KeyCode.Delete) )
        {
            if(!removeFlag)
                removeFlag = true;
        }
        if(removeFlag)
        {
            timer += Time.deltaTime;
            if(timer >= 0.1f)
            {
                if(facade.definitUI.selectCharaName.Length > 0)
                    Remove(CommandUIType.chara, facade.definitUI.charasListObjDic[facade.definitUI.selectCharaName]);
                removeFlag = false;
                timer = 0;
            }
            
        }
    }
}
