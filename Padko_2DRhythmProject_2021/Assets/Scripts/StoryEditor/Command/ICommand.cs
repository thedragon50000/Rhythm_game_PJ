
using UnityEngine;
/// <summary>
/// The 'Command' abstract class that we will inherit from
/// </summary>
/// 

public abstract class ICommand
{
    public CommandReceiver receiver;
    public CommandUIType commandType; 
    public GameObject _gameObject; 
    public abstract void Execute();
    public abstract void UnExecute();
}
