using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class DelayedProteanMiss {
    public const float Duration = _castDuration + _delay + _effectDuration + 1;

    private const float _arenaRadius = 46.83f / 2;
    private const string _mechanicVisible = "MechanicVisible";
    private const string _mechanicNonTargeted = "MechanicNonTargeted";
    private const string _mechanicArenaBoundary = "MechanicArenaBoundary";
    private const string _spawnBoss = "SpawnBoss";
    private const string _delayedProtean = "DelayedProtean";
    private const string _delayedProteanTargeted = "DelayedProteanTargeted";
    private const float _castDuration = 1;
    private const float _delay = 1;
    private const float _effectDuration = 1;

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();
      mechanicData.referenceTetherProperties = new Dictionary<string, TetherProperties>();
      mechanicData.mechanicPools = new Dictionary<string, List<MechanicEvent>>();
      mechanicData.referenceStatusProperties = new Dictionary<string, StatusEffectData>();
      mechanicData.mechanicEvents = new List<MechanicEvent>();
      mechanicData.defaultHealth = 50000;
      mechanicData.referenceMechanicProperties[_mechanicArenaBoundary] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(100, 360, _arenaRadius, 0),
        persistentTickInterval = 0.2f,
        persistentMechanic =
            new ApplyEffectToPlayers() {
              effect =
                  new DamageEffect() {
                    damageAmount = 9999999,
                  },
            },
      };
      mechanicData.referenceMechanicProperties[_delayedProtean] = new MechanicProperties() {
        mechanic =
            new SpawnTargetedEvents() {
              referenceMechanicName = _delayedProteanTargeted,
              targetingScheme = new TargetAllPlayers(),
            },
      };
      mechanicData.referenceMechanicProperties[_delayedProteanTargeted] = new MechanicProperties() {
        visible = false,
        isTargeted = true,
        followSpeed = 0,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(100, 24, 0, 0),
        colorHtml = "#ff0000",
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new WaitEvent() {
                      timeToWait = _castDuration,
                    },
                    new ModifyMechanicEvent() {
                      referenceMechanicName = _mechanicNonTargeted,
                    },
                    new WaitEvent() {
                      timeToWait = _delay,
                    },
                    new ModifyMechanicEvent() {
                      referenceMechanicName = _mechanicVisible,
                    },
                    new ApplyEffectToPlayers() {
                      effect =
                          new DamageEffect() {
                            damageAmount = 100000,
                          },
                    },
                    new WaitEvent() {
                      timeToWait = _effectDuration,
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties[_mechanicVisible] = new MechanicProperties() {
        visible = true,
      };
      mechanicData.referenceMechanicProperties[_mechanicNonTargeted] = new MechanicProperties() {
        isTargeted = false,
      };
      mechanicData.referenceMechanicProperties[_spawnBoss] = new MechanicProperties() {
        visible = false,
        mechanic =
            new SpawnEnemy() {
              enemyName = "Boss",
              textureFilePath = "Mechanics/Resources/Thordan.png",
              colorHtml = "#000000",
              maxHp = 1000000,
              baseMoveSpeed = 2,
              hitboxSize = 3,
              isTargetable = true,
              visualPosition = new Vector3(0, 2, 0),
              visualScale = new Vector3(4, 4, 4),
              referenceMechanicName = _delayedProtean,
              position = new Vector2(0, 0),
            },
      };
      mechanicData.mechanicEvents = new List<MechanicEvent>() {
        new SpawnVisualObject() {
          textureFilePath = "Mechanics/Resources/ArenaCircle.png",
          relativePosition = new Vector3(0, -0.001f, 0),
          eulerAngles = new Vector3(90, 0, 0),
          scale = new Vector3(_arenaRadius * 2, _arenaRadius * 2, 1),
          visualDuration = float.PositiveInfinity,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = _spawnBoss,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = _mechanicArenaBoundary,
        },
      };
      return mechanicData;
    }
  }
}
