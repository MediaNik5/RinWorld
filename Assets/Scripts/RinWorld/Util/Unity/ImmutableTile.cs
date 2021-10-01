using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RinWorld.Util.Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace RinWorld.Util.Unity
{
    public class ImmutableTile
    {
        private readonly Tile topToBottom;

        public ImmutableTile(Tile tile)
        {
            topToBottom = tile;
        }
        public ImmutableTile(ImmutableTile tile)
        {
            topToBottom = tile.topToBottom;
        }

        public void ApplyFor(Tilemap tilemap, Vector3Int position)
        {
            tilemap.SetTile(position, topToBottom);
        }
        public static ImmutableTile Of(JToken jToken)
        {
            if (jToken is JObject jObject)
                return ImmutableRotatingTile.Of(jObject);

            return DataHolder.GetTile(jToken.Value<string>());
        }
    }

    public class ImmutableRotatingTile : ImmutableTile
    {
        private readonly Tile _rightToLeft;
        private readonly Tile _leftToRight;
        private readonly Tile _bottomToTop;
        private readonly ImmutableTile _rightToLeftImmutable;
        private readonly ImmutableTile _leftToRightImmutable;
        private readonly ImmutableTile _bottomToTopImmutable;

        public ImmutableRotatingTile(Tile topToBottom, Tile rightToLeft, Tile leftToRight, Tile bottomToTop) : base(topToBottom)
        {
            _rightToLeft = rightToLeft;
            _leftToRight = leftToRight;
            _bottomToTop = bottomToTop;
        }
        private ImmutableRotatingTile(ImmutableTile topToBottom, ImmutableTile rightToLeft, ImmutableTile leftToRight, ImmutableTile bottomToTop) : base(topToBottom)
        {
            _rightToLeftImmutable = rightToLeft;
            _leftToRightImmutable = leftToRight;
            _bottomToTopImmutable = bottomToTop;
        }
        public void ApplyFor(Tilemap tilemap, Vector3Int position, Slider.Direction direction)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch(direction)
            {
                case Slider.Direction.LeftToRight:
                    if (!ReferenceEquals(_leftToRight, null))
                        tilemap.SetTile(position, _leftToRight);
                    else
                        tilemap.SetTile(position, _leftToRightImmutable);
                    break;
                case Slider.Direction.RightToLeft:
                    if (!ReferenceEquals(_rightToLeft, null))
                        tilemap.SetTile(position, _rightToLeft);
                    else
                        tilemap.SetTile(position, _rightToLeftImmutable);
                    break;
                case Slider.Direction.BottomToTop:
                    if (!ReferenceEquals(_bottomToTop, null))
                        tilemap.SetTile(position, _bottomToTop);
                    else
                        tilemap.SetTile(position, _bottomToTopImmutable);
                    break;
                case Slider.Direction.TopToBottom:
                    base.ApplyFor(tilemap, position);
                    break;
            }
        }
        public static ImmutableRotatingTile Of(JObject jObject)
        {
            return new ImmutableRotatingTile(
                DataHolder.GetTile(jObject["topToBottom"].Value<string>()),
                DataHolder.GetTile(jObject["rightToLeft"]?.Value<string>()),
                DataHolder.GetTile(jObject["leftToRight"]?.Value<string>()),
                DataHolder.GetTile(jObject["bottomToTop"]?.Value<string>())
            );
        }
    }
    public static class ImmutableTileExtension
    {
        public static void SetTile(this Tilemap tilemap, Vector3Int position, [CanBeNull] ImmutableTile tile, Slider.Direction direction = Slider.Direction.TopToBottom)
        {
            if (tile is ImmutableRotatingTile rotatingTile)
                rotatingTile.ApplyFor(tilemap, position, direction);
            else tile?.ApplyFor(tilemap, position);
        }
        public static void SetTile(this Tilemap tilemap, int x, int y, [CanBeNull] ImmutableTile tile, Slider.Direction direction = Slider.Direction.TopToBottom)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile, direction);
        }
    }
}