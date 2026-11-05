using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class GrowShrinkExample : Example {
    public override string Title => "Grow and shrink";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "How free space is shared out with FlexGrow, FlexShrink and FlexBasis.";

    public override string Explanation =>
        "FlexBasis is where an item starts, FlexGrow says how much of the leftover space it takes, FlexShrink how much " +
        "it gives up when there isn't enough. Resize the row: the fixed block never changes, the growing ones split the " +
        "rest 1:2, and once the row is too narrow only the blocks that may shrink get smaller. Widths are read back " +
        "from the applied layout.";

    public override IReactiveComponent CreateDemo() => new GrowShrinkDemo();
}

internal class GrowShrinkDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var width = Remember(70f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 30f,
                    MaxValue = 90f,
                    ValueStep = 2f,
                    Value = width.Value,
                    ValueFormatter = x => $"row {x:0}pt",
                    OnValueChanged = x => width.Value = x
                },

                new Background {
                    FlexController = {
                        Padding = 1.pt,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        sSize = width.Map(x => new YogaVector(x.pt, 12.pt))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.08f },

                    Children = {
                        CreateBlock("fixed", new Color(1f, 0.8f, 0.2f), grow: 0f, shrink: 0f, basis: 16.pt),
                        CreateBlock("grow 1", new Color(0f, 0.75f, 1f), grow: 1f, shrink: 1f, basis: 10.pt),
                        CreateBlock("grow 2", new Color(1f, 0.2f, 0.8f), grow: 2f, shrink: 1f, basis: 10.pt)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private static Background CreateBlock(string rule, Color color, float grow, float shrink, YogaValue basis) {
        var width = Remember(0f);

        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                FlexGrow = grow,
                FlexShrink = shrink,
                FlexBasis = basis
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color with { a = 0.6f },

            Children = {
                new Label {
                    FontSize = 2.6f,
                    sText = width.Map(x => $"{rule}\n{x:0}pt")
                }
            }
        }.With(x => x.LayoutAppliedEvent += _ => width.Value = x.ContentTransform.rect.width);
    }
}
