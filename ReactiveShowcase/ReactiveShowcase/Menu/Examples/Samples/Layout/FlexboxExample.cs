using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class FlexboxExample : Example {
    public override string Title => "Justify and align";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "Where children sit along the direction and across it.";

    public override string Explanation =>
        "JustifyContent places the children along the direction: packed at the start, the center or the end, or " +
        "spread with the free space between them. AlignItems places each child across it, and Stretch makes them fill " +
        "the cross axis, which is why the blocks only have a minimum size.";

    public override IReactiveComponent CreateDemo() => new FlexboxDemo();
}

internal class FlexboxDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var justify = Remember(Justify.Center);
        var align = Remember(Align.Center);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                CreatePicker(justify, new() {
                    [Justify.FlexStart] = new("Start", null),
                    [Justify.Center] = new("Center", null),
                    [Justify.FlexEnd] = new("End", null),
                    [Justify.SpaceBetween] = new("Between", null)
                }),

                CreatePicker(align, new() {
                    [Align.FlexStart] = new("Start", null),
                    [Align.Center] = new("Center", null),
                    [Align.FlexEnd] = new("End", null),
                    [Align.Stretch] = new("Stretch", null)
                }),

                new Background {
                    FlexItem = {
                        Size = new() { x = 64.pt, y = 24.pt },
                        Margin = new() { top = 1.pt }
                    },

                    FlexController = {
                        sJustifyContent = justify,
                        sAlignItems = align,
                        Padding = 1.5f.pt,
                        Gap = 1.pt
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.08f },

                    Children = {
                        CreateBlock(new Color(0f, 0.75f, 1f), 8f),
                        CreateBlock(new Color(1f, 0.2f, 0.8f), 5f),
                        CreateBlock(new Color(1f, 0.8f, 0.2f), 10f)
                    }
                }
            }
        }.Use();
    }

    private static BsSegmentedControl<T> CreatePicker<T>(State<T> state, Dictionary<T, BsSegmentedControlItem> items) {
        return new BsSegmentedControl<T> {
            FlexItem = {
                Size = new() { x = 64.pt, y = 5.pt }
            },

            Items = items,
            Key = state.Value,
            OnKeyChanged = x => state.Value = x
        };
    }

    // A min size instead of a size, so Stretch can still grow the block across the box
    private static Background CreateBlock(Color color, float size) {
        return new Background {
            FlexItem = {
                MinSize = size.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color
        };
    }
}
