using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class WhereExample : Example {
    public override string Title => "Where";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "A state that only passes some values on, and keeps the last one that did.";

    public override string Explanation =>
        "Where returns a state that copies its source only when the value passes the predicate, and keeps the last " +
        "match otherwise. Move the slider: every value reaches the first line, only even ones the second, only 7 and " +
        "up the third. Like Map, it returns a state, so the two chain.";

    public override IReactiveComponent CreateDemo() => new WhereDemo();
}

internal class WhereDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var number = Remember(4);

        var even = number.Where(x => x % 2 == 0);
        var large = number.Where(x => x >= 7);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 10f,
                    ValueStep = 1f,
                    Value = number.Value,
                    OnValueChanged = x => number.Value = Mathf.RoundToInt(x)
                },

                CreateLine("every value", number.Map(x => x.ToString())),
                CreateLine("even only", even.Map(x => x.ToString())),
                CreateLine("7 and up", large.Map(x => x >= 7 ? x.ToString() : "none yet"))
            }
        }.Use();
    }

    [StateGen]
    private static Layout CreateLine(string caption, IState<string> text) {
        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    FlexItem = {
                        Size = new() { x = 20.pt }
                    },

                    Text = caption,
                    FontSize = 3.2f,
                    Alignment = TextAlignmentOptions.CaplineRight,
                    Color = Color.white with { a = 0.6f }
                },

                new Label {
                    FlexItem = {
                        Size = new() { x = 14.pt }
                    },

                    FontSize = 5f,
                    Alignment = TextAlignmentOptions.CaplineLeft,
                    sText = text.In()
                }
            }
        };
    }
}
