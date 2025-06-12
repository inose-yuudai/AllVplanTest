using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MenuCommandBase : IMenuCommand
{
    public abstract void Execute();

    public virtual bool CanExecute() => true;
}

public class NewGameCommand : MenuCommandBase
{
    private const string k_GameSceneName = "GameScene";

    public override void Execute()
    {
        SceneManager.LoadScene(k_GameSceneName);
    }
}

public class LoadGameCommand : MenuCommandBase
{
    public override void Execute()
    {
        // セーブデータロード処理
        Debug.Log("Load Game Executed");
    }

    public override bool CanExecute()
    {
        // セーブデータの存在チェック
        return PlayerPrefs.HasKey("SaveData");
    }
}

public class OptionCommand : MenuCommandBase
{
    public override void Execute()
    {
        // オプション画面への遷移
        Debug.Log("Option Menu Opened");
    }
}

public class ExitCommand : MenuCommandBase
{
    public override void Execute()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
