using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class DirectionGapExample : Example {
    public override string Title => "Direction and gap";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override string Summary => "Children go in a row or a column, with a gap between them.";

    public override string Explanation =>
        "A layout places its children one after another along a direction: a row or a column, either way round. " +
        "Gap is the space between neighbours, not around the edges. Both are properties of FlexController, the " +
        "settings of the container, and both are bound to states here.";

    public override IReactiveComponent CreateDemo() => new DirectionGapDemo();
}

internal class DirectionGapDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var direction = Remember(FlexDirection.Row);
        var gap = Remember(2f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSegmentedControl<FlexDirection> {
                    FlexItem = {
                        Size = new() { x = 64.pt, y = 5.pt }
                    },

                    Items = new Dictionary<FlexDirection, BsSegmentedControlItem> {
                        [FlexDirection.Row] = new("Row", null),
                        [FlexDirection.RowReverse] = new("Row reverse", null),
                        [FlexDirection.Column] = new("Column", null),
                        [FlexDirection.ColumnReverse] = new("Column reverse", null)
                    },

                    Key = direction.Value,
                    OnKeyChanged = x => direction.Value = x
                },

                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 6f,
                    ValueStep = 0.5f,
                    Value = gap.Value,
                    ValueFormatter = x => $"gap {x:0.0}pt",
                    OnValueChanged = x => gap.Value = x
                },

                new Layout {
                    FlexController = {
                        sFlexDirection = direction,
                        sGap = gap.Map(x => new YogaVector(x.pt))
                    },

                    Children = {
                        CreateBlock("1", new Color(0f, 0.75f, 1f)),
                        CreateBlock("2", new Color(1f, 0.2f, 0.8f)),
                        CreateBlock("3", new Color(1f, 0.8f, 0.2f))
                    }
                }
            }
        }.Use();
    }

    private static Background CreateBlock(string text, Color color) {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = 7.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color with { a = 0.7f },

            Children = {
                new Label {
                    Text = text,
                    FontSize = 3.5f
                }
            }
        };
    }
}
