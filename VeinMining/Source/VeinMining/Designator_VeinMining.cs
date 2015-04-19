using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Rimworld
{
    public class Designator_VeinMining : Designator_Mine
    {
        public override int DraggableDimensions
        {
            get { return 0; }
        }

        public Designator_VeinMining()
        {
            this.defaultLabel = "Vein Mining";
            this.icon = ContentFinder<Texture2D>.Get("Minepick", true);
            this.defaultDesc = "Click on a visible ore and you will mine the whole vein.";
            this.useMouseIcon = true;
            this.soundDragSustain = SoundDefOf.DesignateDragStandard;
            this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
            this.soundSucceeded = SoundDefOf.DesignateMine;
            this.hotKey = KeyBindingDefOf.Misc10;
            this.tutorHighlightTag = "DesignatorMine";
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!GenGrid.InBounds(c))
            {
                return AcceptanceReport.WasRejected;
            }

            if (Find.DesignationManager.DesignationAt(c, DesignationDefOf.Mine) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            if( Find.ThingGrid.ThingsAt(c).Any(i => isOre(i.def) && !GridsUtility.Fogged(c)) )
            {
                return AcceptanceReport.WasAccepted;
            }

            return "Must designate mineable and accessable ore!";
        }
        
        public override void DesignateSingleCell(IntVec3 loc)
        {
            var thing = Find.ThingGrid.ThingsAt(loc).FirstOrDefault(i => isOre(i.def));
            if (thing!=null)
            {
                Designator_Mine designator_Mine = new Designator_Mine();
                var veinCells = this.exploreCell(loc, thing.def);
                designator_Mine.DesignateMultiCell(veinCells);
            }
        }

        private List<IntVec3> exploreCell(IntVec3 at, ThingDef selectedOreDef, List<IntVec3> list = null)
        {
            if (list == null)
            {
                list = new List<IntVec3>();
            }

            if (Find.ThingGrid.ThingsListAt(at).Where(i => i.def == selectedOreDef).Any())
            {
                list.Add(at);

                var adjacant = GenAdjFast.AdjacentCells8Way(at).ToArray();
                foreach (var a in adjacant)
                {
                    if (!list.Contains(a))
                    {
                        exploreCell(a, selectedOreDef, list);
                    }
                }
            }
            return list;
        }

        public bool isOre(ThingDef def)
        {
            return def.defName.StartsWith("Mineable");
        }
    }
}
