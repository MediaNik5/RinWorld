using System;
using System.Diagnostics;
using RimCopy.Data;
using RimCopy.Exception;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RimCopy
{
    public static class Game
    {
        private static GameState _gameState = GameState.Preparing;
        private static Stopwatch _stopwatch;

        public static GameState GameState
        {
            get => _gameState;
            private set
            {
                if (_stopwatch != null)
                {
                    _stopwatch.Stop();
                    Debug.Log(
                        $"The game completed {_gameState} and become {value} for {_stopwatch.ElapsedMilliseconds} ms.");
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }
                else
                {
                    _stopwatch = Stopwatch.StartNew();
                }

                _gameState = value;
            }
        }

        private static World.World _world;
        private static Tilemap[] _tilemaps;

        public static int TilemapsLength =>
            _tilemaps.Length;

        public static Tilemap GetUtilTilemap()
        {
            return _tilemaps[_tilemaps.Length - 1];
        }

        public static Tilemap GetTilemap(int index)
        {
            return _tilemaps[index];
        }

        public static void StartGame()
        {
            if (GameState != GameState.Preparing)
                throw new InvalidStateException("The game is already launched.");
            
            GameState = GameState.LoadingConstants;
            DataHolder.ConstantLoader.Load();

            GameState = GameState.Loading;
            InitializeTilemaps();
            _world = new World.World(100, 100, Random.Range(0, int.MaxValue));
            _world.Initialize();
            _world.StartRender(_tilemaps);
            GameState = GameState.Playing;
        }

        private static void InitializeTilemaps()
        {
            _tilemaps = Object.FindObjectsOfType<Tilemap>();
            Array.Sort(_tilemaps,
                (e1, e2) => e2.GetComponent<TilemapRenderer>().sortOrder -
                            e1.GetComponent<TilemapRenderer>().sortOrder);
        }
    }

    public enum GameState
    {
        Preparing,
        LoadingConstants,
        Menu,
        Loading,
        Saving,
        Playing,
        Paused
    }
}