using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Util
{
    public static class Utils
    {

        public static Tile DefaultTile
        {
            get
            {
                var defaultTile = ScriptableObject.CreateInstance<Tile>();
                var texture2D = new Texture2D(128, 128);
                for (int x = 0; x < 128; x++)
                {
                    for (int y = 0; y < 128; y++)
                    {
                        int color = x / 128 + y / 128;
                        texture2D.SetPixel(x, y, color % 2 == 1 ? Color.black : Color.yellow);
                    }
                }

                texture2D.Apply();
                defaultTile.sprite = Sprite.Create(
                    texture2D,
                    new Rect(0, 0, 128, 128),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0U,
                    SpriteMeshType.FullRect
                );
                return defaultTile;
            } 
                
        }

        public static Dictionary<K, V> extractDictionary<K, V>(JObject jObject, Func<string, K> transformKey)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>(jObject.Count);
            foreach (var keyValuePair in jObject)
                dictionary.Add(transformKey(keyValuePair.Key), keyValuePair.Value.Value<V>());
            return dictionary;
        }
        public static Vector3Int ToVector3Int(Vector2 position)
        {
            return new Vector3Int((int) (0.5 + position.x), (int) (0.5 + position.y), 0);
        }
    }
}