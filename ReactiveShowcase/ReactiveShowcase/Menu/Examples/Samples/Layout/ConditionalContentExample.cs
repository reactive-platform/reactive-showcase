using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class ConditionalContentExample : Example {
    public override string Title => "Showing and hiding";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override string Summary => "sEnabled, and keeping the space of what's hidden or not.";

    public override string Explanation =>
        "Binding sEnabled to a state shows and hides a component. A disabled component normally leaves the layout, " +
        "so everything after it moves up. With WithinLayoutIfDisabled it keeps its space instead, which avoids " +
        "content jumping around. Both columns read the same toggle.";

    public override IReactiveComponent CreateDemo() => new ConditionalContentDemo();
}

internal class ConditionalContentDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var shown = Remember(true);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 1.pt
                    },

                    Children = {
                        new Label {
                            Text = "Show the middle block",
                            FontSize = 3.5f
                        },

                        new BsToggle {
                            sToggled = shown,
                            OnToggleStateChanged = x => shown.Value = x
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        AlignItems = Align.FlexStart,
                        Gap = 6.pt
                    },

                    Children = {
                        CreateColumn("Collapses", shown, false),
                        CreateColumn("Keeps its space", shown, true)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private static Layout CreateColumn(string caption, State<bool> shown, bool keepSpace) {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    Text = caption,
                    FontSize = 3.2f,
                    FontStyle = FontStyles.Bold,
                    Color = Color.white with { a = 0.7f }
                },

                CreateBlock("First", new Color(0f, 0.75f, 1f)),

                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 26.pt, y = 6.pt }
                    },

                    // The only difference between the two columns
                    WithinLayoutIfDisabled = keepSpace,
                    sEnabled = shown,

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(1f, 0.2f, 0.8f, 0.5f),

                    Children = {
                        new Label {
                            Text = "Middle",
                            FontSize = 3.2f
                        }
                    }
                },

                CreateBlock("Last", new Color(1f, 0.8f, 0.2f))
            }
        };
    }

    private static Background CreateBlock(string text, Color color) {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { x = 26.pt, y = 6.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color with { a = 0.5f },

            Children = {
                new Label {
                    Text = text,
                    FontSize = 3.2f
                }
            }
        };
    }
}
