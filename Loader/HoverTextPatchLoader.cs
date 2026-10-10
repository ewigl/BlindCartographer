using System;
using System.Linq;
using System.Reflection;
using NotThatBlindCartographer.Modifier;
using HarmonyLib;
using UnityEngine;

namespace NotThatBlindCartographer.Loader
{
    public static class HoverTextPatchLoader
    {
        public static void ApplyPatches()
        {
            var hoverableTypes = typeof(Hoverable).Assembly.GetTypes()
                .Where(t => typeof(Hoverable).IsAssignableFrom(t)).Where(t =>
                {
                    var m = t.GetMethod("GetHoverText", BindingFlags.Instance | BindingFlags.Public);
                    return m != null
                           && m.GetMethodBody() != null
                           && !typeof(Character).IsAssignableFrom(t);
                });

            foreach (var type in hoverableTypes) PatchHoverText(type);
        }

        private static void PatchHoverText(Type type)
        {
            var method = AccessTools.Method(type, "GetHoverText");
            if (method == null)
            {
                Debug.LogWarning($"[HoverTextPatch] Can't find GetHoverText of {type.Name}");
                return;
            }

            var postfix = new HarmonyMethod(typeof(HoverTextModifier)
                .GetMethod(nameof(HoverTextModifier.AddHoverText))
                ?.MakeGenericMethod(type));

            NotThatBlindCartographer.Harmony.Patch(method, postfix: postfix);
        }
    }
}
