using System;
using RinWorld.Util.Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RinWorld.Util.Unity.Buttons
{
    public class StartNewGameButton : BaseButton
    {
        private InputField _inputSeed;
        private Text _seed;

        public StartNewGameButton() : base("New game")
        {
        }

        public override void Init(Transform menuTransform)
        {
            base.Init(menuTransform);
            _seed = _popup.transform.Find("Seed").GetComponent<Text>();
            _inputSeed = _popup.transform.Find("InputField").GetComponent<InputField>();
        }

        public override void Update()
        {
            base.Update();
            _seed.text = DataHolder.Translate("Seed");
            _inputSeed.text = Random.Range(0, int.MaxValue) + "";
            MainMenuButton.interactable = Game.GameState == GameState.Menu;
        }

        protected override void OnClick()
        {
            base.OnClick();

            _confirm.onClick.AddListener(() => Game.StartGame(Convert.ToInt32(_inputSeed.text)));
        }
    }
}