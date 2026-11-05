using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class ColorPickerPage : ShowcasePage {
    private BsColorCircle _sweepCircle = null!;
    private bool _sweeping;
    private float _sweepHue;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var live = Remember(ShowcaseTheme.BasePalette.Value.Primary);
        var saved = Remember(live.Value);

        var pickerContext = new BsColorPickerContext { SelectedColor = ShowcaseTheme.BasePalette.Value.Secondary };
        var pickerAnchor = Ref<RectTransform>();
        var modalAnimated = Remember(true);
        var modalBackground = Remember(true);

        var buttonColors = Palette.Map(ShowcaseStyles.Button);

        return [
            Section(
                "Color circle",
                "OnColorUpdated fires while dragging, OnColorSaved once the handle is released. The bar fades from the saved color to the live one.",
                Row(
                    new BsColorCircle {
                        FlexItem = { Size = 32.pt },
                        Color = live.Value,
                        OnColorUpdated = x => live.Value = x,
                        OnColorSaved = x => saved.Value = x
                    },

                    Column(
                        new Background {
                            FlexItem = {
                                Size = new() { x = 40.pt, y = 8.pt }
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 8f,
                            Color = Color.white,
                            UseGradient = true,
                            GradientDirection = HMUI.ImageView.GradientDirection.Horizontal,
                            sGradientColor0 = saved,
                            sGradientColor1 = live
                        },

                        new Label {
                            FontSize = 3f,
                            sText = RememberDerived(saved, live, (saved, live) => $"Saved {Format(saved)}  Live {Format(live)}"),
                        },

                        Row(
                            new BsTextButton {
                                Text = "Primary",
                                sColors = buttonColors,
                                OnClick = () => ShowcaseTheme.SetPrimary(live.Value)
                            },

                            new BsTextButton {
                                Text = "Secondary",
                                sColors = buttonColors,
                                OnClick = () => ShowcaseTheme.SetSecondary(live.Value)
                            },

                            new BsTextButton {
                                Text = "Surface",
                                sColors = buttonColors,
                                OnClick = () => ShowcaseTheme.SetSurface((live.Value * 0.3f) with { a = 1f })
                            }
                        )
                    )
                )
            ),

            Section(
                "Picker with a modal",
                "BsColorPicker shows the selected color, the modal edits the same context. Both modal options apply on the next open.",
                Row(
                    new BsColorPicker {
                        Context = pickerContext,
                        sColors = Palette.Map(ShowcaseStyles.ColorPicker)
                    }.Bind(pickerAnchor),

                    new Label {
                        Alignment = TextAlignmentOptions.Left,
                        FontSize = 3f,
                        sText = pickerContext.Map(x => $"Live: {Format(x.Color)}\nSelected: {Format(x.SelectedColor)}\nFocused: {x.Focused}")
                    },

                    Column(
                        ToggleRow("Animated", true, x => modalAnimated.Value = x),
                        ToggleRow("Background", true, x => modalBackground.Value = x)
                    )
                ),

                new BsColorCircleModal {
                    Context = pickerContext,
                    PlacementAnchor = pickerAnchor,
                    sAnimated = modalAnimated,
                    sShowBackground = modalBackground,
                    Title = "Pick a color",
                    PlacementData = new() {
                        Placement = RelativePlacement.RightCenter,
                        Offset = new(2f, 0f)
                    }
                }
            ),

            Section(
                "Driven from code",
                "Setting Color moves the handle like a completed user change. The sweep rotates the hue every frame.",
                Row(
                    new BsColorCircle {
                        FlexItem = { Size = 24.pt },
                        Color = Color.red
                    }.Bind(ref _sweepCircle),

                    ToggleRow("Sweep", false, x => _sweeping = x)
                )
            ),

            Sizes(
                "The color circle keeps its aspect and centers itself, the picker button stretches.",
                CreateSizeSample,
                new(8f, 8f),
                new(40f, 40f),
                new(12f, 12f), new(20f, 20f), new(30f, 30f)
            )
        ];
    }

    private ILayoutItem CreateSizeSample() {
        return new BsColorCircle {
            Color = ShowcaseTheme.BasePalette.Value.Primary
        };
    }

    protected override void OnUpdate() {
        if (!_sweeping) {
            return;
        }

        _sweepHue = Mathf.Repeat(_sweepHue + Time.deltaTime * 0.25f, 1f);
        _sweepCircle.Color = Color.HSVToRGB(_sweepHue, 0.5f + 0.5f * Mathf.PingPong(Time.time * 0.5f, 1f), 1f);
    }
}
