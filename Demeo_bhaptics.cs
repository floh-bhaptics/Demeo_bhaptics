using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Boardgame.Haptic;
using Boardgame.BoardEntities;
using Boardgame;
using UnityEngine;

namespace Demeo_bhaptics
{
    public class Demeo_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static int myNetworkId = 0;
        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(XrHapticModule), "Play", new Type[] { typeof(HapticHand), typeof(HapticId) })]
        public class bhaptics_Haptics
        {
            [HarmonyPostfix]
            public static void Postfix(HapticHand hapticHand, HapticId hapticId)
            {
                if (hapticId == HapticId.DefaultClickButton) { tactsuitVr.LOG("Right? " + (hapticHand == HapticHand.RightHand).ToString()); }
            }
        }

        [HarmonyPatch(typeof(Dice), "OnDiceStopped", new Type[] { typeof(Dice.Outcome), typeof(Vector3), typeof(Quaternion) })]
        public class bhaptics_DiceStopped
        {
            [HarmonyPostfix]
            public static void Postfix(Dice.Outcome outcome)
            {
                if (outcome == Dice.Outcome.Miss) tactsuitVr.LOG("Miss");
                if (outcome == Dice.Outcome.Hit) tactsuitVr.LOG("Hit");
                if (outcome == Dice.Outcome.Crit) tactsuitVr.LOG("Crit");
            }
        }

        [HarmonyPatch(typeof(Boardgame.BoardEntities.Abilities.Ability), "GenerateAttackDamage", new Type[] { typeof(Piece), typeof(Piece), typeof(Dice.Outcome), typeof(BoardModel), typeof(Piece[]) })]
        public class bhaptics_GenerateAttackDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Boardgame.BoardEntities.Abilities.Ability __instance, Piece source, Piece mainTarget, Dice.Outcome diceResult)
            {
                if (source.networkID == myNetworkId)
                {
                    tactsuitVr.LOG("Damaged something!");
                }
                if (mainTarget.networkID == myNetworkId)
                {
                    if (diceResult == Dice.Outcome.Miss) return;
                    tactsuitVr.LOG("Got hit!");
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
