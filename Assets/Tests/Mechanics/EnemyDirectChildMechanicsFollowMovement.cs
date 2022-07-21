using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class EnemyDirectChildMechanicsFollowMovement {
    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = Common.BaseMechanicData();

      List<MechanicEvent> events = new List<MechanicEvent>() {
        new SetEnemyAggro() {
          targetingScheme =
              new TargetSpecificPlayerIdsByClass() {
                classType = PlayerClassType.Tank,
                targetIds = new List<int>() { 0 },
              },
        },
      };
      for (int i = 0; i < 2; ++i) {
        events.AddRange(new MechanicEvent[] {
          new WaitEvent() {
            timeToWait = 5,
          },
          new SpawnMechanicEvent() {
            referenceMechanicName = "aoe",
            isPositionRelative = true,
          },
        });
      }

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

      mechanicData.referenceMechanicProperties[Common.BossMechanic] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events = events,
            },
      };

      return mechanicData;
    }
  }
}
