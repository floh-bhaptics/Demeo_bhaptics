using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MyBhapticsTactsuit;
using Boardgame.Haptic;
using Boardgame.BoardEntities;
using Boardgame;
using UnityEngine;
using Boardgame.Data;

namespace Demeo_bhaptics
{
    [BepInPlugin("org.bepinex.plugins.Demeo_bhaptics", "Demeo bhaptics integration", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
        internal static new ManualLogSource Log;
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
        public static List<int> myNetworkIds = new List<int>();
        public static bool myTurn = false;
        public static TactsuitVR tactsuitVr;

        private void Awake()
        {
            // Make my own logger so it can be accessed from the Tactsuit class
            Log = base.Logger;
            // Plugin startup logic
            Logger.LogMessage("Plugin Demeo_bhaptics is loaded!");
            tactsuitVr = new TactsuitVR();
            tactsuitVr.LOG("Logging works.");
            // one startup heartbeat so you know the vest works correctly
            tactsuitVr.PlaybackHaptics("HeartBeat");
            // patch all functions
            var harmony = new Harmony("bhaptics.patch.demeo");
            harmony.PatchAll();
        }



        /*
        [HarmonyPatch(typeof(XrHapticModule), "Play", new Type[] { typeof(HapticHand), typeof(HapticId) })]
        public class bhaptics_Haptics
        {
            [HarmonyPostfix]
            public static void Postfix(HapticHand hapticHand, HapticId hapticId)
            {
                if (hapticId == HapticId.DefaultClickButton) { tactsuitVr.LOG("Right? " + (hapticHand == HapticHand.RightHand).ToString()); }
            }
        }
        */

        [HarmonyPatch(typeof(Dice), "OnDiceStopped", new Type[] { typeof(Dice.Outcome), typeof(Vector3), typeof(Quaternion), typeof(bool) })]
        public class bhaptics_DiceStopped
        {
            [HarmonyPostfix]
            public static void Postfix(Dice __instance, Dice.Outcome outcome)
            {
                if (outcome == Dice.Outcome.Miss) tactsuitVr.LOG("Miss");
                if (outcome == Dice.Outcome.Hit) tactsuitVr.LOG("Hit");
                if (outcome == Dice.Outcome.Crit) tactsuitVr.LOG("Crit");
                tactsuitVr.PlaybackHaptics("HeartBeat");
            }
        }



        [HarmonyPatch(typeof(Boardgame.BoardEntities.Abilities.Ability), "GenerateAttackDamage")]
        public class bhaptics_GenerateAttackDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Boardgame.BoardEntities.Abilities.Ability __instance, Piece source, Piece mainTarget, Dice.Outcome diceResult, Piece[] targets)
            {
                tactsuitVr.LOG("Dice outcome: " + diceResult.ToString());
                tactsuitVr.LOG("Hooked into damage: " + __instance.abilityKey.ToString() + " " + __instance.abilityDamage.targetDamage.ToString());
                tactsuitVr.LOG("Source ID: " + source.networkID.ToString() + " " + mainTarget.networkID.ToString());
                tactsuitVr.LOG("My turn: " + myTurn.ToString());
                bool attacking = source.IsPlayer();
                bool damaged = myNetworkIds.Contains(mainTarget.networkID);
                for (int i = 0; i < targets.Length; i++)
                {
                    Piece player = targets[i];
                    if (myNetworkIds.Contains(player.networkID)) damaged = true;
                    if (player.IsDowned()) tactsuitVr.StartHeartBeat();
                    if (player.IsDead()) tactsuitVr.StopHeartBeat();
                }
                /*
                if (source.networkID == myNetworkId)
                {
                    tactsuitVr.LOG("Damaged something!");
                    tactsuitVr.PlaybackHaptics("Healing");
                }
                */
                if (damaged)
                {
                    if (__instance.abilityDamage.targetDamage <= 0) return;
                    if (diceResult == Dice.Outcome.Miss) return;
                    tactsuitVr.LOG("Got hit! " + __instance.abilityKey.ToString());
                    string damageType = "Impact";
                    switch (__instance.abilityKey)
                    {
                        case DataKeys.AbilityKey.PlayerMelee:
                            damageType = "BladeHit";
                            break;
                        case DataKeys.AbilityKey.AcidSpit:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.BossShockwave:
                            damageType = "ExplosionUp";
                            break;
                        case DataKeys.AbilityKey.Corrupt:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.CorruptionBomb:
                            damageType = "ExplosionUp";
                            break;
                        case DataKeys.AbilityKey.DrainLife:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.Electricity:
                            damageType = "Electrocution";
                            break;
                        case DataKeys.AbilityKey.EnemyFireball:
                            damageType = "FireballHit";
                            break;
                        case DataKeys.AbilityKey.EnemyFrostball:
                            damageType = "FreezeHit";
                            break;
                        case DataKeys.AbilityKey.Explosion:
                            damageType = "ExplosionUp";
                            break;
                        case DataKeys.AbilityKey.Fireball:
                            damageType = "FireballHit";
                            break;
                        case DataKeys.AbilityKey.Freeze:
                            damageType = "FreezeHit";
                            break;
                        case DataKeys.AbilityKey.HealingLight:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.HealingPotion:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.HealingPowder:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.HealingWard:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.HymnOfHealing:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.LightningBolt:
                            damageType = "Electrocution";
                            break;
                        case DataKeys.AbilityKey.Petrify:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.PoisonBomb:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.PoisonedTip:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.PoisonGas:
                            damageType = "Poison";
                            break;
                        case DataKeys.AbilityKey.ReplenishArmor:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.Revive:
                            damageType = "Healing";
                            break;
                        case DataKeys.AbilityKey.Shockwave:
                            damageType = "ExplosionUp";
                            break;
                        default:
                            break;

                    }
                    tactsuitVr.PlaybackHaptics(damageType);
                }
            }
        }



        [HarmonyPatch(typeof(PieceAndTurnController), "OnNewTurnStarted", new Type[] { typeof(int), typeof(int) })]
        public class bhaptics_NewTurnStarted
        {
            [HarmonyPostfix]
            public static void Postfix(PieceAndTurnController __instance)
            {
                foreach (Piece piece in __instance.Pieces())
                {
                    if (__instance.IsPlayersTurn())
                    {
                        int currentID = __instance.CurrentPieceId;
                        if (!myNetworkIds.Contains(currentID)) myNetworkIds.Add(currentID);
                    }
                }
            }
        }

    }

}
