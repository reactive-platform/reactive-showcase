using System;
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

internal class DirectionExample : Example {
    public override string Title => "Playback direction";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Playing every run forwards, backwards or flipping between them.";

    public override string Explanation =>
        "Direction decides which way each run plays. Reverse plays them all backwards, Alternate flips every run so " +
        "a repeat continues from where the last one ended instead of jumping back. Every dot plays the same three runs, " +
        "only the direction differs. The target here is a plain state, a player can animate anything.";

    public override IReactiveComponent CreateDemo() => new DirectionDemo();
}

internal class DirectionDemo : ReactiveComponent {
    private const float TrackWidth = 40f;
    private const float DotSize = 4f;

    private static readonly IAnimationTrack<State<float>> move = AnimationTracks.Create<State<float>>((x, t) => x.Value = t);

    private readonly List<(AnimationPlayer<State<float>> Player, AnimationDirection Direction)> _rows = new();

    protected override GameObject Construct() {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        CreateButton("Play 3 times", () => Play(3)),
                        CreateButton("Loop", () => Play(AnimationIteration.Infinite)),
                        CreateButton("Stop", () => _rows.ForEach(x => x.Player.Stop()))
                    }
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        Gap = 1.pt
                    },

                    Children = {
                        CreateRow(AnimationDirection.Normal),
                        CreateRow(AnimationDirection.Reverse),
                        CreateRow(AnimationDirection.Alternate),
                        CreateRow(AnimationDirection.AlternateReverse)
                    }
                }
            }
        }.Use();
    }

    private void Play(AnimationIteration iterations) {
        foreach (var (player, direction) in _rows) {
            player.Play(new(move, 700.ms, AnimationCurve.EaseInOutSine, iterations, direction));
        }
    }

    [StateGen]
    private Layout CreateRow(AnimationDirection direction) {
        var position = Remember(0f);
        _rows.Add((RememberAnimationPlayer(position), direction));

        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    FlexItem = {
                        Size = new() { x = 22.pt }
                    },

                    Text = direction.ToString(),
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
                                sPosition = position.Map(x => new YogaFrame { left = (x * (TrackWidth - DotSize)).pt, top = 0.pt })
                            },

                            Sprite = GameResources.CircleIcon,
                            Color = new Color(1f, 0.6f, 0.2f)
                        }
                    }
                }
            }
        };
    }

    private static BsTextButton CreateButton(string text, Action onClick) {
        return new BsTextButton {
            Text = text,
            OnClick = onClick
        };
    }
}
