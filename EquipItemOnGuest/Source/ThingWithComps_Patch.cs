using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace EquipItemOnGuest
{
    [HarmonyPatch(typeof(ThingWithComps), "GetFloatMenuOptions")]
    public static class ThingWithComps_Patch
    {
        [HarmonyPostfix]
        public static void AddEquipOnGuestFloatMenuOption(ref IEnumerable<FloatMenuOption> __result, Thing __instance, Pawn selPawn)
        {
            ThingWithComps thing = __instance as ThingWithComps;

            if (ShouldShow(selPawn, thing))
            {
                var options = __result.ToList();
                var guests = selPawn.Map.mapPawns.FreeColonistsSpawned.Where(ThingWithComps_Patch.ApplicablePawn);

                foreach (var guest in guests)
                {
                    var menuOption = new FloatMenuOption($"Equip {thing.LabelCap} on ${guest.LabelShortCap}", () =>
                    {
                        selPawn.jobs.StartJob(new Job(DefDatabase<JobDef>.GetNamed("EquipItemOnGuest"), thing, guest));
                    });
                    options.Add(menuOption);
                }

                __result = options;
            }
        }

        private static bool ShouldShow(Pawn pawn, Thing target)
        {
            return !(pawn.Dead || pawn.Downed);
        }

        private static bool ApplicablePawn(Pawn target)
        {
            return !target.Dead && (target.CurJobDef == JobDefOf.Wait_Downed);
        }
    }
}
