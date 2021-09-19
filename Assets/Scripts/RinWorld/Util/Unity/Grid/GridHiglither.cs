using RinWorld.Util.Data;
using RinWorld.Util.Unity.Buttons;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace RinWorld.Util.Unity.Grid
{
    public class GridHiglither : MonoBehaviour
    {
        private const int XForValue = 210;
        private const int XForName = 10;
        private const int XSize = 180;
        private const int YSize = 20;
        private Camera _camera;
        private GameObject _cellInfoPane;
        private Button _play;
        private UnityEngine.Grid _grid;
        private ImmutableTile _hover;
        private Vector3Int _previousMousePosition;
        private Vector3Int _previousSelection = new Vector3Int(int.MinValue, 0, 0); // First is dummy position
        private ImmutableTile _select;
        private Tilemap _util;

        private void Start()
        {
            if (Game.GameState == GameState.Corrupted)
                return;
            _camera = Camera.main;
            _grid = gameObject.GetComponent<UnityEngine.Grid>();
            _util = Game.GetUtilTilemap();
            _hover = DataHolder.GetUtilTile("hover");
            _select = DataHolder.GetUtilTile("select");
            _cellInfoPane = FindObjectOfType<Canvas>().transform.Find("CellInfoPane").gameObject;
            _play = _cellInfoPane.transform.Find("Play").GetComponent<Button>();
        }

        private void Update()
        {
            if (Game.GameState != GameState.Playing)
                return;

            var mousePosition = GetMousePosition();
            if (mousePosition != _previousMousePosition)
            {
                if (_previousMousePosition != _previousSelection)
                    _util.SetTile(_previousMousePosition, null);
                _hover.ApplyFor(_util, mousePosition);
                _previousMousePosition = mousePosition;
            }

            if (mousePosition != _previousSelection && Input.GetMouseButtonDown(0))
            {
                _util.SetTile(_previousSelection, null);
                _select.ApplyFor(_util, mousePosition);
                _previousSelection = mousePosition;
                ShowCellInfo(mousePosition.x, mousePosition.y);
            }
        }

        private void ShowCellInfo(int x, int y)
        {
            var infos = Game.World.GetCellInfo(x, y);
            _cellInfoPane.SetActive(true);
            var i = 0;
            foreach (var (name, value) in infos)
            {
                UpdateProperty(i, name, false);
                UpdateProperty(i, value, true);
                i++;
            }

            BaseButton.SetTranslation(_play, "Play");
            _play.onClick.RemoveAllListeners();
            _play.onClick.AddListener(() => Game.StartColony(x, y));
        }

        private void UpdateProperty(int i, string value, bool isValue)
        {
            Transform text = _cellInfoPane.transform.Find("Property " + (isValue ? "value" : "key") + $" {i}");
            if (text != null)
                text.GetComponent<Text>().text = DataHolder.Translate(value);
            else
                CreateNewProperty(i, value, isValue);
        }

        private void CreateNewProperty(int i, string value, bool isValue)
        {
            var newText = new GameObject(
                "Property " + (isValue ? "value" : "key") + $" {i}",
                typeof(RectTransform)
            );
            newText.transform.SetParent(_cellInfoPane.transform, false);
            SetTextComponent(newText, value);
            SetTransformComponent(newText, XFor(isValue), YFor(i));
        }

        private static void SetTextComponent(GameObject newText, string value)
        {
            var textComponent = newText.AddComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.text = DataHolder.Translate(value);
        }
        
        private static int XFor(bool isValue) => isValue ? XForValue : XForName;
        private static int YFor(int i) => -50 - 20 * i;

        private static void SetTransformComponent(GameObject newText, int x, int y)
        {
            var rectTransform = newText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.sizeDelta = new Vector2(XSize, YSize);
        }

        private Vector3Int GetMousePosition()
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            return _grid.WorldToCell(mouseWorldPosition);
        }
    }
}