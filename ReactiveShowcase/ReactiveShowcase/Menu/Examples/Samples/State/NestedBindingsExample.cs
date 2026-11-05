using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class NestedBindingsExample : Example {
    public override string Title => "Nested bindings";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override string Summary => "Binding properties of objects inside a component, like its layout and transform.";

    public override string Explanation =>
        "Bindings also work inside nested initializers. FlexItem, FlexController and ContentTransform are objects of " +
        "their own, and their properties get s-versions just the same: sSize on the item, sGap on the controller, " +
        "slocalEulerAngles on the transform. One slider drives all three.";

    public override IReactiveComponent CreateDemo() => new NestedBindingsDemo();
}

internal class NestedBindingsDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var amount = Remember(0.4f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 1f,
                    ValueStep = 0.05f,
                    Value = amount.Value,
                    OnValueChanged = x => amount.Value = x
                },

                new Layout {
                    FlexController = {
                        sGap = amount.Map(x => new YogaVector((x * 8f).pt))
                    },

                    Children = {
                        CreateBlock(),
                        CreateBlock(),
                        CreateBlock()
                    }
                },

                new Background {
                    FlexItem = {
                        sSize = amount.Map(x => new YogaVector((6f + x * 14f).pt))
                    },

                    ContentTransform = {
                        slocalEulerAngles = amount.Map(x => new Vector3(0f, 0f, x * 90f))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(1f, 0.2f, 0.8f, 0.7f)
                }
            }
        }.Use();
    }

    private static Background CreateBlock() {
        return new Background {
            FlexItem = {
                Size = 6.pt
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = new Color(0f, 0.75f, 1f, 0.7f)
        };
    }
}
