using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class ScrollAdjustExample : Example {
    public override string Title => "Scroll events";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "Turning scroll events into a value, one step per notch.";

    public override string Explanation =>
        "onScroll gets the scroll amount in its event data. Only the direction is used here, so every wheel moves the " +
        "value by one step whatever size it reports. The box doesn't know about scrolling at all, it only reads the " +
        "value. In VR the joystick only scrolls inside scroll areas, which add an adapter for it, so this one listens " +
        "to the wheel.";

    public override IReactiveComponent CreateDemo() => new ScrollAdjustDemo();
}

internal class ScrollAdjustDemo : ReactiveComponent {
    private const int Min = 1;
    private const int Max = 10;

    protected override GameObject Construct() {
        var value = Remember(5);

        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 4.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 26.pt, y = 20.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.1f },

                    Do = x => x.WithPointerEvents(onScroll: h => {
                        var delta = h.EventData?.scrollDelta.y ?? 0f;

                        if (delta != 0f) {
                            value.Value = Mathf.Clamp(value.Value + (delta > 0f ? 1 : -1), Min, Max);
                        }
                    }),

                    Children = {
                        new Label {
                            FontSize = 8f,
                            sText = value.Map(x => x.ToString())
                        },

                        new Label {
                            Text = "scroll here",
                            FontSize = 2.8f,
                            Color = Color.white with { a = 0.6f }
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = (Max * 2.4f).pt
                    },

                    Children = {
                        new Background {
                            FlexItem = {
                                sSize = value.Map(x => new YogaVector((x * 2.4f).pt))
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            sColor = value.Map(x => Color.HSVToRGB(x / (float)Max * 0.8f, 0.7f, 1f))
                        }
                    }
                }
            }
        }.Use();
    }
}
