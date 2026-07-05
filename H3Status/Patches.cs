using HarmonyLib;
using H3Status.Model;
using FistVR;

namespace H3Status.Patches
{
    internal static class VersionHandler
    {
        private static VersionStatus versionStatus = new();

        public static VersionStatus GetVersionInfo()
        {
            versionStatus.Version = Plugin.Version;
            versionStatus.GameVersion = $"{GM.Version_UpdateNumber}.{GM.Version_AlphaNumber}.{GM.Version_PatchNumber}";

            return versionStatus;
        }
    }

    [HarmonyPatch]
    internal static class SceneHandler
    {
        public static string activeScene = string.Empty;

        private static SceneStatus sceneStatus = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SteamVR_LoadLevel), nameof(SteamVR_LoadLevel.Begin))]
        private static void SceneEvent(string levelName)
        {
            activeScene = levelName;

            sceneStatus = new SceneStatus
            {
                Name = levelName
            };

            Server.SendMessage(new Event
            {
                Type = EventType.SceneEvent,
                Status = sceneStatus
            });
        }
    }

    [HarmonyPatch]
    internal static class TNHPhaseHandler
    {
        private static readonly string[] holdNamesInstitution = ["HUB", "LIBRARY", "GARDEN", "ATRIUM", "LOBBY", "HEDRONS", "TURBINE", "HYDRO", "SPILLWAY", "RODS", "STORAGE", "APRROACH", "CROSSOVER", "PIPEWORKS", "VOID", "CONCOURSE", "BUNKER", "INCLINATOR", "ABYSS", "SUBSTATION"];
        private static readonly string[] supplyNamesInstitution = ["ARRAY", "STUDIO", "SUITE", "LOFT", "PENTHOUSE", "GREENWALL", "FENESTRA", "JUDGEMENT", "PRESIDIO", "DISSONANCE", "CLERESTORY", "HELIX", "FACILITY", "STACKS", "ALTAR", "PUMP"];
        private static bool isInitialized = false;

        private static TNHLevelStatus levelStatus = new();
        private static TNHPhaseStatus phaseStatus = new();
        private static TNHHoldPhaseStatus holdPhaseStatus = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.DelayedInit))]
        private static void TNHLevelEventPre(TNH_Manager __instance)
        {
            isInitialized = __instance.m_hasInit;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.DelayedInit))]
        private static void TNHLevelEventPost(TNH_Manager __instance)
        {
            bool becameInitialized = !isInitialized && __instance.m_hasInit;
            if (!becameInitialized) return;

            levelStatus = new TNHLevelStatus
            {
                Seed = __instance.HoldSequenceSeed,
                EquipmentSeed = __instance.equipmentSeed,
                LevelName = __instance.LevelName,
                CharacterName = __instance.C.DisplayName,
                ScoreMultiplier = TNHScoreHandler.GetMultiplier(),

                AiDifficulty = __instance.AI_Difficulty,
                RadarMode = __instance.RadarMode,
                TargetMode = __instance.TargetMode,
                HealthMode = __instance.HealthMode,
                EquipmentMode = __instance.EquipmentMode
            };

            Server.SendMessage(new Event
            {
                Type = EventType.TNHLevelEvent,
                Status = levelStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.SetPhase))]
        private static void TNHPhaseEvent(TNH_Phase p, TNH_Manager __instance)
        {
            phaseStatus = new TNHPhaseStatus
            {
                Phase = p,
                Level = __instance.m_level,
                Count = __instance.m_maxLevels,
                Seed = __instance.m_holdSequenceSeed,
                Hold = __instance.m_curHoldIndex,
                Supply = new(),
                HoldName = null,
                SupplyNames = null
            };

            foreach (int i in __instance.m_activeSupplyPointIndicies)
            {
                phaseStatus.Supply.Add(i);
            }

            if (SceneHandler.activeScene == "Institution")
            {
                phaseStatus.HoldName = holdNamesInstitution[__instance.m_curHoldIndex];

                phaseStatus.SupplyNames = new();
                foreach (int i in __instance.m_activeSupplyPointIndicies)
                {
                    phaseStatus.SupplyNames.Add(supplyNamesInstitution[i]);
                }
            }

            Server.SendMessage(new Event
            {
                Type = EventType.TNHPhaseEvent,
                Status = phaseStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_HoldPoint), nameof(TNH_HoldPoint.BeginAnalyzing))]
        [HarmonyPatch(typeof(TNH_HoldPoint), nameof(TNH_HoldPoint.IdentifyEncryption))]
        [HarmonyPatch(typeof(TNH_HoldPoint), nameof(TNH_HoldPoint.CompletePhase))]
        private static void TNHHoldPhaseEvent(TNH_HoldPoint __instance)
        {
            holdPhaseStatus = new TNHHoldPhaseStatus
            {
                Phase = __instance.m_state,
                Level = __instance.m_phaseIndex,
                Count = __instance.H.Phases.Count,
                EncryptionType = __instance.m_curPhase.Encryption,
                EncryptionCount = __instance.m_numTargsToSpawn,
                EncryptionTime = 120f
            };

            Server.SendMessage(new Event
            {
                Type = EventType.TNHHoldPhaseEvent,
                Status = holdPhaseStatus
            });
        }
    }

    [HarmonyPatch]
    internal static class TNHScoreHandler
    {
        private static TNHScoreStatus scoreStatus = new();
        private static TNHTokenStatus tokenStatus = new();

        private static readonly int[] eventMultiplier = [
            10000, // HoldPhaseComplete
            10,    // HoldDecisecondsRemaining
            1,
            10,   // HoldPhaseHealthBonus
            100,  // KillBonus
            200,  // HeadShotBonus
            100,  // MeleeKillBonus
            100,  // JointBreakBonus
            100,  // JointSeverBonus
            1,
            250,  // KillStreakBonus
            10,   // TakePhaseHealthBonus
            1,
            1,
            250   // TakeGuardClearSpeedBonus
        ];

        private static int GetEventScore(TNH_Manager.ScoringEvent ev, int num)
        {
            return num * eventMultiplier[(int)ev];
        }

        private static int GetTotalScore()
        {
            int score = 0;
            int multiplier = GetMultiplier();

            for (int i = 0; i <= 14; i++)
            {
                score += GetEventScore((TNH_Manager.ScoringEvent)i, GM.TNH_Manager.Nums[i]);
            }

            return score * multiplier;
        }

        internal static int GetMultiplier()
        {
            int multiplier = 1;

            if (GM.TNHOptions.TargetModeSetting == TNHSetting_TargetMode.AllTypes)
            {
                multiplier += 3;
            }
            else if (GM.TNHOptions.TargetModeSetting == TNHSetting_TargetMode.Simple)
            {
                multiplier += 2;
            }
            if (GM.TNHOptions.AIDifficultyModifier == TNHModifier_AIDifficulty.Standard)
            {
                multiplier += 3;
            }
            if (GM.TNHOptions.RadarModeModifier == TNHModifier_RadarMode.Standard)
            {
                multiplier += 2;
            }
            else if (GM.TNHOptions.RadarModeModifier != TNHModifier_RadarMode.Omnipresent)
            {
                multiplier += 3;
            }

            return multiplier;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.IncrementScoringStat))]
        private static void TNHScoreEvent(TNH_Manager.ScoringEvent ev, int num, TNH_Manager __instance)
        {
            Plugin.Logger.LogMessage($"{ev}: {GetEventScore(ev, num) * GetMultiplier()} ({GetEventScore(ev, num)}x{GetMultiplier()})");

            scoreStatus = new TNHScoreStatus
            {
                Type = ev,
                Value = GetEventScore(ev, num),
                Mult = GetMultiplier(),
                Score = GetTotalScore()
            };

            Server.SendMessage(new Event
            {
                Type = EventType.TNHScoreEvent,
                Status = scoreStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_HoldPoint), nameof(TNH_HoldPoint.TargetDestroyed))]
        private static void TNHEncryptionDestroyed()
        {
            Server.SendMessage(new Event
            {
                Type = EventType.TNHEncryptionDestroyed
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.AddTokens))]
        private static void TNHTokenEventAdd(int i, TNH_Manager __instance)
        {
            tokenStatus = new TNHTokenStatus
            {
                Change = i,
                Tokens = __instance.m_numTokens
            };

            Server.SendMessage(new Event
            {
                Type = EventType.TNHTokenEvent,
                Status = tokenStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TNH_Manager), nameof(TNH_Manager.SubtractTokens))]
        private static void TNHTokenEventSubtract(int i, TNH_Manager __instance)
        {
            tokenStatus = new TNHTokenStatus
            {
                Change = -i,
                Tokens = __instance.m_numTokens
            };

            Server.SendMessage(new Event
            {
                Type = EventType.TNHTokenEvent,
                Status = tokenStatus
            });
        }
    }

    [HarmonyPatch]
    internal static class PlayerHealthHandler
    {
        private static HealthStatus healthStatus = new();
        private static BuffStatus buffStatus = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.RegisterPlayerHit))]
        private static void HealthEventHit(float DamagePoints, bool FromSelf, int iff, FVRPlayerBody __instance)
        {
            healthStatus = new HealthStatus
            {
                Change = -(int)DamagePoints,
                Health = (int)__instance.Health,
                MaxHealth = (int)__instance.m_startingHealth
            };

            Server.SendMessage(new Event
            {
                Type = EventType.HealthEvent,
                Status = healthStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.HarmPercent))]
        private static void HealthEventHarm(float f, FVRPlayerBody __instance)
        {
            healthStatus = new HealthStatus
            {
                Change = -(int)(__instance.m_startingHealth * f),
                Health = (int)__instance.Health,
                MaxHealth = (int)__instance.m_startingHealth
            };

            Server.SendMessage(new Event
            {
                Type = EventType.HealthEvent,
                Status = healthStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.Init))]
        private static void HealthEventInit(FVRPlayerBody __instance)
        {
            healthStatus = new HealthStatus
            {
                Change = 0,
                Health = (int)__instance.Health,
                MaxHealth = (int)__instance.m_startingHealth
            };

            Server.SendMessage(new Event
            {
                Type = EventType.HealthEvent,
                Status = healthStatus
            });
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.SetHealthThreshold))]
        private static void HealthEventUpdate(float h, FVRPlayerBody __instance)
        {
            healthStatus = new HealthStatus
            {
                Change = (int)(h - __instance.Health),
                Health = (int)h,
                MaxHealth = (int)h
            };

            Server.SendMessage(new Event
            {
                Type = EventType.HealthEvent,
                Status = healthStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.HealPercent))]
        private static void HealthEventHeal(float f, FVRPlayerBody __instance)
        {
            healthStatus = new HealthStatus
            {
                Change = (int)(__instance.m_startingHealth * f),
                Health = (int)__instance.Health,
                MaxHealth = (int)__instance.m_startingHealth
            };

            Server.SendMessage(new Event
            {
                Type = EventType.HealthEvent,
                Status = healthStatus
            });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRPlayerBody), nameof(FVRPlayerBody.ActivatePower))]
        private static void BuffEvent(PowerupType type, PowerUpIntensity intensity, PowerUpDuration d, bool isPuke, bool isInverted, float DurationOverride = -1f)
        {
            float duration = 1f;

            switch (d)
            {
                case PowerUpDuration.Full:
                    duration = 30f;
                    break;
                case PowerUpDuration.Short:
                    duration = 20f;
                    break;
                case PowerUpDuration.VeryShort:
                    duration = 10f;
                    break;
                case PowerUpDuration.Blip:
                    duration = 2f;
                    break;
                case PowerUpDuration.SuperLong:
                    duration = 40f;
                    break;
            }

            if (DurationOverride > 0f)
            {
                duration = DurationOverride;
            }

            buffStatus = new BuffStatus
            {
                Type = type,
                Duration = duration,
                Inverted = isInverted
            };

            Server.SendMessage(new Event
            {
                Type = EventType.BuffEvent,
                Status = buffStatus
            });
        }
    }

    [HarmonyPatch]
    internal static class WeaponAmmoHandler
    {
        private static bool isUpdatePending = false;
        private static AmmoStatus ammoStatus = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FVRFireArm), nameof(FVRFireArm.FVRFixedUpdate))]
        private static void HandlePendingEvent()
        {
            if (isUpdatePending)
            {
                isUpdatePending = false;
                Server.SendMessage(new Event
                {
                    Type = EventType.AmmoEvent,
                    Status = ammoStatus
                });
            }
        }

        private static void UpdateAmmoCount(FVRFireArm fireArm)
        {
            if (fireArm == null || fireArm.m_hand == null) return;
            string weaponName = string.Empty;
            int weaponHand = fireArm.m_hand.IsThisTheRightHand ? 1 : 0;
            FireArmRoundType roundType = fireArm.RoundType;
            FireArmRoundClass roundClass = default;
            int currentAmmo = 0;
            int spentAmmo = 0;
            int maxCapacity = 0;

            try { roundClass = AM.GetDefaultRoundClass(fireArm.RoundType); }
            catch { }

            if (fireArm.ObjectWrapper != null)
            {
                if (IM.HasSpawnedID(fireArm.ObjectWrapper.ItemID))
                {
                    ItemSpawnerID spawnerID = IM.GetSpawnerID(fireArm.ObjectWrapper.ItemID);
                    weaponName = spawnerID.DisplayName;
                }
                else
                {
                    weaponName = fireArm.ObjectWrapper.DisplayName;
                }
            }

            if (fireArm.Magazine != null)
            {
                maxCapacity += fireArm.Magazine.m_capacity;
                currentAmmo += fireArm.Magazine.m_numRounds;

                if (fireArm.Magazine.LoadedRounds != null)
                {
                    for (int i = 0; i < fireArm.Magazine.LoadedRounds.Length; i++)
                    {
                        if (fireArm.Magazine.LoadedRounds[i] != null)
                        {
                            roundClass = fireArm.Magazine.LoadedRounds[i].LR_Class;
                        }
                    }
                }
            }

            if (fireArm.BeltDD != null)
            {
                currentAmmo += fireArm.BeltDD.m_roundsOnBelt;
            }

            if (fireArm.FChambers != null)
            {
                maxCapacity += fireArm.FChambers.Count;

                foreach (var chamber in fireArm.FChambers)
                {
                    if (chamber.m_round == null) continue;

                    if (chamber.IsSpent)
                    {
                        spentAmmo += 1;
                    }
                    else
                    {
                        roundClass = chamber.m_round.RoundClass;
                        currentAmmo += 1;
                    }
                }
            }

            ammoStatus = new AmmoStatus
            {
                Weapon = weaponName,
                Hand = weaponHand,
                RoundType = roundType,
                RoundClass = roundClass,
                Current = currentAmmo,
                Spent = spentAmmo,
                Capacity = maxCapacity
            };

            isUpdatePending = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRFireArmMagazine), nameof(FVRFireArmMagazine.AddRound), [typeof(FireArmRoundClass), typeof(bool), typeof(bool)])]
        [HarmonyPatch(typeof(FVRFireArmMagazine), nameof(FVRFireArmMagazine.AddRound), [typeof(FVRFireArmRound), typeof(bool), typeof(bool), typeof(bool)])]
        // [HarmonyPatch(typeof(FVRFireArmMagazine), nameof(FVRFireArmMagazine.UpdateBulletDisplay))]
        private static void AmmoEventMagazine(FVRFireArmMagazine __instance)
        {
            UpdateAmmoCount(__instance.FireArm);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRFireArm), nameof(FVRFireArm.LoadMag))]
        [HarmonyPatch(typeof(FVRFireArm), nameof(FVRFireArm.EjectMag))]
        // [HarmonyPatch(typeof(FVRFireArm), nameof(FVRFireArm.LoadClip))]
        // [HarmonyPatch(typeof(FVRFireArm), nameof(FVRFireArm.EjectClip))]
        private static void AmmoEventFireArm(FVRFireArm __instance)
        {
            UpdateAmmoCount(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FVRFireArmChamber), nameof(FVRFireArmChamber.UpdateProxyDisplay))]
        private static void AmmoEventChamber(FVRFireArmChamber __instance)
        {
            if (__instance.Firearm != null)
            {
                UpdateAmmoCount(__instance.Firearm);
            }
            else
            {
                FVRFireArm fireArm = __instance.transform.parent?.gameObject.GetComponent<FVRFireArm>();
                if (fireArm != null)
                {
                    UpdateAmmoCount(fireArm);
                }
            }
        }
    }

}
