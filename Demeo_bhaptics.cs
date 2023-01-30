using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Boardgame.Haptic;

namespace Demeo_bhaptics
{
    public class Demeo_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
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

    }
}
