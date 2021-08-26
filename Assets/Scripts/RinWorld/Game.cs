using System;
using System.Diagnostics;
using RinWorld.Util.Data;
using RinWorld.Util.Exception;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace RinWorld
{
    public static class Game
    {
        private static GameState _gameState = GameState.Preparing;
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        private static Tilemap[] _tilemaps;

        public static GameState GameState
        {
            get => _gameState;
            private set
            {
                Stopwatch.Stop();
                Debug.Log($"The game switched from {_gameState} to {value} for {Stopwatch.ElapsedMilliseconds} ms.");
                Stopwatch.Reset();
                Stopwatch.Start();
                _gameState = value;
            }
        }

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

        public static void LaunchGame()
        {
            try
            {
                if (GameState != GameState.Preparing)
                    throw new InvalidStateException("The game is already launched.");

                GameState = GameState.LoadingConstants;
                DataHolder.Load();
                GameState = GameState.Menu;
            }
            catch (Exception ex)
            {
                GameState = GameState.Corrupted;
                throw new Exception("Exception occurred.", ex);
            }
        }

        public static void StartGame(int seed)
        {
            try
            {
                if (GameState != GameState.Menu)
                    throw new InvalidStateException("The game is already in game");

                GameState = GameState.Loading;
                SceneManager.LoadSceneAsync("Scenes/World").completed +=
                    _ => GenerateWorld(seed);
            }
            catch (Exception ex)
            {
                GameState = GameState.Corrupted;
                throw new Exception("Exception occurred.", ex);
            }
        }

        private static void GenerateWorld(int seed)
        {
            InitializeTilemaps();

            World = new World.World(250, 250, seed);
            World.Initialize();
            World.StartRender(_tilemaps);
            GameState = GameState.Playing;
        }

        public static void StartColony(int x, int y)
        {
            try
            {
                if (GameState != GameState.Playing)
                    throw new InvalidStateException("The game is not playing.");
                if (World == null)
                    throw new InvalidStateException("The game does not have generated world");

                GameState = GameState.Loading;
                SceneManager.LoadSceneAsync("Scenes/Map").completed += _ => GenerateColony(x, y);
            }
            catch (Exception ex)
            {
                GameState = GameState.Corrupted;
                throw new Exception("Exception occurred.", ex);
            }
        }

        private static void GenerateColony(int x, int y)
        {
            InitializeTilemaps();

            var colony = World.GenerateColony(x, y);
            colony.StartRender(_tilemaps);
            GameState = GameState.Playing;
        }

        private static void InitializeTilemaps()
        {
            _tilemaps = Object.FindObjectsOfType<Tilemap>();
            Array.Sort(_tilemaps, (e1, e2) =>
                e1.GetComponent<TilemapRenderer>().sortingOrder -
                e2.GetComponent<TilemapRenderer>().sortingOrder);
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