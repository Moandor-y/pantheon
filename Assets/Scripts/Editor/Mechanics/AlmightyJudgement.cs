using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Pantheon.XivSimParser;

namespace Pantheon.Mechanics {
  public static class AlmightyJudgement {
    public static MechanicData GetMechanicData() {
      MechanicData mechanicData = new MechanicData();
      mechanicData.referenceMechanicProperties = new Dictionary<string, MechanicProperties>();
      mechanicData.referenceMechanicProperties["SpawnAlex"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new SpawnEnemy() {
              enemyName = "Alexander Prime",
              textureFilePath = "Mechanics/Resources/AlexPrime.png",
              colorHtml = "#8b4800",
              maxHp = 2147483647,
              baseMoveSpeed = 2,
              hitboxSize = 3,
              isTargetable = false,
              visualPosition = new Vector3(0, 2, 0),
              visualScale = new Vector3(4, 4, 4),
              referenceMechanicName = "AlexMechanics",
              position = new Vector2(0, 0),
            },
      };
      mechanicData.referenceMechanicProperties["AlexMechanics"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new WaitEvent() {
                      timeToWait = 1,
                    },
                    new StartCastBar() {
                      castName = "Almighty Judgement",
                      duration = 2.7f,
                    },
                    new WaitEvent() {
                      timeToWait = 6.1f,
                    },
                    new ExecuteRandomEvents() {
                      mechanicPoolName = "AlmightyJudgementPool",
                    },
                    new WaitEvent() {
                      timeToWait = 8.06f,
                    },
                    new StartCastBar() {
                      castName = "Irresistible Grace",
                      duration = 4.71f,
                    },
                    new WaitEvent() {
                      timeToWait = 4.71f,
                    },
                    new ReshufflePlayerIds(),
                    new SpawnTargetedEvents() {
                      targetingScheme =
                          new TargetSpecificPlayerIds() {
                            targetIds = new List<int>() { 0 },
                          },
                      referenceMechanicName = "IrresistibleGrace",
                      isPositionRelative = true,
                      spawnOnTarget = true,
                    },
                    new WaitEvent() {
                      timeToWait = 100,
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["IrresistibleGrace"] = new MechanicProperties() {
        collisionShape = CollisionShape.Round,
        collisionShapeParams = new Vector4(1.4f, 360, 0, 0),
        colorHtml = "#ff60ab",
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new WaitEvent() {
                      timeToWait = 0.1f,
                    },
                    new ApplyEffectToPlayers() {
                      effect =
                          new DamageEffect() {
                            name = "Irresistible Grace",
                            damageType = "Damage",
                            damageAmount = 1000000,
                            maxStackAmount = 8,
                          },
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementSingleVisual"] =
          new MechanicProperties() {
            collisionShape = CollisionShape.Round,
            collisionShapeParams = new Vector4(2.1213203435596425732025330863145f, 360, 0, 0),
            colorHtml = "#ff9600",
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new WaitEvent() {
                          timeToWait = 1,
                        },
                      },
                },
          };
      mechanicData.referenceMechanicProperties["AlmightyJudgementSingleDamage"] =
          new MechanicProperties() {
            collisionShape = CollisionShape.Round,
            collisionShapeParams = new Vector4(2.1213203435596425732025330863145f, 360, 0, 0),
            colorHtml = "#ff9600",
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new ApplyEffectToPlayers() {
                          effect =
                              new DamageEffect() {
                                name = "Almighty Judgement",
                                damageType = "Damage",
                                damageAmount = 200000,
                              },
                        },
                        new WaitEvent() {
                          timeToWait = 1,
                        },
                      },
                },
          };
      mechanicData.referenceMechanicProperties["AlmightyJudgementSingle"] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events =
                      new List<MechanicEvent>() {
                        new SpawnMechanicEvent() {
                          referenceMechanicName = "AlmightyJudgementSingleVisual",
                          isPositionRelative = true,
                        },
                        new WaitEvent() {
                          timeToWait = 8,
                        },
                        new SpawnMechanicEvent() {
                          referenceMechanicName = "AlmightyJudgementSingleDamage",
                          isPositionRelative = true,
                        },
                      },
                },
          };
      List<MechanicEvent> events = new List<MechanicEvent>();
      foreach (Vector2 position in new Vector2[] {
                 new Vector2(6, 6),
                 new Vector2(-6, 6),
                 new Vector2(3, 3),
                 new Vector2(3, 0),
                 new Vector2(0, 0),
                 new Vector2(-3, 0),
                 new Vector2(-3, -3),
                 new Vector2(-6, -6),
                 new Vector2(6, -6),
               }) {
        events.Add(new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementSingle",
          position = position,
        });
      }
      mechanicData.referenceMechanicProperties["AlmightyJudgementBlue"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events = events,
            },
      };
      events = new List<MechanicEvent>();
      foreach (Vector2 position in new Vector2[] {
                 new Vector2(3, 6),
                 new Vector2(-3, 6),
                 new Vector2(0, 3),
                 new Vector2(6, 3),
                 new Vector2(-6, 0),
                 new Vector2(3, -3),
                 new Vector2(6, -3),
                 new Vector2(0, -6),
               }) {
        events.Add(new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementSingle",
          position = position,
        });
      }
      mechanicData.referenceMechanicProperties["AlmightyJudgementYellow"] =
          new MechanicProperties() {
            visible = false,
            mechanic =
                new ExecuteMultipleEvents() {
                  events = events,
                },
          };
      events = new List<MechanicEvent>();
      foreach (Vector2 position in new Vector2[] {
                 new Vector2(0, 6),
                 new Vector2(-3, 3),
                 new Vector2(-6, 3),
                 new Vector2(6, 0),
                 new Vector2(0, -3),
                 new Vector2(-6, -3),
                 new Vector2(-3, -3),
                 new Vector2(3, -6),
                 new Vector2(-3, -6),
               }) {
        events.Add(new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementSingle",
          position = position,
        });
      }
      mechanicData.referenceMechanicProperties["AlmightyJudgementRed"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events = events,
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementRYB"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementRBY"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementYRB"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementYBR"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementBRY"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                  },
            },
      };
      mechanicData.referenceMechanicProperties["AlmightyJudgementBYR"] = new MechanicProperties() {
        visible = false,
        mechanic =
            new ExecuteMultipleEvents() {
              events =
                  new List<MechanicEvent>() {
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementBlue",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementYellow",
                    },
                    new WaitEvent() {
                      timeToWait = 2,
                    },
                    new SpawnMechanicEvent() {
                      referenceMechanicName = "AlmightyJudgementRed",
                    },
                  },
            },
      };
      mechanicData.mechanicPools = new Dictionary<string, List<MechanicEvent>>();
      mechanicData.mechanicPools["AlmightyJudgementPool"] = new List<MechanicEvent>() {
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementRYB",
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementRBY",
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementYRB",
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementYBR",
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementBRY",
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "AlmightyJudgementBYR",
        },
      };
      mechanicData.mechanicEvents = new List<MechanicEvent>() {
        new SpawnVisualObject() {
          textureFilePath = "Mechanics/Resources/ArenaCircle.png",
          relativePosition = new Vector3(0, -0.001f, 0),
          eulerAngles = new Vector3(90, 0, 0),
          scale = new Vector3(15.8637f, 15.8637f, 1),
          visualDuration = float.PositiveInfinity,
        },
        new SpawnMechanicEvent() {
          referenceMechanicName = "SpawnAlex",
        },
      };
      return mechanicData;
    }
  }
}
