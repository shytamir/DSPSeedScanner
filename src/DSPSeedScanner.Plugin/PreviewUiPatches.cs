using HarmonyLib;

namespace DSPSeedScanner.Plugin
{
    internal static class PreviewUiPatches
    {
        internal static DSPSeedScannerPlugin? Plugin { get; set; }

        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.SetStarmapGalaxy))]
        [HarmonyPostfix]
        private static void CompletedLoad(GameDesc ___gameDesc)
        {
            Plugin?.OnPreviewLoadCompleted(___gameDesc);
        }

        [HarmonyPatch(typeof(UIGalaxySelect), "_OnClose")]
        [HarmonyPostfix]
        private static void PreviewClosed()
        {
            Plugin?.OnPreviewClosed();
        }
    }
}
