using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace EquipItemOnGuest
{
    class JobDriver_EquipItemOnGuest : JobDriver_Equip
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            
            return pawn.Reserve(TargetThingA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnCannotTouch(TargetIndex.B, PathEndMode.ClosestTouch);
            this.FailOnBurningImmobile(TargetIndex.B);

            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.Goto(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return new Toil
            {
                initAction = () =>
                {
                    var guest = TargetThingB as Pawn;
                    var item = TargetThingA as ThingWithComps;

                    guest.equipment.MakeRoomFor(item);
                    guest.equipment.AddEquipment(item);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return new Toil
            {
                initAction = delegate
                {
                    var item = TargetThingA as ThingWithComps;
                    var guest = TargetThingB as Pawn;
                    var faction = guest.Faction;

                    faction.Notify_PlayerTraded(item.MarketValue, pawn);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
