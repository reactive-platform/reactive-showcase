using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class BuildingBlocksExample : Example {
    public override string Title => "Composing UI";
    public override Topic Topic => Topic.Introduction;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override bool Centered => true;
    public override string Summary => "The same card written inline, as a helper method and as a component.";

    public override string Explanation =>
        "UI is plain C#, so it composes like code does. Write a tree inline where it's used once, move it into a " +
        "method when it repeats, and make it a component when it needs its own state or a public API. All three " +
        "cards below look the same, only the last one keeps a click count of its own.";

    public override IReactiveComponent CreateDemo() => new BuildingBlocksDemo();
}

internal class BuildingBlocksDemo : ReactiveComponent {
    protected override GameObject Construct() {
        return new Layout {
            FlexController = {
                AlignItems = Align.FlexStart,
                Gap = 3.pt
            },

            Children = {
                // 1. Inline: right where it's used
                new Background {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Center,
                        Padding = 2.pt,
                        Gap = 0.5f.pt
                    },

                    FlexItem = {
                        Size = new() { x = 20.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.1f },

                    Children = {
                        new Label {
                            Text = "Inline",
                            FontSize = 3.5f,
                            FontStyle = FontStyles.Bold
                        },

                        new Label {
                            Text = "used once",
                            FontSize = 2.6f,
                            Color = Color.white with { a = 0.6f }
                        }
                    }
                },

                // 2. A method: reused with different arguments
                CreateCard("Method", "reused often"),

                // 3. A component: has state and an API of its own
                new CounterCard {
                    Title = "Component"
                }
            }
        }.Use();
    }

    private static Background CreateCard(string title, string subtitle) {
        return new Background {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Padding = 2.pt,
                Gap = 0.5f.pt
            },

            FlexItem = {
                Size = new() { x = 20.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = Color.white with { a = 0.1f },

            Children = {
                new Label {
                    Text = title,
                    FontSize = 3.5f,
                    FontStyle = FontStyles.Bold
                },

                new Label {
                    Text = subtitle,
                    FontSize = 2.6f,
                    Color = Color.white with { a = 0.6f }
                }
            }
        };
    }
}

/// <summary>
/// The same card, but it counts the clicks it gets.
/// </summary>
internal class CounterCard : ReactiveComponent {
    public string Title {
        get => _title.Text;
        set => _title.Text = value;
    }

    private Label _title = null!;

    protected override GameObject Construct() {
        var clicks = Remember(0);

        return new Background {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Padding = 2.pt,
                Gap = 0.5f.pt
            },

            FlexItem = {
                Size = new() { x = 20.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = Color.white with { a = 0.1f },

            Do = x => x.WithPointerEvents(onDown: _ => clicks.Value++),

            Children = {
                new Label {
                    FontSize = 3.5f,
                    FontStyle = FontStyles.Bold
                }.Bind(ref _title),

                new Label {
                    FontSize = 2.6f,
                    Color = Color.white with { a = 0.6f },
                    sText = clicks.Map(x => $"clicked {x} times")
                }
            }
        }.Use();
    }
}
