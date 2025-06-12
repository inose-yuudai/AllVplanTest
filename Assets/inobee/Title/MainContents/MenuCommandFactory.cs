using UnityEngine;

public class MenuCommandFactory
{
    public static IMenuCommand CreateCommand(MenuType menuType)
    {
        return menuType switch
        {
            MenuType.NewGame => new NewGameCommand(),
            MenuType.LoadGame => new LoadGameCommand(),
            MenuType.Option => new OptionCommand(),
            MenuType.Exit => new ExitCommand(),
            _ => throw new System.ArgumentException($"Unknown menu type: {menuType}")
        };
    }
}
