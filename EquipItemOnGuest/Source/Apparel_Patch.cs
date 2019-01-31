using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace EquipItemOnGuest
{
    [HarmonyPatch(typeof(Apparel), "GetFloatMenuOptions")]
    public class Apparel_Patch
    {
        public static void Postfix(Apparel __instance, ref IEnumerable<FloatMenuOption> __result, Pawn selPawn)
        {
            // Log.Message($"[mn] {selPawn.Name}");

            var thing = __instance;

            if (ShouldShow(selPawn, thing))
            {
                // Log.Message("[mn] should show");
                var options = __result.ToList();

                var guests = selPawn.Map.mapPawns.AllPawnsSpawned.Where(ApplicablePawn);

                foreach (var guest in guests)
                {
                    string label = $"Equip {thing.LabelCap} on {guest.LabelShortCap}";

                    // Log.Message($"[mn] {label}");
                    var menuOption = new FloatMenuOption(label, () =>
                    {
                        // selPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        // selPawn.jobs.StartJob(new Job(DefDatabase<JobDef>.GetNamed("EquipItemOnGuest"), thing, guest));

                        Log.Message("[mn] Create new job");
                        Job job = new Job(DefDatabase<JobDef>.GetNamed("EquipItemOnGuest"), thing, guest);

                        // selPawn.jobs.ClearQueuedJobs();

                        Log.Message("[mn] Give job to pawn");
                        // selPawn.jobs.StartJob(job, JobCondition.InterruptForced, null, true);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                    options.Add(menuOption);
                }

                __result = options;
            }
            else
            {
                Log.Message("[mn] should not show");
            }
        }

        private static bool ShouldShow(Pawn pawn, Thing target)
        {
            return !(pawn.Dead || pawn.Downed);
        }

        private static bool ApplicablePawn(Pawn target)
        {
            return 
                !target.Dead && 
                (target.CurJobDef == DefDatabase<JobDef>.GetNamed(JobDefOf.LayDown.defName)) &&
                (target.health.summaryHealth.SummaryHealthPercent < 1f) &&
                (target.def == ThingDefOf.Human);
        }
    }
}
