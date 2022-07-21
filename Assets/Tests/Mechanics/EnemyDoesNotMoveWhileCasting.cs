
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class EnemyDoesNotMoveWhileCasting {
    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = Common.BaseMechanicData();

      mechanicData.referenceMechanicProperties[Common.BossMechanic] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SetEnemyAggro() {
                      targetingScheme =
                          new TargetSpecificPlayerIds() {
                            targetIds = new List<int>() { 0 },
                          },
                    },
                    new StartCastBar() {
                      castName = "aoe",
                      duration = 5,
                    },
                    new WaitEvent() {
                      timeToWait = 5,
                    },
                    new SpawnMechanicEvent() {
                      isPositionRelative = true,
                      referenceMechanicName = "aoe",
                    },
                  },
            },
      };

      mechanicData.referenceMechanicProperties["aoe"] = new MechanicProperties() {
        visible = true,
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(5, 360, 0, 0),
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new WaitEvent() {
                      timeToWait = 1,
                    },
                    new ApplyEffectToPlayers() {
                      effect =
                          new DamageEffect() {
                            damageAmount = 1,
                            damageType = "Fire",
                          },
                    },
                  },
            },
      };

      return mechanicData;
    }
  }
}
