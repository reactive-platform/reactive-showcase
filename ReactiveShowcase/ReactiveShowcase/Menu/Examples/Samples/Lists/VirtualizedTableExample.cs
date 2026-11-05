using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Components.Basic;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class VirtualizedTableExample : Example {
    public override string Title => "Virtualized tables";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override string Summary => "A virtualized Table that only builds the rows you can see.";

    public override string Explanation =>
        "Table measures the first cell and builds just enough cells to fill the view, then reuses them while you " +
        "scroll by handing each one a new CellContext. Cells read the context like any state, so the text and the " +
        "selection follow without rebuilding anything. Watch the cell counter stay put while the row count is huge.";

    public override IReactiveComponent CreateDemo() => new VirtualizedTableDemo();
}

internal class VirtualizedTableDemo : ReactiveComponent {
    private const int RowCount = 10_000;
    private const float RowHeight = 5f;

    private State<int> _cellsBuilt = null!;

    protected override GameObject Construct() {
        _cellsBuilt = Remember(0);

        var selected = Remember(-1);
        var scroll = new ScrollContext();

        var stats = RememberDerived(
            _cellsBuilt, selected,
            (cells, row) => $"{RowCount} rows, {cells} cells built" + (row >= 0 ? $"\nrow {row + 1} selected" : "")
        );

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.5f.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 60.pt }
                    },

                    Children = {
                        new Label {
                            FlexItem = {
                                Flex = 1
                            },

                            FontSize = 3f,
                            Alignment = TextAlignmentOptions.CaplineLeft,
                            EnableWrapping = true,
                            sText = stats
                        },

                        new BsTextButton {
                            Text = "Top",
                            OnClick = () => scroll.ScrollTo(0f)
                        },

                        new BsTextButton {
                            Text = "Bottom",
                            OnClick = () => scroll.ScrollTo(scroll.MaxScrollPos)
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        AlignItems = Align.Stretch,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 60.pt, y = 44.pt }
                    },

                    Children = {
                        new Table<int, IReactiveComponent> {
                            FlexItem = {
                                Flex = 1
                            },

                            ScrollContext = scroll,
                            Items = Enumerable.Range(0, RowCount).ToArray(),
                            SelectionMode = SelectionMode.Single,
                            OnSelectedItemsChanged = x => selected.Value = x.Count > 0 ? x.First() : -1,
                            ConstructCell = CreateCell
                        },

                        new Scrollbar {
                            ScrollContext = scroll
                        }
                    }
                }
            }
        }.Use();
    }

    // Runs once per visible cell, not once per row
    [StateGen]
    private IReactiveComponent CreateCell(CellContext<int> context) {
        _cellsBuilt.Value++;

        return new Background {
            FlexController = {
                AlignItems = Align.Center,
                Padding = new() { left = 2.pt }
            },

            FlexItem = {
                Size = new() { y = RowHeight.pt }
            },

            Sprite = BeatSaberResources.Sprites.rectangle,

            sColor = context.Map(x =>
                x.Selected ? new Color(0f, 0.75f, 1f, 0.35f) :
                x.Item % 2 == 0 ? Color.white with { a = 0.05f } : Color.clear
            ),

            Do = x => x.WithPointerEvents(onDown: _ => context.Selected = true),

            Children = {
                new Label {
                    FontSize = 3.2f,
                    Alignment = TextAlignmentOptions.CaplineLeft,
                    sText = context.Map(x => $"Row {x.Item + 1}")
                }
            }
        };
    }
}
