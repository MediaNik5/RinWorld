using UnityEngine;

namespace RinWorld.Util.Unity.Buttons
{
    public class ExitButton : BaseButton
    {
        public ExitButton() : base("Exit")
        {
        }

        protected override void OnClick()
        {
            base.OnClick();
            _confirm.onClick.AddListener(() => Application.Quit(0));
        }
    }
}