using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Pantheon
{
    public static class XivSimParser
    {
        public class MechanicData
        {
            public Dictionary<string, MechanicProperties> referenceMechanicProperties;
            public Dictionary<string, TetherProperties> referenceTetherProperties;
            public Dictionary<string, List<MechanicEvent>> mechanicPools;
            public Dictionary<string, StatusEffectData> referenceStatusProperties;
            public List<MechanicEvent> mechanicEvents;

            [DefaultValue(130000)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public float defaultHealth;
        }

        public abstract class MechanicEvent { }

        public class MechanicProperties
        {
            public bool isTerrain;
            public bool isTargeted;
            public float followSpeed;

            [DefaultValue(CollisionShape.Unspecified)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public CollisionShape collisionShape;

            public Vector4 collisionShapeParams;
            public string colorHtml;

            [DefaultValue(true)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public bool visible;

            public float persistentTickInterval;
            public float persistentActivationDelay;
            public MechanicEvent mechanic;
            public MechanicEvent persistentMechanic;
            public List<int> staticTargetIds;
            public string mechanicTag;
        }

        public class ExecuteMultipleEvents : MechanicEvent
        {
            public List<MechanicEvent> events;
        }

        public class ExecuteRandomEvents : MechanicEvent
        {
            public string mechanicPoolName;

            [DefaultValue(1)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public int numberToSpawn;
        }

        public class SpawnEnemy : MechanicEvent
        {
            public string enemyName;
            public string textureFilePath;
            public string colorHtml;
            public int maxHp;
            public float baseMoveSpeed;
            public float hitboxSize;
            public float rotation;
            public bool isPositionRelative;
            public bool isRotationRelative;
            public bool isTargetable;
            public bool showInEnemyList;
            public bool isVisible;
            public Vector3 visualPosition;
            public Vector3 visualScale;
            public string referenceMechanicName;
            public string deathMechanicName;
            public Vector2 position;
        }

        public class SpawnMechanicEvent : MechanicEvent
        {
            public string referenceMechanicName;
            public Vector2 position;
            public float rotation;
            public bool isPositionRelative;
            public bool isRotationRelative;
            public bool resetMechanicDepth;
        }

        public class SpawnVisualObject : MechanicEvent
        {
            public string textureFilePath;
            public Vector3 relativePosition;
            public Vector3 eulerAngles;
            public Vector3 scale;

            [DefaultValue("#000000")]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public string colorHtml;

            public bool spawnOnPlayer;
            public bool isRotationRelative;
            public bool isBillboard;
            public float visualDuration;
        }

        public class StartCastBar : MechanicEvent
        {
            public string castName;
            public float duration;
        }

        public class WaitEvent : MechanicEvent
        {
            public float timeToWait;
            public string expressionFormat;
        }

        public class ReshufflePlayerIds : MechanicEvent { }

        public class SpawnTargetedEvents : MechanicEvent
        {
            public TargetingScheme targetingScheme;
            public string referenceMechanicName;
            public Vector2 position;
            public float rotation;
            public bool isPositionRelative;
            public bool isRotationRelative;
            public bool spawnOnTarget;
            public bool resetMechanicDepth;
        }

        public class ApplyEffectToPlayers : MechanicEvent
        {
            [DefaultValue(true)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public bool affectLivingOnly;

            public MechanicEffect effect;
            public List<MechanicEffect> effects;
            public Condition condition;
            public MechanicEffect failedConditionEffect;
            public List<MechanicEffect> failedConditionEffects;
        }

        public abstract class MechanicEffect { }

        public class DamageEffect : MechanicEffect
        {
            public string name;
            public string damageType;
            public float damageAmount;

            [DefaultValue(1)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public int maxStackAmount;
        }

        public abstract class TargetingScheme
        {
            public Condition targetCondition;
            public TargetingScheme fallbackTargetingScheme;
            public bool manuallySetTargetsNeeded;
            public int totalTargetsNeeded;
            public bool dropExtraEvents;

            [DefaultValue(true)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            public bool targetLivingOnly;
        }

        public class TargetSpecificPlayerIds : TargetingScheme
        {
            public List<int> targetIds;
        }

        public class TetherProperties { }

        public enum CollisionShape
        {
            Unspecified,
            Round,
            Rectangle
        }

        public class StatusEffectData { }

        public abstract class Condition { }

        private class TypeBinder : ISerializationBinder
        {
            public Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType($"Pantheon.XivSimParser+{typeName}");
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }

        public static MechanicData Parse(string json)
        {
            return JsonConvert.DeserializeObject<MechanicData>(json, new JsonSerializerSettings()
            {
                SerializationBinder = new TypeBinder(),
                TypeNameHandling = TypeNameHandling.All,
            });
        }
    }
}