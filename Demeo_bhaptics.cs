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

namespace Demeo_bhaptics
{
    [BepInPlugin("org.bepinex.plugins.Demeo_bhaptics", "Demeo bhaptics integration", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
#pragma warning disable CS0109 // Remove unnecessary warning
        internal static new ManualLogSource Log;
#pragma warning restore CS0109
        public static int myNetworkId = 0;
        public static TactsuitVR tactsuitVr;

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
        /*
        [HarmonyPatch(typeof(Dice), "OnDiceStopped", new Type[] { typeof(Dice.Outcome), typeof(Vector3), typeof(Quaternion), typeof(bool) })]
        public class bhaptics_DiceStopped
        {
            [HarmonyPostfix]
            public static void Postfix(Dice __instance, Dice.Outcome outcome)
            {
                if (outcome == Dice.Outcome.Miss) tactsuitVr.LOG("Miss");
                if (outcome == Dice.Outcome.Hit) tactsuitVr.LOG("Hit");
                if (outcome == Dice.Outcome.Crit) tactsuitVr.LOG("Crit");
            }
        }
        */


        [HarmonyPatch(typeof(Boardgame.BoardEntities.Abilities.Ability), "GenerateAttackDamage", new Type[] { typeof(Piece), typeof(Piece), typeof(Dice.Outcome), typeof(BoardModel), typeof(Piece[]) })]
        public class bhaptics_GenerateAttackDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Boardgame.BoardEntities.Abilities.Ability __instance, Piece source, Piece mainTarget, Dice.Outcome diceResult)
            {
                if (source.networkID == myNetworkId)
                {
                    tactsuitVr.LOG("Damaged something!");
                    tactsuitVr.PlaybackHaptics("Healing");
                }
                if (mainTarget.networkID == myNetworkId)
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
        

        
        [HarmonyPatch(typeof(Boardgame.Networking.INetworkController), "AllocateNetworkID", new Type[] {  })]
        public class bhaptics_AllocateNetworkID
        {
            [HarmonyPostfix]
            public static void Postfix(Boardgame.Networking.INetworkController __instance, int __result)
            {
                tactsuitVr.LOG("Network ID: " + __result.ToString());
                myNetworkId = __result;
            }
        }
        
    }
}
