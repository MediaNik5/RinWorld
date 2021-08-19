using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.IO
{
    public static class Files
    {
        /**
         * Read contents of file Application.dataPath + path
         */
        public static string ReadContentsGameFolder(string path)
        {
            return ReadContentsGlobal(Application.dataPath + path);
        }

        public static string ReadContentsGlobal(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllText(path);
        }

        public static IEnumerable<(string, Tile)> ReadTiles(string path, int tileSize)
        {
            foreach (var file in Directory.GetFiles(Application.dataPath + path, "*.png"))
                yield return (ExtractName(file), ReadTile("", tileSize, file, true));
        }

        private static string ExtractName(string file)
        {
            var withoutPath = file.Substring(file.LastIndexOf('/') + 1);
            var name = withoutPath.Substring(0, withoutPath.LastIndexOf('.'));
            return name;
        }

        public static Tile ReadTile(string path, int tileSize, string name, bool isGlobalPath = false)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();

            var texture2D = ReadTexture2D(path, name, tileSize, isGlobalPath);
            tile.sprite = Sprite.Create(
                texture2D,
                new Rect(0, 0, tileSize, tileSize),
                new Vector2(0.5f, 0.5f),
                100f,
                0U,
                SpriteMeshType.FullRect);
            return tile;
        }

        public static Texture2D ReadTexture2D(string path, string name, int tileSize, bool isGlobalPath = false)
        {
            var texture2D = new Texture2D(tileSize, tileSize);
            if (!name.EndsWith(".png"))
                name += ".png";

            byte[] bytes;
            if (isGlobalPath)
                bytes = GetBytesGlobal(path + name);
            else
                bytes = GetBytesGameFolder(path + name);
            texture2D.LoadImage(bytes);
            return texture2D;
        }

        public static byte[] GetBytesGameFolder(string path)
        {
            return GetBytesGlobal(Application.dataPath + path);
        }

        private static byte[] GetBytesGlobal(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }
    }
}