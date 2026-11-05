using System;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Showcase;

internal class SizeShowcase : ReactiveComponent {
    private readonly Func<ILayoutItem> _factory;
    private readonly Vector2 _min;
    private readonly Vector2 _max;
    private readonly Vector2[] _presets;

    /// <param name="factory">Creates a fresh sample, called once per preset and once for the live sample.</param>
    /// <param name="min">The smallest size the sliders allow.</param>
    /// <param name="max">The largest size the sliders allow, also the size of the live stage.</param>
    /// <param name="presets">Fixed sizes shown side by side. The live sample starts at the middle one.</param>
    public SizeShowcase(Func<ILayoutItem> factory, Vector2 min, Vector2 max, params Vector2[] presets) : base(false) {
        _factory = factory;
        _min = min;
        _max = max;
        _presets = presets;

        ConstructAndInit();
    }

    [StateGen]
    protected override GameObject Construct() {
        var initial = _presets.Length > 0 ? _presets[_presets.Length / 2] : (_min + _max) / 2f;

        var size = RememberAnimated(initial, 220.ms, AnimationCurve.EaseOutCubic);
        var chipColors = Palette.Map(ShowcaseStyles.Button);

        var presets = Row();

        foreach (var preset in _presets) {
            var item = _factory();
            item.FlexItem.Size = ToYoga(preset);

            presets.Children.Add(Labeled(FormatSize(preset), item));
        }

        var sample = _factory();
        var modifier = sample.FlexItem;

        modifier.Size = ToYoga(initial);
        size.AddCallbackWeak(modifier, (x, y) => x.Size = ToYoga(y));

        BsSlider widthSlider = null!;
        BsSlider heightSlider = null!;

        var chips = Row();

        foreach (var preset in _presets) {
            chips.Children.Add(
                new BsTextButton {
                    Text = FormatSize(preset),
                    sColors = chipColors,
                    OnClick = () => {
                        // Sliders report back through OnValueChanged, which moves the sample
                        widthSlider.Value = preset.x;
                        heightSlider.Value = preset.y;
                    }
                }
            );
        }

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.FlexStart,
                Gap = 2.pt
            },

            Children = {
                presets,

                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 3.pt
                    },

                    Children = {
                        new Background {
                            FlexItem = {
                                Size = ToYoga(_max)
                            },

                            FlexController = {
                                JustifyContent = Justify.Center,
                                AlignItems = Align.Center
                            },

                            Children = {
                                sample
                            }
                        }.AsBackground(color: Color.white with { a = 0.04f }),

                        Column(
                            CreateSlider("Width", _min.x, _max.x, initial.x, x => size.TargetValue = size.TargetValue with { x = x })
                                .Bind(ref widthSlider),
                            CreateSlider("Height", _min.y, _max.y, initial.y, x => size.TargetValue = size.TargetValue with { y = x })
                                .Bind(ref heightSlider),
                            chips
                        )
                    }
                }
            }
        }.Use();
    }

    private static State<ShowcasePalette> Palette => ShowcaseTheme.Palette;

    [StateGen]
    private BsSlider CreateSlider(string label, float min, float max, float value, Action<float> onChanged) {
        return new BsSlider {
            FlexItem = {
                Size = new() { x = 45.pt, y = 6.pt }
            },

            sColors = Palette.Map(ShowcaseStyles.Slider),
            MinValue = min,
            MaxValue = max,
            ValueStep = 1f,
            Value = value,
            ValueFormatter = x => $"{label} {x:0}",
            OnValueChanged = onChanged
        };
    }

    private static YogaVector ToYoga(Vector2 size) {
        return new(size.x.pt, size.y.pt);
    }

    private static string FormatSize(Vector2 size) {
        return $"{size.x:0}×{size.y:0}";
    }
}