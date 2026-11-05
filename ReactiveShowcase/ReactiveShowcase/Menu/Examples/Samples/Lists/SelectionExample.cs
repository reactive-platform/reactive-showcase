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

internal class SelectionExample : Example {
    public override string Title => "Selection";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override bool Centered => true;
    public override string Summary => "Single and multiple selection, set from cells and read back from the list.";

    public override string Explanation =>
        "Repeaters and tables keep track of what's selected. A cell sets context.Selected to select itself, and " +
        "reads it back to look selected. SelectionMode decides whether that replaces the previous selection or adds " +
        "to it, and OnSelectedItemsChanged reports the whole selection after every change.";

    public override IReactiveComponent CreateDemo() => new SelectionDemo();
}

internal class SelectionDemo : ReactiveComponent {
    private static readonly string[] maps = ["Easy", "Normal", "Hard", "Expert", "Expert+", "Lightshow"];

    protected override GameObject Construct() {
        var selection = Remember("Nothing selected");
        Repeater<string, Background> list = null!;

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSegmentedControl<SelectionMode> {
                    FlexItem = {
                        Size = new() { x = 40.pt, y = 5.pt }
                    },

                    Items = new Dictionary<SelectionMode, BsSegmentedControlItem> {
                        [SelectionMode.Single] = new("Single", null),
                        [SelectionMode.Multiple] = new("Multiple", null)
                    },

                    Key = SelectionMode.Single,
                    OnKeyChanged = x => {
                        // Switching modes starts over, a single mode can't hold several items
                        list.SelectedItems = [];
                        list.SelectionMode = x;
                    }
                },

                new Repeater<string, Background> {
                    FlexController = {
                        FlexDirection = FlexDirection.Row,
                        FlexWrap = Wrap.Wrap,
                        JustifyContent = Justify.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 52.pt }
                    },

                    SelectionMode = SelectionMode.Single,
                    Items = maps,
                    OnSelectedItemsChanged = x => selection.Value = x.Count == 0 ? "Nothing selected" : string.Join(", ", x),
                    ConstructCell = CreateCell
                }.Bind(ref list),

                new Label {
                    FontSize = 3.2f,
                    sText = selection
                }
            }
        }.Use();
    }

    [StateGen]
    private static Background CreateCell(CellContext<string> context) {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { x = 16.pt, y = 5.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            sColor = context.Map(x => x.Selected ? new Color(0f, 0.75f, 1f, 0.6f) : Color.white with { a = 0.1f }),

            Do = x => x.WithPointerEvents(onDown: _ => context.Selected = !context.Selected),

            Children = {
                new Label {
                    FontSize = 3f,
                    sText = context.Map(x => x.Item)
                }
            }
        };
    }
}
