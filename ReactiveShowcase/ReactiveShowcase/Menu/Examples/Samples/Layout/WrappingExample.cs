using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class WrappingExample : Example {
    public override string Title => "Wrapping";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override string Summary => "Letting children flow onto new lines when a row runs out of room.";

    public override string Explanation =>
        "By default a row keeps all its children on one line, even when they don't fit. FlexWrap.Wrap starts a new " +
        "line instead, and Gap then spaces the lines too. Narrow the box and switch wrapping off and on to see the " +
        "chips overflow or flow onto more lines.";

    public override IReactiveComponent CreateDemo() => new WrappingDemo();
}

internal class WrappingDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var width = Remember(50f);
        var wrap = Remember(true);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 2.pt
                    },

                    Children = {
                        new BsSlider {
                            MinValue = 24f,
                            MaxValue = 64f,
                            ValueStep = 2f,
                            Value = width.Value,
                            ValueFormatter = x => $"width {x:0}pt",
                            OnValueChanged = x => width.Value = x
                        },

                        new Label {
                            Text = "Wrap",
                            FontSize = 3.5f
                        },

                        new BsToggle {
                            sToggled = wrap,
                            OnToggleStateChanged = x => wrap.Value = x
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Row,
                        sFlexWrap = wrap.Map(x => x ? Wrap.Wrap : Wrap.NoWrap),
                        Padding = 1.pt,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        sSize = width.Map(x => new YogaVector(x.pt, YogaValue.Auto))
                    },

                    Children = {
                        CreateChip("Expert"),
                        CreateChip("Fast"),
                        CreateChip("Tech"),
                        CreateChip("Vibro"),
                        CreateChip("Dance"),
                        CreateChip("Slow"),
                        CreateChip("Stream"),
                        CreateChip("Jumps")
                    }
                }
            }
        }.Use();
    }

    private static Background CreateChip(string text) {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center,
                Padding = new() { left = 1.5f.pt, right = 1.5f.pt }
            },

            FlexItem = {
                Size = new() { y = 5.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = new Color(0f, 0.75f, 1f, 0.5f),

            Children = {
                new Label {
                    Text = text,
                    FontSize = 3f
                }
            }
        };
    }
}
