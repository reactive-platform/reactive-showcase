using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class DerivedStateExample : Example {
    public override string Title => "RememberDerived";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "A value computed from other states, recomputed when any of them change.";

    public override string Explanation =>
        "RememberDerived takes states as arguments and a function of their values. The states are passed in rather " +
        "than captured, so the result knows what it depends on and updates when either slider moves. The bar's size " +
        "and color both come from the same sum.";

    public override IReactiveComponent CreateDemo() => new DerivedStateDemo();
}

internal class DerivedStateDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var a = Remember(3f);
        var b = Remember(4f);

        var sum = RememberDerived(a, b, (x, y) => x + y);
        var text = RememberDerived(a, b, (x, y) => $"{x:0} + {y:0} = {x + y:0}");

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                CreateSlider(a),
                CreateSlider(b),

                new Label {
                    FontSize = 6f,
                    sText = text
                },

                new Background {
                    FlexItem = {
                        sSize = sum.Map(x => new YogaVector((x * 3f).pt, 3.pt))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = sum.Map(x => Color.Lerp(new Color(0f, 0.75f, 1f), new Color(1f, 0.2f, 0.8f), x / 20f))
                }
            }
        }.Use();
    }

    private static BsSlider CreateSlider(State<float> state) {
        return new BsSlider {
            MinValue = 0f,
            MaxValue = 10f,
            ValueStep = 1f,
            Value = state.Value,
            OnValueChanged = x => state.Value = x
        };
    }
}
