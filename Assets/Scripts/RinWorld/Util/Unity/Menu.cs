using RinWorld.Util.Data;
using RinWorld.Util.Unity.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace RinWorld.Util.Unity
{
    public class Menu : MonoBehaviour
    {
        // [SerializeField] private bool _mainMenu;
        [SerializeField] private bool isMenuOpened;
        private BaseButton[] _buttons;
        private GameObject _menu;
        private Text _header;

        private void Start()
        {
            var canvas = FindObjectOfType<Canvas>();

            var menuTransform = canvas.transform.Find("Menu");
            _header = menuTransform.Find("Text").GetComponent<Text>();
            _menu = menuTransform.gameObject;

            foreach (var button in Buttons())
                button.Init(menuTransform);


            if (Game.GameState == GameState.Menu)
                OpenMenu();
        }


        private void Update()
        {
            if ((Game.GameState == GameState.Paused || Game.GameState == GameState.Playing) &&
                Input.GetKeyDown(KeyCode.Escape))
            {
                isMenuOpened = !isMenuOpened;
                if (isMenuOpened)
                    OpenMenu();
                else
                    CloseMenu();
            }

            if (Game.GameState == GameState.Loading || Game.GameState == GameState.Saving)
                CloseMenu();
        }

        private void OpenMenu()
        {
            Game.SetPause(true);
            _menu.SetActive(true);
            foreach (var button in Buttons())
                button.Update();

            _header.text = DataHolder.Translate("Menu");
        }

        public void CloseMenu()
        {
            Game.SetPause(false);
            _menu.SetActive(false);
        }


        private BaseButton[] Buttons()
        {
            if (_buttons == null)
            {
                _buttons = new BaseButton[5];
                _buttons[0] = new StartNewGameButton();
                _buttons[1] = new LoadGameButton();
                _buttons[2] = new SaveGameButton();
                _buttons[3] = new SettingsButton();
                _buttons[4] = new ExitButton();
            }

            return _buttons;
        }
    }
}