using System.Collections.Generic;
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

internal class CurvesExample : Example {
    public override string Title => "Curves and durations";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "The same move with different curves, all taking the same time.";

    public override string Explanation =>
        "The duration says how long an animated state takes to reach its target, the curve how progress is spread " +
        "over that time. Every dot gets the same target and duration, only the curve differs, so they all arrive " +
        "together but get there differently. Change the duration and play again.";

    public override IReactiveComponent CreateDemo() => new CurvesDemo();
}

internal class CurvesDemo : ReactiveComponent {
    private const float TrackWidth = 46f;
    private const float DotSize = 4f;

    private readonly List<AnimatedState<float>> _dots = new();
    private State<float> _duration = null!;

    protected override GameObject Construct() {
        _duration = Remember(0.8f);
        var duration = _duration;

        duration.AddCallback(x => _dots.ForEach(dot => dot.Timing = x.sec));

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
                        new BsSlider {
                            MinValue = 0.2f,
                            MaxValue = 2f,
                            ValueStep = 0.1f,
                            Value = duration.Value,
                            ValueFormatter = x => $"{x:0.0}s",
                            OnValueChanged = x => duration.Value = x
                        },

                        new BsPrimaryTextButton {
                            Text = "Play",
                            OnClick = () => _dots.ForEach(dot => dot.TargetValue = dot.TargetValue < 0.5f ? 1f : 0f)
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        Gap = 1.pt
                    },

                    Children = {
                        CreateTrack("Linear", AnimationCurve.Linear),
                        CreateTrack("EaseOutCubic", AnimationCurve.EaseOutCubic),
                        CreateTrack("EaseInOutSine", AnimationCurve.EaseInOutSine),
                        CreateTrack("EaseOutBack", AnimationCurve.EaseOutBack),
                        CreateTrack("Exponential", AnimationCurve.Exponential)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private Layout CreateTrack(string name, AnimationCurve curve) {
        var progress = RememberAnimated(0f, _duration.Value.sec, curve);
        _dots.Add(progress);

        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    FlexItem = {
                        Size = new() { x = 18.pt }
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
                                sPosition = progress.Map(x => new YogaFrame { left = (x * (TrackWidth - DotSize)).pt, top = 0.pt })
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
