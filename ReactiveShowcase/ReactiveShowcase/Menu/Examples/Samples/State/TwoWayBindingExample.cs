using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class TwoWayBindingExample : Example {
    public override string Title => "Two-way binding";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "One state driving a toggle, a label and a color at once.";

    public override string Explanation =>
        "The toggle reads the state through sToggled and writes it back in OnToggleStateChanged. Because everything " +
        "reads the same state, flipping it from code moves the toggle too, and there is never a second copy to sync.";

    public override IReactiveComponent CreateDemo() => new TwoWayBindingDemo();
}

internal class TwoWayBindingDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var enabled = Remember(true);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    FontSize = 8f,
                    sText = enabled.Map(x => x ? "ON" : "OFF"),
                    sColor = enabled.Map(x => x ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.4f, 0.4f))
                },

                // Reads the state and writes it back
                new BsToggle {
                    sToggled = enabled,
                    OnToggleStateChanged = x => enabled.Value = x
                },

                new BsTextButton {
                    Text = "Flip from code",
                    OnClick = () => enabled.Value = !enabled.Value
                }
            }
        }.Use();
    }
}
