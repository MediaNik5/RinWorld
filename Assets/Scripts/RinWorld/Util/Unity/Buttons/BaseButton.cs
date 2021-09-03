using System;
using RinWorld.Util.Data;
using UnityEngine;
using UnityEngine.UI;

namespace RinWorld.Util.Unity.Buttons
{
    public abstract class BaseButton
    {
        public readonly string name;
        private Button _cancel;
        protected Button _confirm;
        private Text _header;

        protected GameObject _popup;

        public Button MainMenuButton { get; private set; }

        protected BaseButton(string name)
        {
            this.name = name;
        }

        public virtual void Init(Transform menuTransform)
        {
            MainMenuButton = menuTransform.Find(name).GetComponent<Button>();
            MainMenuButton.onClick.AddListener(OnClick);

            var exitPopupTransform = menuTransform.Find($"{name} popup");
            _popup = exitPopupTransform.gameObject;

            _confirm = exitPopupTransform.Find("Confirm").GetComponent<Button>();
            _cancel = exitPopupTransform.Find("Cancel").GetComponent<Button>();
            _cancel.onClick.AddListener(() => _popup.SetActive(false));
            _header = exitPopupTransform.Find("Header").GetComponent<Text>();
        }

        public virtual void Update()
        {
            SetTranslation(MainMenuButton, name);
            SetTranslation(_confirm, "Confirm");
            SetTranslation(_cancel, "Cancel");
            _header.text = DataHolder.Translate($"{name} header");
        }

        protected virtual void OnClick()
        {
            _popup.SetActive(true);
        }

        public static void SetTranslation(Component obj, string name)
        {
            try
            {
                obj.transform.Find("Text").GetComponent<Text>().text = DataHolder.Translate(name);
            }
            catch (NullReferenceException ex)
            {
                Debug.Log("Null");
            }
        }
    }
}