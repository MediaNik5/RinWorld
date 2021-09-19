using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Util
{
    public static class Types
    {
        public static System.Type[] GetAllSubtypes<Type>()
        {
            return GetAllSubtypes(typeof(Type));
        }
        public static Type[] GetAllSubtypes(Type superType)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where superType.IsInterface ? 
                        type.GetInterface(superType.FullName) != null : 
                        type.IsSubclassOf(superType)
                    select type).ToArray();
        }
        [CanBeNull]
        public static MethodInfo GetMethodOrNull(
            Type clazz, 
            string name, 
            BindingFlags flags, 
            Type returnType, 
            params Type[] parameters
        )
        {
            var method = clazz.GetMethod(name, flags);

            if (method == null ||
                method.ReturnType != returnType ||
                method.GetParameters().Length != parameters.Length ||
                !method.GetParameters().Select(info => info.ParameterType).SequenceEqual(parameters))
            {
                return null;
            }

            return method;
        }

        public static T CreateStaticDelegate<T>(MethodInfo method) where T : class
        {
            return Delegate.CreateDelegate(typeof(T), null, method) as T;
        }
        
        
        public static Tile DefaultTile
        {
            get
            {
                Tile defaultTile = ScriptableObject.CreateInstance<Tile>();
                var texture2D = new Texture2D(128, 128);
                for (var x = 0; x < 128; x++)
                {
                    for (var y = 0; y < 128; y++)
                    {
                        var color = x / 128 + y / 128;
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
    }
}