using System.Collections.Generic;
using RinWorld.Entities.Body;
using RinWorld.Util;
using RinWorld.Util.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace RinWorld.Entities
{
    public class Entity : UnitPreset, ITickable
    {
        public const int Layer = 3;
        protected readonly Dictionary<string, BodyPart> bodyParts = new Dictionary<string, BodyPart>();
        protected Vector2 position = Vector2.zero;
        protected Slider.Direction _direction = Slider.Direction.TopToBottom;

        public ulong LifeTime { get; private set;  }
        protected ulong PreviousTick { get; private set; }
        public void Tick(ulong tick)
        {
            if(PreviousTick + 1 != tick)
                Debug.LogWarning($"Ticks skipped: {tick-PreviousTick} on {this}");
            PreviousTick = tick;
            LifeTime++;
        }
        public void Display(Tilemap[] tilemaps)
        {
            foreach (var bodyPart in bodyParts.Values)
            {
                tilemaps[Layer].SetTile(Utils.ToVector3Int(position), bodyPart._bodyPartPreset.Tile, _direction);
            }
        }
        protected Entity(string name) : base(name)
        {
            LifeTime++;
        }
    }
}