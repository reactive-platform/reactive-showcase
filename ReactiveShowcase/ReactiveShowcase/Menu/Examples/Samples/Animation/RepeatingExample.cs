using System;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class RepeatingExample : Example {
    public override string Title => "Repeating animations";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Playing an animation several times, a fraction of a time or forever.";

    public override string Explanation =>
        "Iterations says how many times an animation plays: a number like 3, a fraction like 2.5 to stop partway through the last " +
        "run, or Infinite to repeat until stopped. The player's Iteration tells which run is playing. Finish jumps " +
        "to the end, and an endless animation finishes the run it's in instead.";

    public override IReactiveComponent CreateDemo() => new RepeatingDemo();
}

internal class RepeatingDemo : ReactiveComponent {
    private static readonly Animation<IReactiveComponent> beat = new(
        AnimationTracks.Scale(1f, 1.4f),
        500.ms,
        AnimationCurve.EaseOutCubic
    );

    private AnimationPlayer<IReactiveComponent> _player = null!;
    private Background _box = null!;

    private State<int> _iteration = null!;
    private State<bool> _playing = null!;

    protected override GameObject Construct() {
        _iteration = Remember(0);
        _playing = Remember(false);

        var go = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Background {
                    FlexItem = {
                        Size = 12.pt,
                        Margin = new() { top = 3.pt, bottom = 3.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(1f, 0.3f, 0.5f)
                }.Bind(ref _box),

                new Label {
                    FlexItem = {
                        Size = new() { x = 30.pt }
                    },

                    FontSize = 3.2f,
                    Color = Color.white with { a = 0.6f },
                    sText = RememberDerived(_playing, _iteration, (playing, iteration) => playing ? $"iteration {iteration}" : "not playing")
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        CreateButton("Once", () => Play(1)),
                        CreateButton("3 times", () => Play(3)),
                        CreateButton("2.5 times", () => Play(2.5f)),
                        CreateButton("Forever", () => Play(AnimationIteration.Infinite))
                    }
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        CreateButton("Stop", () => _player.Stop()),
                        CreateButton("Finish", () => _player.Finish())
                    }
                }
            }
        }.Use();

        _player = RememberAnimationPlayer<IReactiveComponent>(_box);

        return go;
    }

    private void Play(AnimationIteration iterations) {
        _player.Play(beat with { Iterations = iterations });
    }

    protected override void OnUpdate() {
        _playing.Value = _player.IsPlaying;
        _iteration.Value = _player.Iteration;
    }

    private static BsTextButton CreateButton(string text, Action onClick) {
        return new BsTextButton {
            Text = text,
            OnClick = onClick
        };
    }
}
