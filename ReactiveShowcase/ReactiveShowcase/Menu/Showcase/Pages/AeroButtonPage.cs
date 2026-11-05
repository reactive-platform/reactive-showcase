using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Image = Reactive.BeatSaber.Components.Image;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class AeroButtonPage : ShowcasePage {
    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var interactable = Remember(true);

        var counter = RememberAnimated(0f, 700.ms, AnimationCurve.EaseOutCubic);

        var hues = Row();

        for (var i = 0; i < 6; i++) {
            var shift = i / 6f;
            var degrees = i * 60;

            hues.Children.Add(
                new BsAeroButton {
                    FlexController = {
                        Padding = new() { left = 2.pt, right = 2.pt }
                    },

                    FlexItem = {
                        Size = new() { y = 8.pt }
                    },

                    sInteractable = interactable,
                    sColors = Palette.Map(p => WithHoverHue(p, shift)),

                    ConstructContent = (color, skew) => new Label {
                        FlexItem = {
                            FlexShrink = 1f
                        },

                        EnableWrapping = false,
                        Overflow = TextOverflowModes.Ellipsis,
                        EnableAutoSizing = true,
                        FontSizeMin = 2f,
                        FontSizeMax = 4f,

                        Alignment = TextAlignmentOptions.Capline,
                        sFontStyle = skew.Map(x => x > 0 ? FontStyles.Italic : FontStyles.Normal),
                        Text = $"+{degrees}°",
                        sColor = color.In()
                    }
                }
            );
        }

        return [
            Section(
                "Hover glow",
                "Aero buttons light up with the hovered background color. Each one here shifts the primary hue a bit further.",
                hues,
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "Content",
                "Content gets the color state, so icons and text fade together with the hover.",
                Row(
                    CreateIconButton(GameResources.EditIcon, "Edit"),
                    CreateIconButton(GameResources.SearchIcon, "Search"),
                    CreateIconButton(GameResources.CloseIcon, "Close")
                )
            ),

            Section(
                "Rolling counter",
                "An animated float state with an ease-out curve, mapped into the label text.",
                Row(
                    new Label {
                        FlexItem = {
                            Size = new() { x = 20.pt }
                        },

                        FontSize = 7f,
                        sText = counter.Map(x => Mathf.RoundToInt(x).ToString()),
                        sColor = Palette.Map(x => x.Primary)
                    },

                    CreateCounterButton("+1", () => counter.TargetValue += 1f),
                    CreateCounterButton("+25", () => counter.TargetValue += 25f),
                    CreateCounterButton("+1000", () => counter.TargetValue += 1000f),
                    CreateCounterButton("Reset", () => counter.TargetValue = 0f)
                )
            ),

            Sizes(
                "The glow and the content follow the button size.",
                CreateSizeSample,
                new(10f, 4f),
                new(60f, 16f),
                new(16f, 5f), new(26f, 7f), new(42f, 11f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return CreateIconButton(GameResources.SearchIcon, "Search");
    }

    private static BsAeroButtonColors WithHoverHue(ShowcasePalette palette, float shift) {
        var colors = ShowcaseStyles.AeroButton(palette);

        return colors with {
            BackgroundColors = colors.BackgroundColors with {
                Colors = colors.BackgroundColors.Colors with {
                    HoveredColor = ShowcaseTheme.ShiftHue(palette.Primary, shift)
                }
            }
        };
    }

    [StateGen]
    private static BsAeroButton CreateIconButton(Sprite icon, string text) {
        return new BsAeroButton {
            FlexController = {
                Padding = new() { left = 2.pt, right = 2.pt },
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            FlexItem = {
                Size = new() { y = 8.pt }
            },

            sColors = Palette.Map(ShowcaseStyles.AeroButton),

            ConstructContent = (color, skew) => new Layout {
                FlexItem = {
                    Size = new() { y = 100.pct },
                    FlexShrink = 1f
                },

                FlexController = {
                    AlignItems = Align.Center,
                    Gap = 1.pt
                },

                Children = {
                    new Image {
                        FlexItem = {
                            Size = new() { y = 55.pct },
                            MaxSize = 4.pt,
                            AspectRatio = 1f
                        },

                        Sprite = icon,
                        PreserveAspect = true,
                        sColor = color.In(),
                        sSkew = skew.In()
                    },

                    new Label {
                        FlexItem = {
                            FlexShrink = 1f
                        },

                        EnableWrapping = false,
                        Overflow = TextOverflowModes.Ellipsis,
                        EnableAutoSizing = true,
                        FontSizeMin = 2f,
                        FontSizeMax = 4f,

                        Alignment = TextAlignmentOptions.Capline,
                        sFontStyle = skew.Map(x => x > 0 ? FontStyles.Italic : FontStyles.Normal),
                        Text = text,
                        sColor = color.In()
                    }
                }
            }
        };
    }

    [StateGen]
    private static BsAeroButton CreateCounterButton(string text, System.Action onClick) {
        return new BsAeroButton {
            FlexController = {
                Padding = new() { left = 2.pt, right = 2.pt }
            },

            FlexItem = {
                Size = new() { y = 7.pt }
            },

            sColors = Palette.Map(ShowcaseStyles.AeroButton),
            OnClick = onClick,

            ConstructContent = (color, skew) => new Label {
                FlexItem = {
                    FlexShrink = 1f
                },

                EnableWrapping = false,
                Overflow = TextOverflowModes.Ellipsis,
                EnableAutoSizing = true,
                FontSizeMin = 2f,
                FontSizeMax = 4f,

                Alignment = TextAlignmentOptions.Capline,
                sFontStyle = skew.Map(x => x > 0 ? FontStyles.Italic : FontStyles.Normal),
                Text = text,
                sColor = color.In()
            }
        };
    }
}
