using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public GameMenuManager gameMenu;
    bool active;
    public async void OnButtonPressed(string buttonIndex)
    {
        if (active) return;
        active = true;
        print(buttonIndex);

        switch (buttonIndex)
        {
            case "Quit To Menu":
                Time.timeScale = 1;
                print("a");
                await Task.WhenAll(Fade(false));

                print("b");
                SceneManager.LoadScene(0);
                break;

            case "Quit":
                await Task.WhenAll(Fade(false));

                Application.Quit();
                break;


            case "Continue":
                gameMenu.OnChangeMenuState(false);
                break;

            case "New Game":
                print("a");

                await Task.WhenAll(Fade(false));
                print("b");

                SceneManager.LoadScene(1);
                break;

            default:
                break;
        }
        active = false;
    }

    async Task Fade(bool value)
    {
        FadeManager.Fade(value);

        await Task.Delay(1300);
    }
}
