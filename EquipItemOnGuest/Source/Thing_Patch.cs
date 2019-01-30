using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace EquipItemOnGuest
{
    [HarmonyPatch(typeof(Thing), "GetFloatMenuOptions")]
    public class Thing_Patch
    {
        public static void Postfix(ref IEnumerable<FloatMenuOption> __result, Thing __instance, Pawn selPawn)
        {
            Log.Message($"[mn] {selPawn.Name}");

            var thing = __instance;

            if (ShouldShow(selPawn, thing))
            {
                Log.Message("[mn] should show");
                var options = __result.ToList();

                foreach (var pawn in selPawn.Map.mapPawns.AllPawnsSpawned)
                {
                    Log.Message($"[mn] pawn: {pawn.Name}");
                }
                var guests = selPawn.Map.mapPawns.AllPawnsSpawned.Where(ApplicablePawn);

                foreach (var guest in guests)
                {
                    string label = $"Equip {thing.LabelCap} on ${guest.LabelShortCap}";

                    Log.Message($"[mn] {label}");
                    var menuOption = new FloatMenuOption(label, () =>
                    {
                        selPawn.jobs.StartJob(new Job(DefDatabase<JobDef>.GetNamed("EquipItemOnGuest"), thing, guest));
                    });
                    options.Add(menuOption);
                }

                __result = options;
            }
            else
            {
                Log.Warning("[mn] should not show");
            }
        }

        private static bool ShouldShow(Pawn pawn, Thing target)
        {
            return !(pawn.Dead || pawn.Downed);
        }

        private static bool ApplicablePawn(Pawn target)
        {
            Log.Message($"[mn] {target.Name}: {target.CurJobDef.ToString()}");
            return !target.Dead && (target.CurJobDef == DefDatabase<JobDef>.GetNamed(JobDefOf.Wait_Downed.defName));
        }
    }
}
