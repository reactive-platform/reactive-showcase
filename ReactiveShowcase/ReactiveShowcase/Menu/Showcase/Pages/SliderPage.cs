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
using Image = Reactive.BeatSaber.Components.Image;
using Label = Reactive.BeatSaber.Components.Label;
using UImage = UnityEngine.UI.Image;

namespace ReactiveShowcase.Showcase;

internal class SliderPage : ShowcasePage {
    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.Slider);

        var r = Remember(0f);
        var g = Remember(190f);
        var b = Remember(255f);
        var mixed = RememberDerived(r, g, b, (r, g, b) => new Color(r / 255f, g / 255f, b / 255f));

        var fill = RememberAnimated(0.35f, 6.fact);
        var interactable = Remember(true);

        return [
            Section(
                "Formatting",
                "Value text, step and buttons are all optional.",
                Row(
                    Labeled("Percent formatter", new BsSlider {
                        sColors = colors,
                        ValueStep = 0.01f,
                        Value = 0.5f,
                        ValueFormatter = x => $"{x * 100f:0}%"
                    }),

                    Labeled("Stepped 0-10, no buttons", new BsSlider {
                        sColors = colors,
                        MinValue = 0f,
                        MaxValue = 10f,
                        ValueStep = 1f,
                        Value = 3f,
                        ShowButtons = false
                    }),

                    Labeled("No value text", new BsSlider {
                        sColors = colors,
                        ValueStep = 0.05f,
                        Value = 0.25f,
                        ShowValueText = false
                    }),

                    Labeled("Interactable", new BsSlider {
                        sColors = colors,
                        sInteractable = interactable,
                        MinValue = -5f,
                        MaxValue = 5f,
                        ValueStep = 0.5f,
                        ValueFormatter = x => x.ToString("+0.0;-0.0;0")
                    })
                ),
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "Color mixer",
                "Each slider gets its own handle color, the swatch is derived from all three.",
                Row(
                    Column(
                        CreateChannelSlider(r, Color.red),
                        CreateChannelSlider(g, Color.green),
                        CreateChannelSlider(b, new Color(0.2f, 0.4f, 1f))
                    ),

                    Column(
                        new Background {
                            FlexItem = { Size = 16.pt },
                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 8f,
                            sColor = mixed
                        },

                        new Label {
                            FontSize = 3f,
                            sText = mixed.Map(Format)
                        }
                    ),

                    Column(
                        new BsTextButton {
                            Text = "Use as primary",
                            sColors = Palette.Map(ShowcaseStyles.Button),
                            OnClick = () => ShowcaseTheme.SetPrimary(mixed.Value)
                        },

                        new BsTextButton {
                            Text = "Use as secondary",
                            sColors = Palette.Map(ShowcaseStyles.Button),
                            OnClick = () => ShowcaseTheme.SetSecondary(mixed.Value)
                        }
                    )
                )
            ),

            Section(
                "Smoothed value",
                "The slider sets the target of an animated state, the fills follow it with a time-delta factor instead of a fixed duration.",
                Row(
                    new BsSlider {
                        sColors = colors,
                        ValueStep = 0.01f,
                        Value = 0.35f,
                        ValueFormatter = x => $"{x * 100f:0}%",
                        OnValueChanged = x => fill.TargetValue = x
                    },

                    new Image {
                        FlexItem = { Size = 14.pt },
                        Sprite = GameResources.CircleIcon,
                        PreserveAspect = true,
                        ImageType = UImage.Type.Filled,
                        FillMethod = UImage.FillMethod.Radial360,
                        sFillAmount = fill,
                        sColor = Palette.Map(x => x.Primary)
                    },

                    new Background {
                        FlexItem = {
                            Size = new() { x = 40.pt, y = 3.pt }
                        },

                        FlexController = {
                            AlignItems = Align.Stretch
                        },

                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 20f,
                        Color = Color.white with { a = 0.1f },

                        Children = {
                            new Image {
                                FlexItem = {
                                    sSize = fill.Map(x => new YogaVector((x * 100f).pct, 100.pct))
                                },

                                Sprite = BeatSaberResources.Sprites.background,
                                PixelsPerUnit = 20f,
                                UseGradient = true,
                                GradientDirection = HMUI.ImageView.GradientDirection.Horizontal,
                                Color = Color.white,
                                sGradientColor0 = Palette.Map(x => x.Primary),
                                sGradientColor1 = Palette.Map(x => x.Secondary)
                            }
                        }
                    }
                )
            ),

            Sizes(
                "The track, handle and buttons share the space, the value text stays readable.",
                CreateSizeSample,
                new(20f, 4f),
                new(80f, 12f),
                new(30f, 5f), new(45f, 6f), new(70f, 9f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsSlider {
            sColors = Palette.Map(ShowcaseStyles.Slider),
            ValueStep = 0.01f,
            Value = 0.6f
        };
    }

    [StateGen]
    private static BsSlider CreateChannelSlider(State<float> channel, Color handle) {
        return new BsSlider {
            sColors = Palette.Map(p => {
                var colors = ShowcaseStyles.Slider(p);
                return colors with { HandleColors = colors.HandleColors with { Color = handle } };
            }),

            MinValue = 0f,
            MaxValue = 255f,
            ValueStep = 1f,
            Value = channel.Value,
            OnValueChanged = x => channel.Value = x
        };
    }
}
