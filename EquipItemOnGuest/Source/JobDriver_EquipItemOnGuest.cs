using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse.AI;
using Verse;

namespace EquipItemOnGuest
{
    class JobDriver_EquipItemOnGuest : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetThingA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            var item = TargetThingA as Apparel;
            var guest = TargetThingB as Pawn;

            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            Log.Message("Not despawned, null, or forbidden");

            this.FailOnCannotTouch(TargetIndex.B, PathEndMode.ClosestTouch);
            Log.Message("Can touch");

            this.FailOnBurningImmobile(TargetIndex.A);
            Log.Message("Not burning/immobile");
            

            yield return Toils_Reserve.Reserve(TargetIndex.A);
            Log.Message("After reserve");

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            Log.Message("After GotoThing");

            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            Log.Message("After StartCarryThing");

            yield return Toils_Goto.Goto(TargetIndex.B, PathEndMode.ClosestTouch);
            Log.Message("After Goto");

            yield return new Toil
            {
                initAction = () =>
                {
                    guest.equipment.AddEquipment(item);
                    guest.apparel.Wear(item);
                    guest.apparel.Notify_ApparelAdded(item);
                },
                defaultCompleteMode = ToilCompleteMode.FinishedBusy
            }.FailOn(() => !guest.apparel.CanWearWithoutDroppingAnything(item.def));
            Log.Message("After first custom Toil");

            yield return new Toil
            {
                initAction = delegate
                {
                    // Can't give yourself gifts
                    if (guest.Faction == pawn.Faction) return;

                    Tradeable thing = new Tradeable();
                    thing.AddThing(item, Transactor.Colony);

                    FactionGiftUtility.GiveGift(new List<Tradeable>() { thing }, guest.Faction, new GlobalTargetInfo(item));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            Log.Message("After second custom Toil");
        }
    }
}
