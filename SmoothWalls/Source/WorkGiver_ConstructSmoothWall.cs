using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_ConstructSmoothWall : WorkGiver
	{
		public override PathMode PathMode
		{
			get { return PathMode.Touch; }
		}

		public WorkGiver_ConstructSmoothWall(WorkGiverDef giverDef) : base(giverDef)
		{

		}

	    public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
	    {
	        return Find.DesignationManager.DesignationsOfDef(DefDatabase<DesignationDef>.GetNamed("SmoothWalls")).Select(designation => designation.target.Cell); 
	    }

	    public override bool HasJobOnCell(Pawn pawn, IntVec3 c)
		{
            return pawn.Faction == Faction.OfColony && Find.DesignationManager.DesignationAt(c, DefDatabase<DesignationDef>.GetNamed("SmoothWalls") ) != null && pawn.CanReserveAndReach(c, ReservationType.Total, PathMode.Touch, Danger.Some);
		}

		public override Job JobOnCell(Pawn pawn, IntVec3 c)
		{
            return new Job( DefDatabase<JobDef>.GetNamed("SmoothWalls") , new TargetInfo(c));
		}
	}
}
