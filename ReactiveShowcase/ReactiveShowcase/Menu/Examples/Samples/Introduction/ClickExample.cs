using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class ClickExample : Example {
    public override string Title => "Handling a click";
    public override Topic Topic => Topic.Introduction;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override bool Centered => true;
    public override string Summary => "A button that changes a label.";

    public override string Explanation =>
        "OnClick runs when the button is clicked. Bind keeps a reference to the label while the tree is built, so " +
        "the click can change its text.";

    public override IReactiveComponent CreateDemo() => new ClickDemo();
}

internal class ClickDemo : ReactiveComponent {
    private Label _label = null!;

    protected override GameObject Construct() {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    Text = "Not clicked yet",
                    FontSize = 5f
                }.Bind(ref _label),

                new BsPrimaryTextButton {
                    Text = "Click me",
                    OnClick = () => _label.Text = "Clicked!"
                }
            }
        }.Use();
    }
}
