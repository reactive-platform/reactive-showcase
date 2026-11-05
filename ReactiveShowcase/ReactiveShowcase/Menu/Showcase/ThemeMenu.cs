using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Components.Basic;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class ThemeMenu : ModalBase {
    private enum Slot {
        Primary,
        Secondary,
        Surface
    }

    public System.Action? OnCloseClicked { get; set; }

    private BsColorCircle _circle = null!;
    private State<Slot> _slot = null!;
    private bool _syncing;
    private bool _editing;

    private const float BodyHeight = 34f;
    private const float ControlsWidth = 48f;

    [StateGen]
    protected override IReactiveComponent ConstructContent() {
        var palette = ShowcaseTheme.Palette;
        _slot = Remember(Slot.Primary);

        var slotColors = RememberDerived(
            palette, _slot,
            (p, slot) => ShowcaseStyles.SegmentedControl(p with { Primary = Readable(Pick(p, slot)) })
        );

        var presets = new Repeater<(string Name, ShowcasePalette Palette), BsButton> {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                JustifyContent = Justify.SpaceBetween,
                AlignItems = Align.Stretch
            },

            FlexItem = {
                Size = new() { x = 26.pt, y = BodyHeight.pt }
            },

            SelectionMode = SelectionMode.None,
            Items = ShowcaseTheme.Presets,
            ConstructCell = CreatePreset
        };

        // Presets and other pages change the palette too, so the circle follows it
        ShowcaseTheme.BasePalette.AddCallback(this, static (x, _) => x.SyncCircle(), lazy: true);

        return new Layout {
            FlexController = {
                ConstrainHorizontal = false,
                ConstrainVertical = false
            },

            Children = {
                new Background {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Stretch,
                        Padding = 3.pt,
                        Gap = 2.pt
                    },

                    Children = {
                        new Layout {
                            FlexController = {
                                AlignItems = Align.Center,
                                JustifyContent = Justify.SpaceBetween
                            },

                            Children = {
                                new Layout {
                                    FlexController = {
                                        AlignItems = Align.Center,
                                        Gap = 2.pt
                                    },

                                    Children = {
                                        new Label {
                                            Text = "Palette",
                                            FontSize = 4.2f,
                                            FontStyle = FontStyles.Bold,
                                            Alignment = TextAlignmentOptions.Capline,
                                            sColor = palette.Map(x => x.Primary)
                                        },

                                        new Layout {
                                            FlexController = { Gap = 0.75f.pt },

                                            Children = {
                                                CreatePreview(palette.Map(x => x.Primary)),
                                                CreatePreview(palette.Map(x => x.Secondary)),
                                                CreatePreview(palette.Map(x => x.Surface))
                                            }
                                        }
                                    }
                                },

                                new BsImageButton {
                                    FlexItem = { Size = 6.pt },
                                    Sprite = GameResources.CloseIcon,
                                    ShowUnderline = false,
                                    sColors = palette.Map(ShowcaseStyles.Button),
                                    OnClick = () => OnCloseClicked?.Invoke()
                                }
                            }
                        },

                        new Layout {
                            FlexController = {
                                AlignItems = Align.Stretch,
                                Gap = 2.pt
                            },

                            Children = {
                                CreateSection("Presets", presets),

                                CreateSection(
                                    "Colors",
                                    new Layout {
                                        FlexController = {
                                            AlignItems = Align.Stretch,
                                            Gap = 3.pt
                                        },

                                        FlexItem = {
                                            Size = new() { y = BodyHeight.pt }
                                        },

                                        Children = {
                                            new BsColorCircle {
                                                FlexItem = { Size = BodyHeight.pt },
                                                Color = ShowcaseTheme.BasePalette.Value.Primary,
                                                OnColorUpdated = HandleColorUpdated
                                            }.Bind(ref _circle),

                                            new Layout {
                                                FlexController = {
                                                    FlexDirection = FlexDirection.Column,
                                                    JustifyContent = Justify.Center,
                                                    AlignItems = Align.Stretch,
                                                    Gap = 2.pt
                                                },

                                                FlexItem = {
                                                    Size = new() { x = ControlsWidth.pt }
                                                },

                                                Children = {
                                                    new BsSegmentedControl<Slot> {
                                                        FlexItem = {
                                                            Size = new() { y = 6.pt }
                                                        },

                                                        sColors = slotColors,
                                                        Key = Slot.Primary,
                                                        OnKeyChanged = x => {
                                                            _slot.Value = x;
                                                            SyncCircle();
                                                        },
                                                        Items = new Dictionary<Slot, BsSegmentedControlItem> {
                                                            [Slot.Primary] = new("Primary", null),
                                                            [Slot.Secondary] = new("Secondary", null),
                                                            [Slot.Surface] = new("Surface", null)
                                                        }
                                                    },

                                                    new BsToggle {
                                                        sToggled = ShowcaseTheme.Rainbow,
                                                        OnToggleStateChanged = x => ShowcaseTheme.Rainbow.Value = x,
                                                        sColors = palette.Map(ShowcaseStyles.Toggle)
                                                    }.InNamedRail("Rainbow"),

                                                    new BsSlider {
                                                        sColors = palette.Map(ShowcaseStyles.Slider),
                                                        sInteractable = ShowcaseTheme.Rainbow,
                                                        MinValue = 0.02f,
                                                        MaxValue = 1f,
                                                        ValueStep = 0.02f,
                                                        Value = ShowcaseTheme.RainbowSpeed.Value,
                                                        ValueFormatter = x => $"{x:0.00} turns/s",
                                                        OnValueChanged = x => ShowcaseTheme.RainbowSpeed.Value = x
                                                    }.InNamedRail("Speed")
                                                }
                                            }
                                        }
                                    }
                                )
                            }
                        }
                    }
                }.AsBlurBackground()
            }
        };
    }

    private static Background CreateSection(string title, ILayoutItem content) {
        return new Background {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                Padding = 2.pt,
                Gap = 1.5f.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            Material = GameResources.UINoGlowMaterial,
            ImageType = UnityEngine.UI.Image.Type.Sliced,
            PixelsPerUnit = 10f,
            Color = Color.black with { a = 0.35f },

            Children = {
                new Label {
                    Text = title,
                    FontSize = 3.2f,
                    FontStyle = FontStyles.Bold,
                    Alignment = TextAlignmentOptions.CaplineLeft,
                    Color = Color.white with { a = 0.6f }
                },

                content
            }
        };
    }

    private void HandleColorUpdated(Color color) {
        if (_syncing) {
            return;
        }

        // The palette change comes back through the callback, which shouldn't move the handle being dragged
        _editing = true;

        switch (_slot.Value) {
            case Slot.Primary:
                ShowcaseTheme.SetPrimary(color);
                break;
            case Slot.Secondary:
                ShowcaseTheme.SetSecondary(color);
                break;
            default:
                ShowcaseTheme.SetSurface(color);
                break;
        }

        _editing = false;
    }

    private void SyncCircle() {
        if (_editing) {
            return;
        }

        _syncing = true;
        _circle.Color = Pick(ShowcaseTheme.BasePalette.Value, _slot.Value);
        _syncing = false;
    }

    private static Color Pick(ShowcasePalette palette, Slot slot) {
        return slot switch {
            Slot.Primary => palette.Primary,
            Slot.Secondary => palette.Secondary,
            _ => palette.Surface
        };
    }

    // Surfaces are usually close to black, which an accent can't be
    private static Color Readable(Color color) {
        Color.RGBToHSV(color, out var h, out var s, out var v);

        return Color.HSVToRGB(h, s, Mathf.Max(v, 0.6f));
    }

    [StateGen]
    private static BsButton CreatePreset(CellContext<(string Name, ShowcasePalette Palette)> context) {
        return new BsButton {
            FlexController = {
                Padding = new() { left = 1.5f.pt, right = 1.5f.pt },
                JustifyContent = Justify.FlexStart,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { y = 5.pt }
            },

            ShowUnderline = false,
            sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.Button),
            OnClick = () => ShowcaseTheme.BasePalette.Value = context.Item.Palette,

            ConstructContent = (color, _) => new Layout {
                FlexItem = {
                    FlexShrink = 1f
                },

                FlexController = {
                    AlignItems = Align.Center,
                    Gap = 0.5f.pt
                },

                Children = {
                    CreateDot(context.Map(x => x.Item.Palette.Primary)),
                    CreateDot(context.Map(x => x.Item.Palette.Secondary)),

                    new Label {
                        FlexItem = {
                            Margin = new() { left = 0.5f.pt },
                            FlexShrink = 1f
                        },

                        EnableWrapping = false,
                        Overflow = TextOverflowModes.Ellipsis,
                        EnableAutoSizing = true,
                        FontSizeMin = 2f,
                        FontSizeMax = 3.2f,

                        Alignment = TextAlignmentOptions.CaplineLeft,
                        sText = context.Map(x => x.Item.Name),
                        sColor = color.In()
                    }
                }
            }
        };
    }

    [StateGen]
    private static Background CreateDot(IState<Color> color) {
        return new Background {
            FlexItem = { Size = 2.5f.pt },
            Sprite = GameResources.CircleIcon,
            sColor = color.In()
        };
    }

    [StateGen]
    private static Background CreatePreview(MappedState<ShowcasePalette, Color> color) {
        return new Background {
            FlexItem = { Size = 3.pt },
            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 12f,
            sColor = color
        };
    }
}
