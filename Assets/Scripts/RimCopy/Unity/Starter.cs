using UnityEngine;
using UnityEngine.UI;

namespace RimCopy.Unity
{
    public class Starter : MonoBehaviour
    {
        private void Awake()
        {
            try
            {
                Game.StartGame();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                var canvas = FindObjectOfType<Canvas>();
                var errorPane = canvas.transform.Find("ErrorPane").gameObject;
                errorPane.SetActive(true);

                var errorText = errorPane.transform.Find("ErrorText").GetComponent<Text>();
                errorText.text = "Here is the stacktrace for current exception. " +
                                 $"Full stacktrace can be found in {Application.persistentDataPath}/Player.log\n" + ex;

                var button = errorPane.transform.Find("ExitButton").GetComponent<Button>();
                button.onClick.AddListener(() => Application.Quit(1));
            }
        }
    }
}