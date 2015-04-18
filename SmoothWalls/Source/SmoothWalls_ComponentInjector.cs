using UnityEngine;
using Verse;

namespace RimWorld
{
    internal class SmoothWalls_ComponentInjector : ITab
    {
        protected GameObject initializer;

        public SmoothWalls_ComponentInjector()
        {
            Log.Message("ComponentInjector: initializing for " + SmoothWalls_ComponentInjectorBehavior.mapComponentName);
            this.initializer = new GameObject("SmoothWalls_ComponentInjector");
            this.initializer.AddComponent<SmoothWalls_ComponentInjectorBehavior>();
            UnityEngine.Object.DontDestroyOnLoad(this.initializer);
            
        }

        protected override void FillTab()
        {

        }
    }
}