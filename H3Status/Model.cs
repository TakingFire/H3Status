#nullable enable

using System.Collections.Generic;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Serialization;
using Valve.Newtonsoft.Json.Converters;
using FistVR;

namespace H3Status.Model
{
    internal struct Event
    {
        public EventType Type { get; set; }
        public object? Status { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter), true)]
    internal enum EventType
    {
        Hello,
        SceneEvent,
        AmmoEvent,
        HealthEvent,
        BuffEvent,
        TNHLevelEvent,
        TNHPhaseEvent,
        TNHHoldPhaseEvent,
        TNHScoreEvent,
        TNHEncryptionDestroyed,
        TNHTokenEvent
    }

    internal struct VersionStatus
    {
        public string Version { get; set; }
        public string GameVersion { get; set; }
    }

    internal struct SceneStatus
    {
        public string Name { get; set; }
    }

    internal struct AmmoStatus
    {
        public string Weapon { get; set; }
        public FireArmRoundType RoundType { get; set; }
        public FireArmRoundClass RoundClass { get; set; }
        public int Hand { get; set; }

        public int Current { get; set; }
        public int Spent { get; set; }
        public int Capacity { get; set; }
    }

    internal struct HealthStatus
    {
        public float Change { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
    }

    internal struct BuffStatus
    {
        public PowerupType Type { get; set; }
        public float Duration { get; set; }
        public bool Inverted { get; set; }
    }

    internal struct TNHLevelStatus
    {
        public int Seed { get; set; }
        public int EquipmentSeed { get; set; }
        public string LevelName { get; set; }
        public string CharacterName { get; set; }
        public int ScoreMultiplier { get; set; }

        public TNHModifier_AIDifficulty AiDifficulty { get; set; }
        public TNHModifier_RadarMode RadarMode { get; set; }
        public TNHSetting_TargetMode TargetMode { get; set; }
        public TNHSetting_HealthMode HealthMode { get; set; }
        public TNHSetting_EquipmentMode EquipmentMode { get; set; }
    }

    internal struct TNHPhaseStatus
    {
        public TNH_Phase Phase { get; set; }
        public int Level { get; set; }
        public int Count { get; set; }
        public int Seed { get; set; }

        public int Hold { get; set; }
        public List<int> Supply { get; set; }

        public string? HoldName { get; set; }
        public List<string>? SupplyNames { get; set; }
    }

    internal struct TNHHoldPhaseStatus
    {
        public TNH_HoldPoint.HoldState Phase { get; set; }
        public int Level { get; set; }
        public int Count { get; set; }

        public TNH_EncryptionType EncryptionType { get; set; }
        public int EncryptionCount { get; set; }
        public float EncryptionTime { get; set; }
    }

    internal struct TNHScoreStatus
    {
        public TNH_Manager.ScoringEvent Type { get; set; }
        public int Value { get; set; }
        public int Mult { get; set; }
        public int Score { get; set; }
    }

    internal struct TNHTokenStatus
    {
        public int Change { get; set; }
        public int Tokens { get; set; }
    }
}
