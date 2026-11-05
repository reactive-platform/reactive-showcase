using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class DropdownPage : ShowcasePage {
    private static readonly Dictionary<string, AnimationCurve> curves = new() {
        ["Linear"] = AnimationCurve.Linear,
        ["Ease In"] = AnimationCurve.EaseIn,
        ["Ease Out"] = AnimationCurve.EaseOut,
        ["Ease In Out"] = AnimationCurve.EaseInOut,
        ["Exponential"] = AnimationCurve.Exponential,
        ["Ease Out Quint"] = AnimationCurve.EaseOutQuint,
        ["Ease In Out Expo"] = AnimationCurve.EaseInOutExpo,
        ["Ease In Back"] = AnimationCurve.EaseInBack,
        ["Ease Out Back"] = AnimationCurve.EaseOutBack,
        ["Ease In Out Back"] = AnimationCurve.EaseInOutBack,
        ["Ease In Out Sine"] = AnimationCurve.EaseInOutSine,
        ["Ease Out Cubic"] = AnimationCurve.EaseOutCubic
    };

    private const float TrackWidth = 70f;
    private const float BlockWidth = 6f;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.Dropdown);
        var interactable = Remember(true);

        var curveName = Remember("Ease Out Back");
        var reference = RememberAnimated(0f, 900.ms);
        var progress = RememberAnimated(0f, 900.ms, curves[curveName.Value]);

        var curveItems = new Dictionary<string, BsDropdownItem>();
        foreach (var name in curves.Keys) {
            curveItems[name] = new(name, null);
        }

        return [
            Section(
                "Items",
                "Items can have icons, the list scrolls once there are more than ModalParams.MaxVisibleRows.",
                Row(
                    Labeled("Icons", new BsDropdown<int> {
                        sColors = colors,
                        sInteractable = interactable,
                        Key = 1,
                        Items = new Dictionary<int, BsDropdownItem> {
                            [0] = new("Edit", GameResources.EditIcon),
                            [1] = new("Search", GameResources.SearchIcon),
                            [2] = new("Close", GameResources.CloseIcon),
                            [3] = new("Arrow", GameResources.ArrowIcon)
                        }
                    }),

                    Labeled("3 visible rows", new BsDropdown<int> {
                        sColors = colors,
                        sInteractable = interactable,
                        ModalParams = new() { MaxVisibleRows = 3 },
                        Key = 0,
                        Items = CreateNumbered(12)
                    }),

                    Labeled("No skew", new BsDropdown<int> {
                        sColors = colors,
                        sInteractable = interactable,
                        Skew = 0f,
                        Key = 2,
                        Items = CreateNumbered(5)
                    })
                ),
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "List width",
                "ModalParams sets the opened list. Points are fixed, percents follow the field, and MinWidth and MaxWidth clamp the result. By default the list takes the field width within 24-46pt.",
                Row(
                    Labeled("Default", CreateWideDropdown(new(), curveItems)),

                    Labeled("Fixed 36pt", CreateWideDropdown(new() { Width = 36.pt }, curveItems)),

                    Labeled("100%, no limits", CreateWideDropdown(
                        new() { Width = 100.pct, MinWidth = YogaValue.Undefined, MaxWidth = YogaValue.Undefined },
                        curveItems
                    )),

                    Labeled("Dims the canvas", CreateWideDropdown(
                        new() { CompositionAnimParams = new() { DimCanvas = true } },
                        curveItems
                    ))
                )
            ),

            Section(
                "Animation curves",
                "Pick a curve and play it. The top block is linear for reference, the bottom one uses the selected curve with the same duration.",
                Row(
                    new BsDropdown<string> {
                        sColors = colors,
                        ModalParams = new() { MaxVisibleRows = 6 },
                        Key = curveName.Value,
                        Items = curveItems,
                        OnKeyChanged = x => curveName.Value = x
                    },

                    new BsPrimaryTextButton {
                        Text = "Play",
                        sColors = Palette.Map(ShowcaseStyles.PrimaryButton),
                        OnClick = () => {
                            var target = progress.TargetValue < 0.5f ? 1f : 0f;

                            progress.Curve = curves[curveName.Value];
                            progress.TargetValue = target;
                            reference.TargetValue = target;
                        }
                    }
                ),
                CreateTrack(reference, Palette.Map(x => Color.white with { a = 0.5f })),
                CreateTrack(progress, Palette.Map(x => x.Primary))
            ),

            Sizes(
                "The field stretches, the arrow stays on the right and the opened list matches the field width.",
                CreateSizeSample,
                new(16f, 4f),
                new(70f, 12f),
                new(24f, 5f), new(36f, 7f), new(56f, 9f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsDropdown<int> {
            sColors = Palette.Map(ShowcaseStyles.Dropdown),
            // Without limits, so the list always matches the field
            ModalParams = new() { Width = 100.pct, MinWidth = YogaValue.Undefined, MaxWidth = YogaValue.Undefined },
            Key = 1,
            Items = new Dictionary<int, BsDropdownItem> {
                [0] = new("Edit", GameResources.EditIcon),
                [1] = new("Search", GameResources.SearchIcon),
                [2] = new("Close", GameResources.CloseIcon)
            }
        };
    }

    [StateGen]
    private BsDropdown<string> CreateWideDropdown(BsDropdownModalParams modalParams, Dictionary<string, BsDropdownItem> items) {
        return new BsDropdown<string> {
            FlexItem = {
                Size = new() { x = 56.pt, y = 6.pt }
            },

            sColors = Palette.Map(ShowcaseStyles.Dropdown),
            ModalParams = modalParams with { MaxVisibleRows = 4 },
            Key = "Ease In Out Back",
            Items = items
        };
    }

    [StateGen]
    private static Background CreateTrack(AnimatedState<float> progress, MappedState<ShowcasePalette, Color> color) {
        return new Background {
            FlexItem = {
                Size = new() { x = TrackWidth.pt, y = 5.pt }
            },

            FlexController = {
                AlignItems = Align.Stretch
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 15f,
            Color = Color.white with { a = 0.08f },

            Children = {
                new Background {
                    FlexItem = {
                        PositionType = PositionType.Absolute,
                        Size = new() { x = BlockWidth.pt, y = 100.pct },
                        sPosition = progress.Map(x => new YogaFrame { left = (x * (TrackWidth - BlockWidth)).pt, top = 0.pt })
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 15f,
                    sColor = color
                }
            }
        };
    }

    private static Dictionary<int, BsDropdownItem> CreateNumbered(int count) {
        var items = new Dictionary<int, BsDropdownItem>();

        for (var i = 0; i < count; i++) {
            items[i] = new($"Option {i + 1}", null);
        }

        return items;
    }
}
