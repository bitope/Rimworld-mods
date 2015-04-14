using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
    public class JobDriver_SmoothWall : JobDriver
	{
		public const float BaseSmoothWork = 1200f;
		private float workLeft = -1000f;

        public JobDriver_SmoothWall(Pawn pawn) : base(pawn)
		{
		}

	    protected override IEnumerable<Toil> MakeNewToils()
	    {
            yield return Toils_Reserve.Reserve(TargetIndex.A, ReservationType.Total);

	        {
	            var toil = Toils_Goto.GotoCell(TargetIndex.A, PathMode.Touch);
                toil.FailOn(() =>
                            {
                                if (!pawn.CanReach(TargetLocA, PathMode.Touch, Danger.Some))
                                {
                                    Find.DesignationManager.RemoveAllDesignationsAt(TargetLocA);
                                    return true;
                                }
                                return false;
                            });
	            yield return toil;
	        }

            {
                Toil toil = new Toil();
                toil.defaultCompleteMode = ToilCompleteMode.Delay;
                toil.defaultDuration = 650; 

                toil.AddPreTickAction(() =>
                                      {
                                          workLeft += 1f;
                                      });

                toil.AddFinishAction(() =>
                                     {
                                         var stuff = "Blocks"+ MineUtility.MineableInCell(TargetLocA).def.defName;
                                         if (stuff == "BlocksMineableSteel") stuff = "Steel";
                                         if (stuff == "BlocksMineableSilver") stuff = "Silver";
                                         if (stuff == "BlocksMineableGold") stuff = "Gold";
                                         if (stuff == "BlocksMineableUranium") stuff = "Uranium";
                                         if (stuff == "BlocksMineablePlasteel") stuff = "Plasteel";

                                         var stuffDef = DefDatabase<ThingDef>.GetNamed(stuff);

                                         MineUtility.MineableInCell(TargetLocA).Destroy();

                                         var thing = ThingMaker.MakeThing(ThingDefOf.Wall, stuffDef);
                                         thing = GenSpawn.Spawn(thing, TargetLocA);
                                         thing.SetFaction(Faction.OfColony);
                                     });

                toil.AddFinishAction(()=>Find.DesignationManager.RemoveAllDesignationsAt(TargetLocA));
                yield return toil;
            }
	       
	    }

	    public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.workLeft, "ticksToDone", 0f, false);
		}
	}
}
