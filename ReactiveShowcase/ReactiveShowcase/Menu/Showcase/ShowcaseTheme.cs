using System.Collections.Generic;
using Reactive;
using UnityEngine;

namespace ReactiveShowcase.Showcase;

internal readonly record struct ShowcasePalette(Color Primary, Color Secondary, Color Surface);

/// <summary>
/// The global palette. Pages map <see cref="Palette"/> into component colors, so changing it restyles everything live.
/// </summary>
internal static class ShowcaseTheme {
    public static readonly IReadOnlyList<(string Name, ShowcasePalette Palette)> Presets = [
        ("Beat Saber", new(new(0f, 0.75f, 1f), new(0.99f, 0.15f, 0.88f), Color.black)),
        ("Sunset", new(new(1f, 0.55f, 0.1f), new(0.6f, 0.2f, 1f), new(0.15f, 0.02f, 0.1f))),
        ("Toxic", new(new(0.5f, 1f, 0.1f), new(0f, 1f, 0.7f), new(0f, 0.1f, 0.05f))),
        ("Crimson", new(new(1f, 0.15f, 0.25f), new(1f, 0.8f, 0.2f), new(0.12f, 0f, 0f))),
        ("Mono", new(Color.white, new(0.6f, 0.6f, 0.6f), Color.black)),
        ("Tonal", new(Color.HSVToRGB(0.55f, 0.75f, 1f), Color.HSVToRGB(0.58f, 0.55f, 0.85f), Color.HSVToRGB(0.56f, 0.7f, 0.12f))),
    ];

    public static ShowcasePalette Default => Presets[0].Palette;

    /// <summary>
    /// The palette the user picked. Rainbow mode rotates its hues without touching it.
    /// </summary>
    public static readonly State<ShowcasePalette> BasePalette = StateUtils.Remember(Default);

    /// <summary>
    /// The palette components should read.
    /// </summary>
    public static readonly State<ShowcasePalette> Palette = StateUtils.Remember(Default);

    public static readonly State<bool> Rainbow = StateUtils.Remember(false);

    /// <summary>
    /// Hue turns per second.
    /// </summary>
    public static readonly State<float> RainbowSpeed = StateUtils.Remember(0.1f);

    private static float _hueShift;

    static ShowcaseTheme() {
        BasePalette.AddCallback(_ => Apply(), lazy: true);

        Rainbow.AddCallback(x => {
            if (!x) {
                _hueShift = 0f;
            }

            Apply();
        }, lazy: true);
    }

    public static void SetPrimary(Color color) => BasePalette.Value = BasePalette.Value with { Primary = color };
    public static void SetSecondary(Color color) => BasePalette.Value = BasePalette.Value with { Secondary = color };
    public static void SetSurface(Color color) => BasePalette.Value = BasePalette.Value with { Surface = color };

    /// <summary>
    /// Advances the rainbow, called once per frame by the showcase root.
    /// </summary>
    public static void Tick() {
        if (!Rainbow.Value) {
            return;
        }

        _hueShift = Mathf.Repeat(_hueShift + Time.deltaTime * RainbowSpeed.Value, 1f);
        Apply();
    }

    private static void Apply() {
        var p = BasePalette.Value;

        Palette.Value = new(
            ShiftHue(p.Primary, _hueShift),
            ShiftHue(p.Secondary, _hueShift),
            ShiftHue(p.Surface, _hueShift)
        );
    }

    public static Color ShiftHue(Color color, float shift) {
        Color.RGBToHSV(color, out var h, out var s, out var v);

        return Color.HSVToRGB(Mathf.Repeat(h + shift, 1f), s, v) with { a = color.a };
    }
}
