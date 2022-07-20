using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class TargetTanks {
    private const float _arenaRadius = 46.83f / 2;
    private const string _mechanicArenaBoundary = "MechanicArenaBoundary";
    private const string _spawnBoss = "SpawnBoss";
    private const string _targetTanks = "TargetTanks";
    private const string _targetedAoe = "TargetedAoe";
    private const string _statusVuln = "Vuln";
    private const float _castDuration = 1;
    private const string _damageType = "Fire";

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();
      mechanicData.referenceTetherProperties = new Dictionary<string, TetherProperties>();
      mechanicData.mechanicPools = new Dictionary<string, List<MechanicEvent>>();
      mechanicData.referenceStatusProperties = new Dictionary<string, StatusEffectData>();
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
      mechanicData.referenceMechanicProperties[_targetedAoe] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(1, 360, 0, 0),
        mechanic =
            new ApplyEffectToPlayers() {
              effects =
                  new List<MechanicEffect>() {
                    new DamageEffect() {
                      damageAmount = 1,
                      damageType = _damageType,
                    },
                    new ApplyStatusEffect() {
                      referenceStatusName = _statusVuln,
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties[_targetTanks] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new WaitEvent() {
                      timeToWait = _castDuration,
                    },
                    new ReshufflePlayerIds(),
                    new SpawnTargetedEvents() {
                      targetingScheme =
                          new TargetSpecificPlayerIdsByClass() {
                            classType = PlayerClassType.Tank,
                            targetIds =
                                new List<int>() {
                                  0,
                                  1,
                                  2,
                                  3,
                                  4,
                                  5,
                                  6,
                                },
                          },
                      spawnOnTarget = true,
                      referenceMechanicName = _targetedAoe,
                    },
                  },
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
          referenceMechanicName = _targetTanks,
        },
      };
      mechanicData.referenceStatusProperties[_statusVuln] = new DamageModifier() {
        damageMultiplier = 100000,
        damageType = _damageType,
        statusName = _statusVuln,
        duration = 3,
      };
      return mechanicData;
    }
  }
}
