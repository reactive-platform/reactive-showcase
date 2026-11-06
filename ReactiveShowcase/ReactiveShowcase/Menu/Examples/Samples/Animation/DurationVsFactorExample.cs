using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class DurationVsFactorExample : Example {
    public override string Title => "Duration and factor";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "Reaching a target in a fixed time, or by closing the gap at a steady rate.";

    public override string Explanation =>
        "An animated state takes a timing: a duration like 400.ms, or a factor like 8.fact. A duration gets there in " +
        "exactly that time along its curve, but every new target restarts it. A factor closes a share of the remaining " +
        "gap every second, fast when far and slow when close. Jump between the ends, then drag the slider and compare.";

    public override IReactiveComponent CreateDemo() => new DurationVsFactorDemo();
}

internal class DurationVsFactorDemo : ReactiveComponent {
    private const float TrackWidth = 46f;
    private const float DotSize = 4f;

    protected override GameObject Construct() {
        var target = Remember(0f);

        var timed = RememberAnimated(0f, 400.ms, AnimationCurve.EaseInOutCubic);
        var factored = RememberAnimated(0f, 8.fact);

        target.AddCallback(x => {
            timed.TargetValue = x;
            factored.TargetValue = x;
        });

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 2.pt
                    },

                    Children = {
                        new BsTextButton {
                            Text = "Start",
                            OnClick = () => target.Value = 0f
                        },

                        new BsSlider {
                            MinValue = 0f,
                            MaxValue = 1f,
                            ValueStep = 0.01f,
                            ValueFormatter = x => $"{x:0%}",
                            sValue = target,
                            OnValueChanged = x => target.Value = x
                        },

                        new BsTextButton {
                            Text = "End",
                            OnClick = () => target.Value = 1f
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        Gap = 1.pt
                    },

                    Children = {
                        CreateTrack("400.ms", timed),
                        CreateTrack("8.fact", factored)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private static Layout CreateTrack(string name, IState<float> progress) {
        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    FlexItem = {
                        Size = new() { x = 12.pt }
                    },

                    Text = name,
                    FontSize = 2.6f,
                    Alignment = TextAlignmentOptions.CaplineRight,
                    Color = Color.white with { a = 0.6f }
                },

                new Background {
                    FlexController = {
                        AlignItems = Align.Stretch
                    },

                    FlexItem = {
                        Size = new() { x = TrackWidth.pt, y = DotSize.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.06f },

                    Children = {
                        new Background {
                            FlexItem = {
                                PositionType = PositionType.Absolute,
                                Size = DotSize.pt,
                                sPosition = progress.Map(x => new YogaFrame { left = (x * (TrackWidth - DotSize)).pt, top = 0.pt }).In()
                            },

                            Sprite = GameResources.CircleIcon,
                            Color = new Color(0f, 0.75f, 1f)
                        }
                    }
                }
            }
        };
    }
}
