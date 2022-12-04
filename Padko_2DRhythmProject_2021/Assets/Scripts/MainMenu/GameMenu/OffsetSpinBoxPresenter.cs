using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteEditor.Presenter;
using UniRx;
using UnityEngine.UI;
using Game.MainMenu;
public class OffsetSpinBoxPresenter : SpinBoxPresenterBase
{
    public SettingMenu settingMenu;
    //public Slider slider;
    protected override ReactiveProperty<int> GetReactiveProperty()
    {
        return settingMenu.offset;
    }
}
