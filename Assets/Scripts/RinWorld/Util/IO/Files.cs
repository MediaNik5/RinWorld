using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RinWorld.Util.Exception;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Util.IO
{
    public static class Files
    {
        private static readonly char[] separatorChars = {'/', '\\'};
        public static void WriteContentsGameFolder(string localPath, string contents)
        {
            WriteContentsGlobal(Path.Combine(Application.dataPath, localPath), contents);
        }

        private static void WriteContentsGlobal(string globalPath, string contents)
        {
            File.WriteAllText(globalPath, contents);
        }

        /**
         * Read contents of file Path.Combine(Application.dataPath, path)
         */
        public static string ReadContentsGameFolder(string localPath)
        {
            return ReadContentsGlobal(Path.Combine(Application.dataPath, localPath));
        }

        public static string ReadContentsGlobal(string globalPath)
        {
            if (!File.Exists(globalPath))
                return null;

            return File.ReadAllText(globalPath);
        }

        public static IEnumerable<(string, Tile)> ReadTiles(string path, int tileSize)
        {
            foreach (var file in Directory.GetFiles(Path.Combine(Application.dataPath, path), "*.png"))
                yield return (ExtractName(file), ReadTile("", tileSize, file, true));
        }

        private static string ExtractName(string file)
        {
            var withoutPath = file.Substring(file.LastIndexOfAny(separatorChars) + 1);
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
                    new Rect(0, 0, texture2D.width, texture2D.height),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0U,
                    SpriteMeshType.FullRect
                );
            }
            catch (ArgumentException e)
            {
                throw new InvalidFileException(Path.Combine(path, name), e);
            }

            return tile;
        }

        public static Texture2D ReadTexture2D(string path, string name, int tileSize, bool isGlobalPath = false)
        {
            var texture2D = new Texture2D(tileSize, tileSize, TextureFormat.ARGB32, false);
            if (!name.EndsWith(".png"))
                name += ".png";

            byte[] bytes;
            if (isGlobalPath)
                bytes = GetBytesGlobal(Path.Combine(path, name));
            else
                bytes = GetBytesGameFolder(Path.Combine(path, name));
            texture2D.LoadImage(bytes);
            return texture2D;
        }
        public static byte[] GetBytesGameFolder(string localPath)
        {
            return GetBytesGlobal(Path.Combine(Application.dataPath, localPath));
        }

        private static byte[] GetBytesGlobal(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

        public static bool FileGameFolderExists(string localPath)
        {
            return FileGlobalExists(Path.Combine(Application.dataPath, localPath));
        }

        public static bool FileGlobalExists(string globalPath)
        {
            return File.Exists(globalPath);
        }
        /// <summary>
        /// Gets contents from all files in <c>folder</c> matching <c>format</c>
        /// </summary>
        /// <returns>Contents of all files</returns>
        public static string[] GetFilesGameFolder(string folder, string format)
        {
            return 
                Directory.GetFiles(
                    Path.Combine(Application.dataPath, folder), 
                    format
                ).Select(File.ReadAllText).ToArray();
        }
    }
}