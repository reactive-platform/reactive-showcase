using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class PaddingMarginExample : Example {
    public override string Title => "Padding and margin";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override string Summary => "Space inside a container's edges, and space around one child.";

    public override string Explanation =>
        "Padding belongs to the container: it keeps all children away from its edges. Margin belongs to a single " +
        "item: it pushes its neighbours and the edges away from that item only. The first slider pads the box, the " +
        "second gives just the middle block a margin, and the box grows to fit both.";

    public override IReactiveComponent CreateDemo() => new PaddingMarginDemo();
}

internal class PaddingMarginDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var padding = Remember(2f);
        var margin = Remember(0f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 6f,
                    ValueStep = 0.5f,
                    Value = padding.Value,
                    ValueFormatter = x => $"padding {x:0.0}",
                    OnValueChanged = x => padding.Value = x
                },

                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 6f,
                    ValueStep = 0.5f,
                    Value = margin.Value,
                    ValueFormatter = x => $"margin {x:0.0}",
                    OnValueChanged = x => margin.Value = x
                },

                new Background {
                    FlexController = {
                        AlignItems = Align.Center,
                        sPadding = padding.Map(x => new YogaFrame(x.pt))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.1f },

                    Children = {
                        CreateBlock(new Color(0f, 0.75f, 1f)),

                        new Background {
                            FlexItem = {
                                Size = 7.pt,
                                sMargin = margin.Map(x => new YogaFrame(x.pt))
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            Color = new Color(1f, 0.2f, 0.8f, 0.7f)
                        },

                        CreateBlock(new Color(1f, 0.8f, 0.2f))
                    }
                }
            }
        }.Use();
    }

    private static Background CreateBlock(Color color) {
        return new Background {
            FlexItem = {
                Size = 7.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color with { a = 0.7f }
        };
    }
}
