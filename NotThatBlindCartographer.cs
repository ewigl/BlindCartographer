using NotThatBlindCartographer.Loader;
using NotThatBlindCartographer.Ui;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace NotThatBlindCartographer
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class NotThatBlindCartographer : BaseUnityPlugin
    {
        private const string PluginGuid = "ewigl." + PluginName;
        private const string PluginName = "NotThatBlindCartographer";
        private const string PluginVersion = "1.0.4";
        public static ConfigEntry<KeyCode> ConfigPinKey;
        public static ConfigEntry<KeyCode> ConfigPinModifierKey;
        public static ConfigEntry<string> ConfigPinToMapText;
        public static ConfigEntry<string> ConfigEnterPinNameText;
        public static ConfigEntry<string> ConfigPinAddedText;
        private static ConfigEntry<bool> _modEnabled;
        public static ConfigEntry<bool> _enableCartographyTableInNoMapMode;
        public static ConfigEntry<bool> _enableHotKey;
        public static ConfigEntry<bool> _showHotKeyText;

        public static Harmony Harmony;

        private GameObject _lastHoveredObject;
        private string _lastObjectName;

        public void Awake()
        {
            _modEnabled = Config.Bind("General", "ModEnabled", true, "Enable or disable the mod.");
            _enableCartographyTableInNoMapMode = Config.Bind("Map", "EnableCartographyTableInNoMapMode", true, "Enable cartography table in no map mode.");
            _enableHotKey = Config.Bind("Hotkeys", "EnableHotKey", true, "Enable hot key.");
            _showHotKeyText = Config.Bind("Hotkeys", "ShowHotKeyText", true, "Show hot key text.");
            ConfigPinKey = Config.Bind("Hotkeys", "HotKey", KeyCode.Y, "Key to press with the modifier key to pin hovered object to the map.");
            ConfigPinModifierKey = Config.Bind("Hotkeys", "HotKeyModifier", KeyCode.LeftShift, "Modifier key to press with HotKey to pin hovered object to the map.");
            ConfigPinToMapText = Config.Bind("Text", "TextPinToMap", "Pin to the map", "Text shown after the pin hotkey hint when hovering a pin-able object.");
            ConfigEnterPinNameText = Config.Bind("Text", "TextEnterPinName", "Enter pin name", "Title of the pin name input dialog.");
            ConfigPinAddedText = Config.Bind("Text", "TextPinAdded", "Pin `{pinName}` added!", "Notification shown after adding a pin. Placeholder: {pinName}.");

            if (_modEnabled.Value)
            {
                Harmony = Harmony.CreateAndPatchAll(typeof(NotThatBlindCartographer).Assembly, PluginGuid);
                HoverTextPatchLoader.ApplyPatches();
            }
        }

        private void Update()
        {
            if (!_modEnabled.Value) return;
            if (IsAnyGameUIOpen()) return;
            if (_enableHotKey.Value && Input.GetKeyDown(ConfigPinKey.Value) && Input.GetKey(ConfigPinModifierKey.Value))
            {
                var player = Player.m_localPlayer;
                if (player == null) return;

                GetHoverObject(player);

                TextInput.instance.RequestText(
                    new PinTextReceiver(_lastObjectName, AddToMap),
                    ConfigEnterPinNameText.Value,
                    255
                );
            }
        }

        private bool IsAnyGameUIOpen()
        {
            return InventoryGui.IsVisible()
                   || Minimap.IsOpen()
                   || TextInput.IsVisible()
                   || StoreGui.IsVisible()
                   || Chat.instance?.HasFocus() == true
                   || Console.IsVisible()
                   || Menu.IsVisible();
        }

        private void GetHoverObject(Player player)
        {
            _lastHoveredObject = null;
            _lastObjectName = "";
            var hoverObject = player.GetHoverObject();
            if (hoverObject != null)
            {
                var hoverable = hoverObject.GetComponentInParent<Hoverable>();
                if (hoverable != null)
                {
                    _lastHoveredObject = hoverObject;

                    var hoverText = hoverable.GetHoverText();
                    if (!string.IsNullOrEmpty(hoverText))
                    {
                        var split = hoverText.Split('\n');
                        if (split.Length > 1)
                            _lastObjectName = split[0];
                    }
                }
            }
        }

        private void AddToMap(string pinName)
        {
            var minimap = Minimap.instance;
            if (minimap == null) return;
            var position = _lastHoveredObject == null
                ? Player.m_localPlayer.transform.position
                : _lastHoveredObject.transform.position;

            minimap.AddPin(position, Minimap.PinType.Icon3, pinName, true, false);

            Player.m_localPlayer?.Message(MessageHud.MessageType.TopLeft, ConfigPinAddedText.Value.Replace("{pinName}", pinName), 0, null, false);
        }
    }
}
