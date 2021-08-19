using RimCopy.Data;
using UnityEngine;

namespace RimCopy.Unity.Grid
{
    public class CameraMover : MonoBehaviour
    {
        private UnityEngine.Grid _grid;

        private const string MoveMouse = "MoveMapByMouse";
        private const int MinSize = 10;
        private const int MaxSize = 1000;
        private Camera _camera;
        private float _currentSize;

        private Vector3 _previousMousePosition;
        private Vector3 _previousCameraPosition;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _grid = gameObject.GetComponent<UnityEngine.Grid>();
            _currentSize = _camera.orthographicSize;
        }

        private void Update()
        {
            if (Input.GetKeyDown(DataHolder.GetKeyCode(MoveMouse)))
                InitMovingTowardsMousePointer();

            if (Input.GetKey(DataHolder.GetKeyCode(MoveMouse)))
                MoveTowardsMousePointer();

            var zoom = Input.mouseScrollDelta.y;
            if (zoom != 0f)
                ZoomTowardsMousePointer(_camera.ScreenToWorldPoint(Input.mousePosition), zoom * _currentSize / 10);
        }

        private void InitMovingTowardsMousePointer()
        {
            _previousMousePosition = Input.mousePosition;
            _previousCameraPosition = transform.position;
        }

        private void MoveTowardsMousePointer()
        {
            var currentMousePosition = Input.mousePosition;
            transform.position = _previousCameraPosition +
                                 _camera.ScreenToWorldPoint(_previousMousePosition - currentMousePosition) -
                                 _camera.ScreenToWorldPoint(Vector3.zero);
        }

        // Thanks to MasterKelli from https://answers.unity.com/questions/384753/ortho-camera-zoom-to-mouse-point.html
        private void ZoomTowardsMousePointer(Vector3 zoomTowards, float amount)
        {
            // dont move if its max or min
            if (_currentSize == MaxSize && amount < 0)
                return;
            if (_currentSize == MinSize && amount > 0)
                return;

            // Calculate how much we will have to move towards the zoomTowards position
            var multiplier = 1.0f / _currentSize * amount;

            // Move camera
            var transform = this.transform;
            var position = transform.position;
            position += (zoomTowards - position) * multiplier;
            transform.position = position;

            // Zoom camera
            _currentSize -= amount;

            // Limit zoom
            _currentSize = Mathf.Clamp(_currentSize, MinSize, MaxSize);
            _camera.orthographicSize = _currentSize;
        }
    }
}