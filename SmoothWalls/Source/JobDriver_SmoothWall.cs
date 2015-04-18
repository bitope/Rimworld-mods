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
        private DesignationDef smoothWallsDesignationDef = DefDatabase<DesignationDef>.GetNamed("SmoothWalls"); 

	    protected override IEnumerable<Toil> MakeNewToils()
        {
            float skill = this.pawn.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Construction);
            double skillMod = 1.0 / Math.Exp(skill / 20.0);

            yield return Toils_Reserve.Reserve(TargetIndex.A);

            {
                var toil = Toils_Goto.GotoCell(TargetIndex.A, PathMode.Touch);
                toil.FailOn(() =>
                            {
                                if (!pawn.CanReach(TargetLocA, PathMode.Touch, Danger.Some))
                                {
                                    var designation = Find.DesignationManager.DesignationAt(TargetLocA, smoothWallsDesignationDef);
                                    Find.DesignationManager.RemoveDesignation(designation);
                                    return true;
                                }
                                return false;
                            });
                yield return toil;
            }

            {
                Toil toil = new Toil();
                toil.defaultCompleteMode = ToilCompleteMode.Delay;
                toil.defaultDuration = (int)(650 * skillMod);

                toil.AddPreTickAction(() =>
                                      {
                                          workLeft += 1f;
                                      });

                toil.AddFinishAction(() =>
                                     {
                                         var stuffName = MineUtility.MineableInCell(TargetLocA).def.defName;
                                         if (stuffName.StartsWith("Mineable"))
                                             stuffName = stuffName.Replace("Mineable", "");
                                         else
                                             stuffName = "Blocks" + stuffName;
                                         var stuffDef = DefDatabase<ThingDef>.GetNamed(stuffName);

                                         MineUtility.MineableInCell(TargetLocA).Destroy();

                                         var thing = ThingMaker.MakeThing(ThingDefOf.Wall, stuffDef);
                                         thing = GenSpawn.Spawn(thing, TargetLocA);
                                         thing.SetFaction(Faction.OfColony);
                                     });

                toil.AddFinishAction(() =>
                {
                    var designation = Find.DesignationManager.DesignationAt(TargetLocA, smoothWallsDesignationDef);
                    Find.DesignationManager.RemoveDesignation(designation);
                }
                    );
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
