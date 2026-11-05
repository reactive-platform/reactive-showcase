using System;
using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;
using ScrollArea = Reactive.BeatSaber.Components.ScrollArea;

namespace ReactiveShowcase.Showcase;

/// <summary>
/// A scrollable page made of sections.
/// </summary>
internal abstract class ShowcasePage : ReactiveComponent {
    protected static State<ShowcasePalette> Palette => ShowcaseTheme.Palette;

    [StateGen]
    protected abstract IEnumerable<ILayoutItem> ConstructSections();

    protected override void OnInitialize() {
        this.FlexItem.Flex = 1f;
    }

    protected override GameObject Construct() {
        var context = new ScrollContext();

        var content = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                Gap = 2.pt,
                ConstrainVertical = false
            }
        };

        foreach (var section in ConstructSections()) {
            content.Children.Add(section);
        }

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Row,
                AlignItems = Align.Stretch,
                Gap = 1.pt
            },

            Children = {
                new ScrollArea {
                    FlexItem = {
                        Flex = 1
                    },

                    ScrollContext = context,
                    ScrollContent = content
                },

                new Scrollbar {
                    FlexItem = {
                        Margin = new() { top = 9.pt }
                    },

                    ScrollContext = context
                }
            }
        }.Use();
    }
}

internal static class ShowcaseUi {
    [StateGen]
    public static Background Section(string title, string description, params ILayoutItem[] content) {
        var section = new Background {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                Padding = 2.pt,
                Gap = 1.5f.pt
            },

            Children = {
                new Label {
                    Text = title,
                    FontSize = 4.5f,
                    FontStyle = FontStyles.Bold,
                    Alignment = TextAlignmentOptions.Left,
                    sColor = ShowcaseTheme.Palette.Map(x => x.Primary)
                },

                new Label {
                    Text = description,
                    FontSize = 3f,
                    EnableWrapping = true,
                    Alignment = TextAlignmentOptions.Left,
                    Color = Color.white with { a = 0.6f }
                }
            }
        }.AsBackground(color: Color.black with { a = 0.35f });

        foreach (var item in content) {
            section.Children.Add(item);
        }

        return section;
    }

    /// <summary>
    /// A "Sizes" section with the component at a few fixed sizes and a live one resized with sliders.
    /// </summary>
    public static Background Sizes(string description, Func<ILayoutItem> factory, Vector2 min, Vector2 max, params Vector2[] presets) {
        return Section("Sizes", description, new SizeShowcase(factory, min, max, presets));
    }

    public static Layout Row(params ILayoutItem[] items) {
        var row = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Row,
                FlexWrap = Wrap.Wrap,
                JustifyContent = Justify.FlexStart,
                AlignItems = Align.Center,
                Gap = 2.pt
            }
        };

        foreach (var item in items) {
            row.Children.Add(item);
        }

        return row;
    }

    public static Layout Column(params ILayoutItem[] items) {
        var column = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.pt
            }
        };

        foreach (var item in items) {
            column.Children.Add(item);
        }

        return column;
    }

    public static Layout Labeled(string caption, ILayoutItem item) {
        return Column(item, Caption(caption));
    }

    public static Label Caption(string text) {
        return new Label {
            Text = text,
            FontSize = 2.8f,
            Color = Color.white with { a = 0.5f }
        };
    }

    /// <summary>
    /// A text with a toggle on its right.
    /// </summary>
    [StateGen]
    public static Layout ToggleRow(string text, bool initial, Action<bool> onChanged) {
        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    Text = text,
                    FontSize = 3.5f
                },

                new BsToggle {
                    Toggled = initial,
                    OnToggleStateChanged = onChanged,
                    sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.Toggle)
                }
            }
        };
    }

    /// <summary>
    /// A text with a toggle bound both ways to the state.
    /// </summary>
    [StateGen]
    public static Layout ToggleRow(string text, State<bool> state) {
        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    Text = text,
                    FontSize = 3.5f
                },

                new BsToggle {
                    sToggled = state,
                    OnToggleStateChanged = x => state.Value = x,
                    sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.Toggle)
                }
            }
        };
    }

    public static Background Swatch(Color color, float size, Action onClick) {
        return new Background {
            FlexItem = {
                Size = size.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 12f,
            Color = color
        }.WithPointerEvents(onDown: _ => onClick());
    }

    public static string Format(Color color) {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
}
