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
        private static readonly Stopwatch Stopwatch;

        static Game()
        {
            Stopwatch = Stopwatch.StartNew();
        }
        public static GameState GameState
        {
            get => _gameState;
            private set
            {
                Stopwatch.Stop();
                Debug.Log(
                    $"The game completed {_gameState} and became {value} for {Stopwatch.ElapsedMilliseconds} ms.");
                Stopwatch.Reset();
                Stopwatch.Start();
                _gameState = value;
            }
        }

        private static Tilemap[] _tilemaps;

        public static int TilemapsLength =>
            _tilemaps.Length;

        public static World.World World { get; private set; }

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
            try
            {
                if (GameState != GameState.Preparing)
                    throw new InvalidStateException("The game is already launched.");

                GameState = GameState.LoadingConstants;
                DataHolder.Load();

                GameState = GameState.Loading;
                InitializeTilemaps();
                World = new World.World(250, 250, Random.Range(0, int.MaxValue));
                World.Initialize();
                World.StartRender(_tilemaps);
                GameState = GameState.Playing;
            }
            catch (System.Exception ex)
            {
                GameState = GameState.Corrupted;
                throw new System.Exception("Exception occured.", ex);
            }
        }

        private static void InitializeTilemaps()
        {
            _tilemaps = Object.FindObjectsOfType<Tilemap>();
            Array.Sort(_tilemaps,
                (e1, e2) => e2.GetComponent<TilemapRenderer>().sortOrder -
                            e1.GetComponent<TilemapRenderer>().sortOrder);
        }

        public static void SetPause(bool pause)
        {
            if (pause && GameState == GameState.Playing)
                GameState = GameState.Paused;
            else if (!pause && GameState == GameState.Paused)
                GameState = GameState.Playing;
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
        Paused,
        Corrupted
    }
}