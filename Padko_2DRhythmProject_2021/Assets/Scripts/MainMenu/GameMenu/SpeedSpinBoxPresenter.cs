using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteEditor.Presenter;
using UniRx;
using Game.MainMenu;
public class SpeedSpinBoxPresenter : SpinBoxPresenterBase
{
    public SettingMenu settingMenu;
    //public Slider slider;
    protected override ReactiveProperty<int> GetReactiveProperty()
    {
        return settingMenu.speed;
    }
}
