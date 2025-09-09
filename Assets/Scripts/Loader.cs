using UnityEngine.SceneManagement;
public static class Loader
{
    public enum Scene
    {
        MainMenu,
        MainGame,
    }

    public static void Load(Scene targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
    public static void LoadString(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
    public static void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}