using UnityEngine;

public class StartupSettings : MonoBehaviour
{
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // ya da FullScreenMode.ExclusiveFullScreen
        Screen.fullScreen = true;
    }
}
