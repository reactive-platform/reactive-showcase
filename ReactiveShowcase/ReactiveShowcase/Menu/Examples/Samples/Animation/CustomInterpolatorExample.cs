using System;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class CustomInterpolatorExample : Example {
    public override string Title => "Custom interpolators";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override bool Centered => true;
    public override string Summary => "Animating through hue instead of RGB with your own IValueInterpolator.";

    public override string Explanation =>
        "RememberAnimated works for any type you can interpolate. Colors lerp in RGB by default, which goes through " +
        "muddy grays between opposite colors. HsvInterpolator walks the hue circle the short way instead. Both " +
        "swatches get the same target and duration, so the only difference is the interpolator.";

    public override IReactiveComponent CreateDemo() => new CustomInterpolatorDemo();
}

internal class CustomInterpolatorDemo : ReactiveComponent {
    private static readonly Color start = new(1f, 0.2f, 0.2f);

    protected override GameObject Construct() {
        var rgb = RememberAnimated(start, 1200.ms, AnimationCurve.EaseInOutSine);
        var hsv = RememberAnimated(start, HsvInterpolator.Instance, 1200.ms, AnimationCurve.EaseInOutSine);

        void SetTarget(Color color) {
            rgb.TargetValue = color;
            hsv.TargetValue = color;
        }

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        Gap = 4.pt
                    },

                    Children = {
                        CreateSwatch("RGB", rgb),
                        CreateSwatch("HSV", hsv)
                    }
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        CreateTarget("Red", start, SetTarget),
                        CreateTarget("Cyan", new Color(0.2f, 1f, 1f), SetTarget),
                        CreateTarget("Blue", new Color(0.2f, 0.3f, 1f), SetTarget),
                        CreateTarget("Yellow", new Color(1f, 0.9f, 0.2f), SetTarget)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private static Layout CreateSwatch(string caption, AnimatedState<Color> color) {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Background {
                    FlexItem = {
                        Size = 18.pt
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = color
                },

                new Label {
                    Text = caption,
                    FontSize = 3.5f
                }
            }
        };
    }

    private static BsTextButton CreateTarget(string text, Color color, Action<Color> onClick) {
        return new BsTextButton {
            Text = text,
            OnClick = () => onClick(color)
        };
    }
}

/// <summary>
/// Interpolates hue, saturation and value separately, taking the shorter way around the hue circle.
/// </summary>
internal class HsvInterpolator : IValueInterpolator<Color> {
    public static readonly HsvInterpolator Instance = new();

    public Color Lerp(Color from, Color to, float t) {
        Color.RGBToHSV(from, out var h1, out var s1, out var v1);
        Color.RGBToHSV(to, out var h2, out var s2, out var v2);

        // From -0.5 to 0.5, so the hue never goes the long way
        var hueDelta = Mathf.Repeat(h2 - h1 + 0.5f, 1f) - 0.5f;

        return Color.HSVToRGB(
            Mathf.Repeat(h1 + hueDelta * t, 1f),
            Mathf.Lerp(s1, s2, t),
            Mathf.Lerp(v1, v2, t)
        );
    }

    public bool Equals(Color x, Color y) => x == y;

    public int GetHashCode(Color obj) => obj.GetHashCode();
}
