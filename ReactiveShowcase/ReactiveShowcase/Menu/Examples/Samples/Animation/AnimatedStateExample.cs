using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class AnimatedStateExample : Example {
    public override string Title => "RememberAnimated";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "States that move towards their target instead of jumping.";

    public override string Explanation =>
        "RememberAnimated returns a state with a duration and a curve. Setting TargetValue starts the animation and " +
        "every frame in between notifies the bindings, so the color, the scale and the readout all glide. Hover the card.";

    public override IReactiveComponent CreateDemo() => new AnimatedStateDemo();
}

internal class AnimatedStateDemo : ReactiveComponent {
    private static readonly Color idle = new(1f, 1f, 1f, 0.12f);
    private static readonly Color hovered = new(0f, 0.75f, 1f, 0.9f);

    protected override GameObject Construct() {
        var color = RememberAnimated(idle, 250.ms, AnimationCurve.EaseOutCubic);
        var scale = RememberAnimated(1f, 300.ms, AnimationCurve.EaseOutBack);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 4.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 32.pt, y = 18.pt }
                    },

                    // The layout ignores scale, so growing doesn't push the label below
                    ContentTransform = {
                        slocalScale = scale.Map(x => Vector3.one * x)
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = color,

                    Do = x => x.WithPointerEvents(
                        onEnter: _ => {
                            color.TargetValue = hovered;
                            scale.TargetValue = 1.15f;
                        },
                        onLeave: _ => {
                            color.TargetValue = idle;
                            scale.TargetValue = 1f;
                        }
                    ),

                    Children = {
                        new Label {
                            Text = "Hover me",
                            FontSize = 4.5f
                        }
                    }
                },

                new Label {
                    FontSize = 3.2f,
                    Color = Color.white with { a = 0.6f },
                    sText = scale.Map(x => $"scale {x:0.00}")
                }
            }
        }.Use();
    }
}
