using RinWorld.Util.Data;
using RinWorld.Util.Unity.Buttons;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace RinWorld.Util.Unity.Grid
{
    public class GridHiglither : MonoBehaviour
    {
        private Camera _camera;
        private GameObject _cellInfoPane;
        private Button _play;
        private UnityEngine.Grid _grid;
        private ImmutableTile _hover;
        private Vector3Int _previousMousePosition;
        private Vector3Int _previousSelection = new Vector3Int(int.MinValue, 0, 0);
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
                CreateProperty(i, name, false);
                CreateProperty(i, value, true);
                i++;
            }

            BaseButton.SetTranslation(_play, "Play");
            _play.onClick.RemoveAllListeners();
            _play.onClick.AddListener(() => Game.StartColony(x, y));
        }

        private void CreateProperty(int i, string value, bool isValue)
        {
            var text = _cellInfoPane.transform.Find("Property " + (isValue ? "value" : "key") + $" {i}");
            if (text == null)
            {
                var newText = new GameObject("Property " + (isValue ? "value" : "key") + $" {i}",
                    typeof(RectTransform));
                newText.transform.SetParent(_cellInfoPane.transform, false);
                var rectTransform = newText.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchoredPosition = new Vector2(isValue ? 210 : 10, -50 - 20 * i);
                rectTransform.sizeDelta = new Vector2(180, 20);
                var textComponent = newText.AddComponent<Text>();
                textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                textComponent.text = DataHolder.Translate(value);
            }
            else
            {
                text.GetComponent<Text>().text = DataHolder.Translate(value);
            }
        }

        private Vector3Int GetMousePosition()
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            return _grid.WorldToCell(mouseWorldPosition);
        }
    }
}