using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Components.Basic;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class RepeaterBasicsExample : Example {
    public override string Title => "Repeater";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override string Summary => "A list state shown as cells, one per item.";

    public override string Explanation =>
        "A Repeater takes a list of items and a function building a cell for one item. Bind its Items to a list state " +
        "and the cells follow the list: adding or removing an item is just assigning a new list. Each cell reads its " +
        "item from the CellContext it's given, a state like any other.";

    public override IReactiveComponent CreateDemo() => new RepeaterBasicsDemo();
}

internal class RepeaterBasicsDemo : ReactiveComponent {
    private const int MaxNames = 6;

    protected override GameObject Construct() {
        var names = Remember<IReadOnlyList<string>>(["Ada", "Linus", "Grace"]);
        var next = 0;

        string[] pool = ["Alan", "Barbara", "Dennis", "Margaret", "Ken", "Frances"];

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
                        new BsPrimaryTextButton {
                            Text = "Add",
                            sInteractable = names.Map(x => x.Count < MaxNames),
                            OnClick = () => names.Value = names.Value.Append(pool[next++ % pool.Length]).ToArray()
                        },

                        new BsTextButton {
                            Text = "Remove",
                            sInteractable = names.Map(x => x.Count > 0),
                            OnClick = () => names.Value = names.Value.Take(names.Value.Count - 1).ToArray()
                        }
                    }
                },

                new Repeater<string, Background> {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Center,
                        Gap = 0.5f.pt
                    },

                    SelectionMode = SelectionMode.None,
                    sItems = names,

                    ConstructCell = context => new Background {
                        FlexController = {
                            JustifyContent = Justify.Center,
                            AlignItems = Align.Center
                        },

                        FlexItem = {
                            Size = new() { x = 30.pt, y = 4.5f.pt }
                        },

                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 10f,
                        Color = Color.white with { a = 0.1f },

                        Children = {
                            new Label {
                                FontSize = 3f,
                                sText = context.Map(x => $"{x.Index + 1}. {x.Item}")
                            }
                        }
                    }
                }
            }
        }.Use();
    }
}
