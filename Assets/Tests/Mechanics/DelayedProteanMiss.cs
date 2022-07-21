using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Test.Mechanics {
  public static class DelayedProteanMiss {
    public const float Duration = _castDuration + _delay + _effectDuration + 1;

    private const string _delayedProteanTargeted = "DelayedProteanTargeted";
    private const float _castDuration = 1;
    private const float _delay = 1;
    private const float _effectDuration = 1;

    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = Common.BaseMechanicData();
      mechanicData.referenceMechanicProperties[Common.BossMechanic] = new MechanicProperties() {
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
                      referenceMechanicName = Common.MechanicNonTargeted,
                    },
                    new WaitEvent() {
                      timeToWait = _delay,
                    },
                    new ModifyMechanicEvent() {
                      referenceMechanicName = Common.MechanicVisible,
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
      return mechanicData;
    }
  }
}
