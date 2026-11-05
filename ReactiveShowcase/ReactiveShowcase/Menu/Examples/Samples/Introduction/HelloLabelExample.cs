using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class HelloLabelExample : Example {
    public override string Title => "Construct and Use";
    public override Topic Topic => Topic.Introduction;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override bool Centered => true;
    public override string Summary => "The smallest component: a tree described once in Construct.";

    public override string Explanation =>
        "A component describes its view once, in Construct. Object initializers build the tree and Use() turns the root " +
        "into the component's own GameObject. Nothing here changes later, so there is no state yet.";

    public override IReactiveComponent CreateDemo() => new HelloLabelDemo();
}

internal class HelloLabelDemo : ReactiveComponent {
    protected override GameObject Construct() {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    Text = "Hello, Reactive!",
                    FontSize = 7f,
                    FontStyle = FontStyles.Bold
                },

                new Label {
                    Text = "Built once, in Construct",
                    FontSize = 3.5f,
                    Color = Color.white with { a = 0.6f }
                }
            }
        }.Use();
    }
}
