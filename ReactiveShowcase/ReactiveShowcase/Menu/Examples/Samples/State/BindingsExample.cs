using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class BindingsExample : Example {
    public override string Title => "Bindings";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override string Summary => "Any settable property gets an s-version that follows a state.";

    public override string Explanation =>
        "For a property like Text, the generator adds sText: assign a state to it and the property is set now and " +
        "again whenever the state changes. It works for every settable property, so one state can drive the text, " +
        "the color, the size and whether something is shown at all. The slider only changes the level.";

    public override IReactiveComponent CreateDemo() => new BindingsDemo();
}

internal class BindingsDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var level = Remember(3);

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
                    Value = level.Value,
                    OnValueChanged = x => level.Value = Mathf.RoundToInt(x)
                },

                new Label {
                    // Every s-property below reads the same state
                    sText = level.Map(x => $"Level {x}"),
                    sFontSize = level.Map(x => 3f + x * 0.4f),
                    sColor = level.Map(x => Color.Lerp(Color.white, new Color(1f, 0.3f, 0.3f), x / 10f))
                },

                new Label {
                    Text = "Careful, that's loud",
                    FontSize = 3.2f,
                    FontStyle = FontStyles.Bold,
                    Color = new Color(1f, 0.6f, 0.3f),
                    sEnabled = level.Map(x => x >= 8)
                }
            }
        }.Use();
    }
}
