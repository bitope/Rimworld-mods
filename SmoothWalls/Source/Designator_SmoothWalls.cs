using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
    public class Designator_SmoothWalls : Designator
	{
        
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		public override bool DragDrawMeasurements
		{
			get
			{
				return true;
			}
		}

		public Designator_SmoothWalls()
		{
            //Log.Message("Designator_SmoothWalls Loaded.");

		    this.defaultLabel = "Smooth walls";
			this.defaultDesc = "Smooth the rough walls.";
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/SmoothFloor", true);
			this.useMouseIcon = true;
			this.soundDragSustain = SoundDefOf.DesignateDragStandard;
			this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			this.soundSucceeded = SoundDefOf.DesignateSmoothFloor;
			this.hotKey = KeyBindingDefOf.Misc1;
		}

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds())
            {
                return false;
            }

            if (Find.DesignationManager.DesignationAt(c, DefDatabase<DesignationDef>.GetNamed("SmoothWalls")) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            if (c.Fogged())
            {
                return false;
            }

            if (MineUtility.MineableInCell(c) == null)
            {
                return "MessageMustDesignateMineable".Translate();
            }

            if (!Find.ListerPawns.ColonistsAndPrisoners.Any(i => i.CanReach(c, PathMode.Touch, Danger.Some)))
            {
                return false;
            }

            return AcceptanceReport.WasAccepted;
		}

        public override void DesignateSingleCell(IntVec3 c)
        {
            Find.DesignationManager.AddDesignation(new Designation(c, DefDatabase<DesignationDef>.GetNamed("SmoothWalls")));
		}

		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}
	}
}
