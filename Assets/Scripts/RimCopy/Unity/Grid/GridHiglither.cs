using RimCopy.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.Unity.Grid
{
    public class GridHiglither : MonoBehaviour
    {
        private UnityEngine.Grid _grid;
        private Tilemap _util;
        private Tile _hover;
        private Tile _select;
        private Vector3Int _previousMousePosition;
        private Vector3Int _previousSelection;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            _grid = gameObject.GetComponent<UnityEngine.Grid>();
            _util = Game.GetUtilTilemap();
            _hover = DataHolder.GetUtilTile("hover");
            _select = DataHolder.GetUtilTile("select");
        }

        private void Update()
        {
            var mousePosition = GetMousePosition();
            if (mousePosition != _previousMousePosition)
            {
                if (_previousMousePosition != _previousSelection)
                    _util.SetTile(_previousMousePosition, null);
                _util.SetTile(mousePosition, _hover);
                _previousMousePosition = mousePosition;
            }

            if (mousePosition != _previousSelection && Input.GetMouseButtonDown(0))
            {
                _util.SetTile(_previousSelection, null);
                _util.SetTile(mousePosition, _select);
                _previousSelection = mousePosition;
            }
        }

        private Vector3Int GetMousePosition()
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            return _grid.WorldToCell(mouseWorldPosition);
        }
    }
}