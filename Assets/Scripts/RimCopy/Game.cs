using System;
using RimCopy.Data;
using RimCopy.Exception;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RimCopy
{
    public static class Game
    {
        public static GameState GameState { get; private set; } = GameState.Preparing;

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