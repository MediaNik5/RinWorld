namespace RinWorld.Util.Unity.Buttons
{
    public class SaveGameButton : BaseButton
    {
        public SaveGameButton() : base("Save game")
        {
        }

        public override void Update()
        {
            base.Update();
            MainMenuButton.interactable = Game.GameState != GameState.Menu;
        }
    }
}