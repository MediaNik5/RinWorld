using RimCopy.Data;
using UnityEngine;
using UnityEngine.UI;

namespace RimCopy.Unity
{
    public class Menu : MonoBehaviour
    {
        private GameObject menu;

        private Button startNewGame;
        private Button loadGame;
        private Button saveGame;
        private Button settings;
        private Button exit;
        private Text text;

        // [SerializeField] private bool _mainMenu;
        [SerializeField] private bool _isMenuOpened;

        private void Start()
        {
            var canvas = FindObjectOfType<Canvas>();
            menu = canvas.transform.Find("Menu").gameObject;
            startNewGame = menu.transform.Find("StartNewGame").GetComponent<Button>();
            // startNewGame.onClick.AddListener(StartNewGame);
            loadGame = menu.transform.Find("LoadGame").GetComponent<Button>();
            // loadGame.onClick.AddListener(LoadGame);
            saveGame = menu.transform.Find("SaveGame").GetComponent<Button>();
            // saveGame.onClick.AddListener(SaveGame);
            settings = menu.transform.Find("Settings").GetComponent<Button>();
            // settings.onClick.AddListener(Settings);
            exit = menu.transform.Find("Exit").GetComponent<Button>();
            // exit.onClick.AddListener(Exit);
            text = menu.transform.Find("Menu").GetComponent<Text>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isMenuOpened = !_isMenuOpened;
                if (_isMenuOpened)
                    OpenMenu();
                else
                    CloseMenu();
            }
        }

        private void OpenMenu()
        {
            Game.SetPause(true);
            menu.SetActive(true);
            startNewGame.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate("Start new game");
            loadGame.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate("Load game");
            saveGame.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate("Save game");
            saveGame.interactable = Game.GameState != GameState.Menu;
            settings.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate("Settings");
            exit.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate("Exit");
            text.text = DataHolder.Translate("Menu");
        }

        private void CloseMenu()
        {
            Game.SetPause(false);
            menu.SetActive(false);
        }
    }
}