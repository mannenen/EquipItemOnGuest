using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using Verse;

namespace EquipItemOnGuest
{
    public class EquipItemOnGuest : Mod
    {
        public EquipItemOnGuest(ModContentPack contentPack) : base(contentPack)
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.github.mannenen.eiog");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("Equip Item On Guest for RimWorld 1.0 - Patching complete");
        }
    }
}
