using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RinWorld.Characteristics;
using RinWorld.Util;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;
using Types = RinWorld.Util.Reflection.Types;

namespace RinWorld.Entities.Body
{
    public class BodyPartPreset : UnitPreset
    {
        private static readonly StatePreset AllKey = StatePreset.Dummy;
        public float MaxDurability { get; }
        /// <summary>
        /// Expected number of this body part inside the body.
        /// <example>
        /// You expect to be one peace of head in body.
        /// </example>
        /// </summary>
        public int NormalNumber { get; }
        /// <summary>
        /// Reserved for future body parts. You usually have, for example, 2 hands,
        /// but imagine, that in theory in the future it's ok to have 2 other bionic hands.
        /// So you might have 4 hands max available, but 2 hands as normal number. 
        /// </summary>
        public int MaxAvailableNumber { get; }
        [CanBeNull] public ImmutableTile Tile { get; }
        
        private readonly Dictionary<string, float> _influencedBodyParts;
        private readonly string _jointedToString;
        private BodyPartPreset _jointedToBodyPartPreset;

        public BodyPartPreset JointedTo =>
            _jointedToBodyPartPreset ??= (BodyPartPreset)DataHolder.GetUnit(_jointedToString);

        protected BodyPartPreset(
            string name,
            float maxDurability,
            int normalNumber,
            int maxAvailableNumber,
            string jointedTo,
            [CanBeNull] ImmutableTile tile,
            [NotNull] Dictionary<string, float> influencedBodyParts
        ) : base(name)
        {
            MaxDurability = maxDurability;
            NormalNumber = normalNumber;
            MaxAvailableNumber = maxAvailableNumber;
            _jointedToString = jointedTo;
            Tile = tile;
            _influencedBodyParts = influencedBodyParts;
        }
        /// <summary>
        /// Amount of influence that this body part affects to statePreset.
        ///
        /// <example>
        /// Let us have organ <c>organ</c> and <c>statePreset</c>.
        /// And if <c>organ.Influence(statePreset)</c> is equal to <c>a</c>, then if <c>organ</c> gets hurt and now
        /// has b of its max durability then <c>statePreset</c>'s Durability should be
        /// <c>Durability*(1 - (1 - b)*a)</c>, or dealt damage should be <c>Damage*(1 - b)*a</c>
        /// </example>
        /// </summary>
        /// <param name="statePreset">Some state preset</param>
        /// <returns>influence of this body part to statePreset in range [0, 1]</returns>
        public float Influence(StatePreset statePreset)
        {
            if (_influencedBodyParts.ContainsKey(statePreset.Name))
                return _influencedBodyParts[statePreset.Name];
            if (_influencedBodyParts.ContainsKey(AllKey.Name))
                return _influencedBodyParts[AllKey.Name];
            return 0;
        }
        

        // ReSharper disable once UnusedMember.Local
        private new static UnitPreset Of(JObject jBodyPart)
        {
            ImmutableTile tile = null;
            if (jBodyPart.ContainsKey("tile"))
                tile = ImmutableTile.Of(jBodyPart["tile"]);
            
            return new BodyPartPreset(
                jBodyPart["name"].Value<string>(),
                jBodyPart["maxDurability"].Value<float>(),
                jBodyPart["normalNumber"].Value<int>(),
                jBodyPart["maxAvailableNumber"].Value<int>(),
                jBodyPart["jointedTo"].Value<string>(),
                tile, 
                Utils.extractDictionary<string, float>(jBodyPart["influences"] as JObject, s => s)
            );
        }
    }
}