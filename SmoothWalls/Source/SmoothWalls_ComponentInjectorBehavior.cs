using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld
{
    internal class SmoothWalls_ComponentInjectorBehavior : MonoBehaviour
    {
        public static readonly string mapComponentName = "RimWorld.Designator_SmoothWalls";
        private Designator_SmoothWalls mapComponent = new Designator_SmoothWalls();
        protected bool reinjectNeeded = false;
        protected float reinjectTime = 0f;
        public void OnLevelWasLoaded(int level)
        {
            this.reinjectNeeded = true;
            if (level >= 0)
            {
                this.reinjectTime = 1f;
            }
            else
            {
                this.reinjectTime = 0f;
            }
        }

        public void FixedUpdate()
        {
            if (this.reinjectNeeded)
            {
                this.reinjectTime -= Time.fixedDeltaTime;
                if (this.reinjectTime <= 0f)
                {
                    this.reinjectNeeded = false;
                    this.reinjectTime = 0f;

                    var ordersTab = DefDatabase<DesignationCategoryDef>.GetNamed("Orders");
                    //foreach (var tab in ordersTab.specialDesignatorClasses)
                    //{
                    //    Log.Message(tab.Name);
                    //}

                    if (ordersTab.specialDesignatorClasses.All(i => i.Name != "Designator_SmoothWalls"))
                    {
                        Log.Message("Injecting Designator_SmoothWalls.");
                        int order = ordersTab.specialDesignatorClasses.IndexOf(typeof (Designator_SmoothFloor));
                        ordersTab.specialDesignatorClasses.Insert(order+1,typeof(Designator_SmoothWalls));
                        ordersTab.ResolveReferences();
                    }
                }
            }
        }
        public void Start()
        {
            this.OnLevelWasLoaded(-1);
        }
    }
}
