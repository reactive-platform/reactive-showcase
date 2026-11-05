using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class SegmentedControlPage : ShowcasePage {
    private enum Swatch {
        Primary,
        Secondary,
        Warm,
        Cool
    }

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.SegmentedControl);
        var skew = Remember(BeatSaberStyle.Skew);
        var interactable = Remember(true);

        var swatch = Remember(Swatch.Primary);
        var swatchColor = RememberAnimated(ShowcaseTheme.Palette.Value.Primary, 400.ms, AnimationCurve.EaseOutCubic);

        RememberDerived(swatch, Palette, ResolveSwatch)
            .AddCallback(x => swatchColor.TargetValue = x);

        return [
            Section(
                "Orientation",
                "Cells pick rounded corners from the layout direction, so the same control works horizontally and vertically.",
                Row(
                    Labeled("Text", new BsSegmentedControl<int> {
                        FlexItem = {
                            Size = new() { x = 50.pt, y = 7.pt }
                        },

                        sColors = colors,
                        Key = 1,
                        Items = new Dictionary<int, BsSegmentedControlItem> {
                            [0] = new("Easy", null),
                            [1] = new("Hard", null),
                            [2] = new("Expert+", null)
                        }
                    }),

                    Labeled("Vertical", new BsSegmentedControl<int> {
                        FlexController = {
                            FlexDirection = FlexDirection.Column,
                            Gap = 0.5f.pt
                        },

                        FlexItem = {
                            Size = new() { x = 24.pt, y = 24.pt }
                        },

                        sColors = colors,
                        Key = 0,
                        Items = new Dictionary<int, BsSegmentedControlItem> {
                            [0] = new("Edit", GameResources.EditIcon),
                            [1] = new("Search", GameResources.SearchIcon),
                            [2] = new("Close", GameResources.CloseIcon)
                        }
                    }),

                    Labeled("Icons", new BsSegmentedControl<int> {
                        FlexItem = {
                            Size = new() { x = 30.pt, y = 7.pt }
                        },

                        sColors = colors,
                        Key = 0,
                        Items = new Dictionary<int, BsSegmentedControlItem> {
                            [0] = new(null, GameResources.ArrowIcon),
                            [1] = new(null, GameResources.CircleIcon),
                            [2] = new(null, GameResources.CaretIcon)
                        }
                    })
                )
            ),

            Section(
                "Skew and interactable",
                "Skew is a plain float, so it can be bound to anything. Italics kick in once it's above zero.",
                new BsSegmentedControl<int> {
                    FlexItem = {
                        Size = new() { x = 70.pt, y = 7.pt }
                    },

                    sColors = colors,
                    sSkew = skew,
                    sInteractable = interactable,
                    Key = 2,
                    Items = new Dictionary<int, BsSegmentedControlItem> {
                        [0] = new("Alpha", null),
                        [1] = new("Beta", null),
                        [2] = new("Gamma", null),
                        [3] = new("Delta", null)
                    }
                },
                Row(
                    Labeled("Skew", new BsSlider {
                        sColors = Palette.Map(ShowcaseStyles.Slider),
                        MinValue = -0.4f,
                        MaxValue = 0.4f,
                        ValueStep = 0.02f,
                        Value = BeatSaberStyle.Skew,
                        OnValueChanged = x => skew.Value = x
                    }),
                    ToggleRow("Interactable", true, x => interactable.Value = x)
                )
            ),

            Section(
                "Driving content",
                "The key drives a KeyedContainer and an animated color, so switching cross-fades the swatch.",
                Row(
                    new BsSegmentedControl<Swatch> {
                        FlexItem = {
                            Size = new() { x = 60.pt, y = 7.pt }
                        },

                        sColors = colors,
                        Key = Swatch.Primary,
                        OnKeyChanged = x => swatch.Value = x,
                        Items = new Dictionary<Swatch, BsSegmentedControlItem> {
                            [Swatch.Primary] = new("Primary", null),
                            [Swatch.Secondary] = new("Secondary", null),
                            [Swatch.Warm] = new("Warm", null),
                            [Swatch.Cool] = new("Cool", null)
                        }
                    },

                    new Background {
                        FlexItem = { Size = 10.pt },
                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 8f,
                        sColor = swatchColor
                    },

                    new KeyedContainer<Swatch> {
                        FlexController = {
                            AlignItems = Align.Center
                        },

                        FlexItem = {
                            Size = new() { x = 30.pt, y = 8.pt }
                        },

                        sKey = swatch,
                        Views = new Dictionary<Swatch, IReactiveComponent> {
                            [Swatch.Primary] = CreateCaption("Your main color"),
                            [Swatch.Secondary] = CreateCaption("Used for accents"),
                            [Swatch.Warm] = CreateCaption("Sunny orange"),
                            [Swatch.Cool] = CreateCaption("Icy teal")
                        }
                    }
                )
            ),

            Sizes(
                "Cells split the width evenly, the selection highlight follows them.",
                CreateSizeSample,
                new(20f, 4f),
                new(80f, 14f),
                new(30f, 5f), new(50f, 7f), new(70f, 10f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsSegmentedControl<int> {
            sColors = Palette.Map(ShowcaseStyles.SegmentedControl),
            Key = 1,
            Items = new Dictionary<int, BsSegmentedControlItem> {
                [0] = new("Easy", null),
                [1] = new("Hard", null),
                [2] = new("Expert+", null)
            }
        };
    }

    private static Color ResolveSwatch(Swatch swatch, ShowcasePalette palette) {
        return swatch switch {
            Swatch.Primary => palette.Primary,
            Swatch.Secondary => palette.Secondary,
            Swatch.Warm => new(1f, 0.55f, 0.1f),
            _ => new(0.2f, 0.9f, 0.9f)
        };
    }

    private static IReactiveComponent CreateCaption(string text) {
        return new Label {
            Text = text,
            FontSize = 3.5f,
            Color = Color.white with { a = 0.8f }
        };
    }
}
