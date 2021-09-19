using UnityEngine;
using UnityEngine.UI;

namespace RinWorld.Util.Unity
{
    public class Starter : MonoBehaviour
    {
        private void Awake()
        {
            try
            {
                Game.LaunchGame();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                var canvas = FindObjectOfType<Canvas>();
                var errorPane = canvas.transform.Find("ErrorPane");
                errorPane.gameObject.SetActive(true);

                var errorText = errorPane.Find("Text").GetComponent<Text>();
                errorText.text = "Here is the stacktrace for current exception. " +
                                 $"Full stacktrace can be found in {Application.persistentDataPath}/Player.log\n" + ex;

                var button = errorPane.Find("ExitButton").GetComponent<Button>();
                button.onClick.AddListener(() => Application.Quit(1));
            }
        }
    }
}