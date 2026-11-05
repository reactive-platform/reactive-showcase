using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Components.Basic;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class RepeaterExample : Example {
    public override string Title => "Cell reuse";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override string Summary => "A Repeater keeps its cells and hands them new items instead of rebuilding.";

    public override string Explanation =>
        "A Repeater builds a cell only when it has more items than cells. Shuffling, removing and adding again " +
        "reuse the cells it already has: each one gets a new CellContext and reads its item from it, so nothing is " +
        "rebuilt. That's why a cell must read everything from the context. Watch the counter stop growing.";

    public override IReactiveComponent CreateDemo() => new RepeaterDemo();
}

internal class RepeaterDemo : ReactiveComponent {
    private static readonly string[] pool = ["Apple", "Banana", "Cherry", "Mango", "Peach", "Kiwi", "Plum", "Lime"];

    protected override GameObject Construct() {
        var items = Remember<IReadOnlyList<string>>(pool.Take(4).ToArray());
        var built = Remember(0);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        new BsTextButton {
                            Text = "Shuffle",
                            OnClick = () => items.Value = items.Value.OrderBy(_ => Random.value).ToArray()
                        },

                        new BsTextButton {
                            Text = "Add",
                            sInteractable = items.Map(x => x.Count < pool.Length),
                            OnClick = () => items.Value = items.Value.Append(pool.First(x => !items.Value.Contains(x))).ToArray()
                        },

                        new BsTextButton {
                            Text = "Remove",
                            sInteractable = items.Map(x => x.Count > 0),
                            OnClick = () => items.Value = items.Value.Take(items.Value.Count - 1).ToArray()
                        }
                    }
                },

                new Label {
                    FontSize = 3.5f,
                    sText = RememberDerived(items, built, (i, b) => $"{i.Count} items, {b} cells built")
                },

                new Repeater<string, Background> {
                    FlexController = {
                        FlexDirection = FlexDirection.Row,
                        FlexWrap = Wrap.Wrap,
                        JustifyContent = Justify.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 62.pt }
                    },

                    sItems = items,
                    SelectionMode = SelectionMode.None,

                    ConstructCell = context => CreateCell(context, built)
                }
            }
        }.Use();
    }

    // Only runs when the repeater has more items than cells
    [StateGen]
    private static Background CreateCell(CellContext<string> context, State<int> built) {
        built.Value++;

        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { x = 14.pt, y = 6.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = Color.white with { a = 0.1f },

            Children = {
                new Label {
                    FontSize = 3.2f,
                    sText = context.Map(x => x.Item)
                }
            }
        };
    }
}
