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
        private Vector3Int _previousMousePosition;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            _grid = gameObject.GetComponent<UnityEngine.Grid>();
            _util = Game.GetUtilTilemap();
            _hover = DataHolder.getUtilTile("hover");
        }

        private void Update()
        {
            var mousePosition = GetMousePosition();
            if (mousePosition == _previousMousePosition)
                return;
            _util.SetTile(_previousMousePosition, null);
            _util.SetTile(mousePosition, _hover);
            _previousMousePosition = mousePosition;
        }

        private Vector3Int GetMousePosition()
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            return _grid.LocalToCell(mouseWorldPosition);
        }
    }
}