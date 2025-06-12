using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuItemData
{
    [SerializeField]
    private string _displayText;

    [SerializeField]
    private bool _isEnabled = true;

    [SerializeField]
    private MenuType _menuType;

    public string DisplayText => _displayText;
    public bool IsEnabled => _isEnabled;
    public MenuType Type => _menuType;
}

public enum MenuType
{
    NewGame,
    LoadGame,
    Option,
    Exit
}
