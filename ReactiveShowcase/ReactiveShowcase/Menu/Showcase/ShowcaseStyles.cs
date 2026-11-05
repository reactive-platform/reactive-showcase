using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using UnityEngine;

namespace ReactiveShowcase.Showcase;

/// <summary>
/// Maps the palette into component colors, starting from the stock Beat Saber styles.
/// </summary>
internal static class ShowcaseStyles {
    public static BsButtonColors Button(ShowcasePalette p) {
        var d = BeatSaberStyle.BsButtonColors;

        return d with {
            BackgroundColors = d.BackgroundColors with {
                Colors = d.BackgroundColors.Colors with {
                    HoveredColor = p.Primary with { a = 0.35f },
                    ActiveColor = p.Primary with { a = 0.6f }
                }
            },
            UnderlineColors = new() {
                Color = p.Primary with { a = 0.8f }
            }
        };
    }

    public static BsPrimaryButtonColors PrimaryButton(ShowcasePalette p) {
        var d = BeatSaberStyle.BsPrimaryButtonColors;

        return d with {
            BackgroundColors = d.BackgroundColors with {
                GradientColors0 = d.BackgroundColors.GradientColors0 with {
                    Color = p.Primary,
                    HoveredColor = Color.Lerp(p.Primary, Color.white, 0.3f)
                },
                GradientColors1 = d.BackgroundColors.GradientColors1 with {
                    Color = p.Secondary with { a = 0.6f },
                    HoveredColor = Color.Lerp(p.Secondary, Color.white, 0.3f) with { a = 0.6f }
                }
            },
            BorderColors = d.BorderColors with { Color = p.Primary with { a = 0.7f } },
            OutlineColors = d.OutlineColors with { Color = p.Primary with { a = 0.3f } }
        };
    }

    public static BsAeroButtonColors AeroButton(ShowcasePalette p) {
        var d = BeatSaberStyle.BsAeroButtonColors;

        return d with {
            BackgroundColors = d.BackgroundColors with {
                Colors = d.BackgroundColors.Colors with {
                    HoveredColor = p.Secondary
                }
            }
        };
    }

    public static BsToggleColors Toggle(ShowcasePalette p) {
        var d = BeatSaberStyle.BsToggleColors;

        return d with {
            BackgroundColors = d.BackgroundColors with { HoveredColor = p.Primary with { a = 0.3f } },
            KnobColors = d.KnobColors with { ActiveColor = p.Primary }
        };
    }

    public static BsSliderColors Slider(ShowcasePalette p) {
        var d = BeatSaberStyle.BsSliderColors;

        return d with {
            BackgroundColors = d.BackgroundColors with { HoveredColor = p.Secondary with { a = 0.5f } },
            HandleColors = d.HandleColors with { Color = p.Primary }
        };
    }

    public static BsSegmentedControlColors SegmentedControl(ShowcasePalette p) {
        var d = BeatSaberStyle.BsSegmentedControlColors;

        return d with {
            CellContentColors = d.CellContentColors with { ActiveColor = p.Primary },
            CellBackgroundColors = d.CellBackgroundColors with {
                HoveredColor = p.Primary with { a = 0.2f },
                ActiveColor = p.Primary with { a = 0.15f }
            }
        };
    }

    public static BsDropdownColors Dropdown(ShowcasePalette p) {
        var d = BeatSaberStyle.BsDropdownColors;

        return d with {
            ButtonBackgroundColors = d.ButtonBackgroundColors with { HoveredColor = p.Primary with { a = 0.3f } },
            ItemContentColors = d.ItemContentColors with { ActiveColor = p.Primary },
            ItemBackgroundColors = d.ItemBackgroundColors with { HoveredColor = p.Primary with { a = 0.2f } }
        };
    }

    public static BsListControlColors ListControl(ShowcasePalette p) {
        var d = BeatSaberStyle.BsListControlColors;

        return d with {
            ContentColors = d.ContentColors with { Color = Color.Lerp(Color.white, p.Primary, 0.5f) },
            LeftButtonColors = d.LeftButtonColors with {
                Colors = d.LeftButtonColors.Colors with { HoveredColor = p.Primary }
            },
            RightButtonColors = d.RightButtonColors with {
                Colors = d.RightButtonColors.Colors with { HoveredColor = p.Primary }
            }
        };
    }

    public static BsInputFieldColors InputField(ShowcasePalette p) {
        var d = BeatSaberStyle.BsInputFieldColors;

        return d with {
            IconColors = d.IconColors with { Color = p.Primary },
            UnderlineColors = d.UnderlineColors with { Color = p.Primary with { a = 0.6f } }
        };
    }

    public static BsKeyboardColors Keyboard(ShowcasePalette p) {
        var d = BeatSaberStyle.BsKeyboardColors.KeyColors;

        return new() {
            KeyColors = d with {
                BackgroundColors = d.BackgroundColors with {
                    GradientColors1 = d.BackgroundColors.GradientColors1 with { HoveredColor = p.Primary }
                },
                BorderColors = d.BorderColors with { HoveredColor = p.Secondary }
            }
        };
    }

    public static BsNumpadColors Numpad(ShowcasePalette p) {
        return new() {
            KeyColors = Keyboard(p).KeyColors
        };
    }

    public static BsColorPickerColors ColorPicker(ShowcasePalette p) {
        var d = BeatSaberStyle.BsColorPickerColors;

        return d with {
            IconColors = d.IconColors with { Color = p.Primary }
        };
    }
}
