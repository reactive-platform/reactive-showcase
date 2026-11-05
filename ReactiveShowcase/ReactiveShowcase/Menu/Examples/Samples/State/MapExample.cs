using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class MapExample : Example {
    public override string Title => "Map";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "One state reshaped into others: text, a size and a color.";

    public override string Explanation =>
        "Map returns a new state that is always a function of its source. Each one updates whenever the number " +
        "changes, so the word, the bar and the swatch never disagree. A mapped state is a state like any other, " +
        "so it can be bound, mapped again or passed around.";

    public override IReactiveComponent CreateDemo() => new MapDemo();
}

internal class MapDemo : ReactiveComponent {
    private static readonly string[] words = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];

    protected override GameObject Construct() {
        var number = Remember(4);

        var word = number.Map(x => words[x]);
        var width = number.Map(x => new YogaVector((x * 4f + 2f).pt, 4.pt));
        var color = number.Map(x => Color.HSVToRGB(x / 12f, 0.75f, 1f));

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.5f.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 0f,
                    MaxValue = 10f,
                    ValueStep = 1f,
                    Value = number.Value,
                    OnValueChanged = x => number.Value = Mathf.RoundToInt(x)
                },

                new Label {
                    FontSize = 6f,
                    sText = word
                },

                new Background {
                    FlexItem = {
                        sSize = width
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = color
                }
            }
        }.Use();
    }
}
