using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class ComponentBindingsExample : Example {
    public override string Title => "Bindings on your components";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Your own component's properties get s-versions just like built-in ones.";

    public override string Explanation =>
        "Meter is an ordinary component with a Value property. Because it has a setter, the generator gives it " +
        "sValue automatically, nothing to declare. Two meters are bound to the same state, a third is set once " +
        "and never follows. Inside, Value only sets a private state that the meter's own bindings read.";

    public override IReactiveComponent CreateDemo() => new ComponentBindingsDemo();
}

internal class ComponentBindingsDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var level = Remember(0.6f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 1f,
                    ValueStep = 0.05f,
                    Value = level.Value,
                    OnValueChanged = x => level.Value = x
                },

                // Generated from Meter.Value, like sText is from Label.Text
                new Meter {
                    sValue = level
                },

                new Meter {
                    sValue = level.Map(x => 1f - x)
                },

                // Plain assignment: set once, never follows
                new Meter {
                    Value = 0.3f
                }
            }
        }.Use();
    }
}

/// <summary>
/// A horizontal bar filled up to its value, from 0 to 1.
/// </summary>
internal class Meter : ReactiveComponent {
    private const float Width = 40f;

    public float Value {
        get => _value.Value;
        set => _value.Value = Mathf.Clamp01(value);
    }

    private State<float> _value = null!;

    protected override GameObject Construct() {
        _value = Remember(0f);

        return new Background {
            // Without a controller the fill wouldn't be laid out. An empty initializer wouldn't create one,
            // the getter only runs when something is assigned
            FlexController = {
                AlignItems = Align.Stretch
            },

            FlexItem = {
                Size = new() { x = Width.pt, y = 4.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = Color.white with { a = 0.08f },

            Children = {
                new Background {
                    FlexItem = {
                        sSize = _value.Map(x => new YogaVector((x * Width).pt, 100.pct))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(0f, 0.75f, 1f, 0.8f)
                }
            }
        }.Use();
    }
}
