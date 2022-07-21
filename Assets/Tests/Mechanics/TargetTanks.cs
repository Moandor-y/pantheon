using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class TargetTanks {
    private const string _targetedAoe = "TargetedAoe";
    private const string _statusVuln = "Vuln";
    private const float _castDuration = 1;
    private const string _damageType = "Fire";

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = Common.BaseMechanicData();
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
      mechanicData.referenceMechanicProperties[Common.BossMechanic] = new MechanicProperties() {
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
