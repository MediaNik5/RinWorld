using System;
using System.Collections.Generic;
using System.IO;
using RimCopy.Exception;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.IO
{
    public static class Files
    {
        public static void WriteContentsGameFolder(string localPath, string contents)
        {
            WriteContentsGlobal(Application.dataPath + localPath, contents);
        }

        private static void WriteContentsGlobal(string globalPath, string contents)
        {
            File.WriteAllText(globalPath, contents);
        }

        /**
         * Read contents of file Application.dataPath + path
         */
        public static string ReadContentsGameFolder(string localPath)
        {
            return ReadContentsGlobal(Application.dataPath + localPath);
        }

        public static string ReadContentsGlobal(string globalPath)
        {
            if (!File.Exists(globalPath))
                return null;

            return File.ReadAllText(globalPath);
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
            try
            {
                tile.sprite = Sprite.Create(
                    texture2D,
                    new Rect(0, 0, tileSize, tileSize),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0U,
                    SpriteMeshType.FullRect
                );
            }
            catch (ArgumentException e)
            {
                throw new InvalidFileException(path + name, e);
            }
            
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

        public static byte[] GetBytesGameFolder(string localPath)
        {
            return GetBytesGlobal(Application.dataPath + localPath);
        }

        private static byte[] GetBytesGlobal(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

        public static bool FileGameFolderExists(string localPath)
        {
            return FileGlobalExists(Application.dataPath + localPath);
        }

        public static bool FileGlobalExists(string globalPath)
        {
            return File.Exists(globalPath);
        }
    }
}