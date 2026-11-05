using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class AbsolutePositionExample : Example {
    public override string Title => "Absolute positioning";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Taking an item out of the flow and pinning it with Position insets.";

    public override string Explanation =>
        "PositionType.Absolute takes an item out of the flow: its siblings are laid out as if it wasn't there. " +
        "Position then places it by its distance from the parent's edges, in points or percents, and only the edges " +
        "you set count. The badge's edges come from a derived state, so picking a corner or moving the inset just " +
        "produces a new YogaFrame.";

    public override IReactiveComponent CreateDemo() => new AbsolutePositionDemo();
}

internal class AbsolutePositionDemo : ReactiveComponent {
    private enum Corner {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    protected override GameObject Construct() {
        var corner = Remember(Corner.TopRight);
        var inset = Remember(1f);

        var position = RememberDerived(corner, inset, (c, i) => c switch {
            Corner.TopLeft => new YogaFrame { top = i.pt, left = i.pt },
            Corner.TopRight => new YogaFrame { top = i.pt, right = i.pt },
            Corner.BottomLeft => new YogaFrame { bottom = i.pt, left = i.pt },
            _ => new YogaFrame { bottom = i.pt, right = i.pt }
        });

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSegmentedControl<Corner> {
                    FlexItem = {
                        Size = new() { x = 60.pt, y = 5.pt }
                    },

                    Items = new Dictionary<Corner, BsSegmentedControlItem> {
                        [Corner.TopLeft] = new("Top left", null),
                        [Corner.TopRight] = new("Top right", null),
                        [Corner.BottomLeft] = new("Bottom left", null),
                        [Corner.BottomRight] = new("Bottom right", null)
                    },

                    Key = corner.Value,
                    OnKeyChanged = x => corner.Value = x
                },

                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 8f,
                    ValueStep = 1f,
                    Value = inset.Value,
                    ValueFormatter = x => $"inset {x:0}pt",
                    OnValueChanged = x => inset.Value = x
                },

                new Background {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 56.pt, y = 26.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.08f },

                    Children = {
                        new Label {
                            Text = "Flow content",
                            FontSize = 4f
                        },

                        new Label {
                            Text = "stays centered",
                            FontSize = 3f,
                            Color = Color.white with { a = 0.6f }
                        },

                        // Out of the flow, pinned by the edges in the derived frame
                        new Background {
                            FlexController = {
                                JustifyContent = Justify.Center,
                                AlignItems = Align.Center
                            },

                            FlexItem = {
                                PositionType = PositionType.Absolute,
                                Size = new() { x = 14.pt, y = 5.pt },
                                sPosition = position
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            Color = new Color(1f, 0.2f, 0.8f, 0.8f),

                            Children = {
                                new Label {
                                    Text = "NEW",
                                    FontSize = 3f
                                }
                            }
                        }
                    }
                }
            }
        }.Use();
    }
}
