using System;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Nickel;

internal static class SpriteLoaderPatches
{
    private static readonly WeakEvent<GetTextureEventArgs> OnGetTextureWeakEvent = new();

    internal static event EventHandler<GetTextureEventArgs> OnGetTexture
    {
        add => OnGetTextureWeakEvent.Add(value);
        remove => OnGetTextureWeakEvent.Remove(value);
    }

    internal static void Apply(Harmony harmony, ILogger logger)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(SpriteLoader), nameof(SpriteLoader.Get)) ?? throw new InvalidOperationException("Could not patch game methods: missing method `SpriteLoader.Get`"),
            prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(SpriteLoaderPatches), nameof(Get_Prefix)), priority: Priority.Last)
        );
    }

    private static bool Get_Prefix(Spr id, ref Texture2D? __result)
    {
        if (SpriteLoader.textures.TryGetValue(id, out __result))
            return false;

        GetTextureEventArgs args = new(id);
        OnGetTextureWeakEvent.Raise(null, args);
        __result = args.Texture;
        return args.Texture is null;
    }

    internal sealed class GetTextureEventArgs
    {
        public Spr Sprite { get; init; }
        public Texture2D? Texture { get; set; }

        public GetTextureEventArgs(Spr sprite)
        {
            this.Sprite = sprite;
        }
    }
}
