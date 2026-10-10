namespace NotThatBlindCartographer.Modifier
{
    public static class HoverTextModifier
    {
        public static void AddHoverText<T>(T __instance, ref string __result)
        {
            if (!(NotThatBlindCartographer._enableHotKey.Value && NotThatBlindCartographer._showHotKeyText.Value) ||
                __instance == null) return;
            var pinToMapText = $"[<color=yellow>{NotThatBlindCartographer.ConfigPinModifierKey.Value} + {NotThatBlindCartographer.ConfigPinKey.Value}</color>] " +
                               NotThatBlindCartographer.ConfigPinToMapText.Value;
            __result += $"\n{pinToMapText}";
        }
    }
}
