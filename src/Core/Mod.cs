using TUNING;
using UnityEngine;
using HarmonyLib;

namespace AIAgentPlugin
{
    public class Mod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Debug.Log("[AI Agent Plugin] Mod.OnLoad called");
            
            // Call our main loader
            Loader.OnLoad();
            
            base.OnLoad(harmony);
        }
    }
}