using System;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class AnimationPlayerExample : Example {
    public override string Title => "AnimationPlayer";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Playing animations on a component, stopping, finishing and chaining them.";

    public override string Explanation =>
        "An animated state animates a value, an AnimationPlayer animates a component: it plays an Animation, a track " +
        "with a duration and a curve, on its target. Play starts one, Stop freezes it where it is, Finish jumps to the " +
        "end. The finish callback starts the next one, which is how the round trip chains a fade out and a fade in.";

    public override IReactiveComponent CreateDemo() => new AnimationPlayerDemo();
}

internal class AnimationPlayerDemo : ReactiveComponent {
    private static readonly Animation<IReactiveComponent> grow = new(AnimationTracks.Scale(0.3f, 1f), 1.5f, AnimationCurve.EaseOutCubic);
    private static readonly Animation<IReactiveComponent> fadeOut = new(AnimationTracks.Fade(1f, 0f), 0.4f);
    private static readonly Animation<IReactiveComponent> fadeIn = new(AnimationTracks.Fade(0f, 1f), 0.4f);

    private AnimationPlayer<IReactiveComponent> _player = null!;
    private Background _box = null!;

    protected override GameObject Construct() {
        var go = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 4.pt
            },

            Children = {
                new Background {
                    FlexItem = {
                        Size = 14.pt
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(0f, 0.75f, 1f)
                }.Bind(ref _box),

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        CreateButton("Grow", () => _player.Play(grow)),
                        CreateButton("Stop", () => _player.Stop()),
                        CreateButton("Finish", () => _player.Finish()),

                        CreateButton("Round trip", () => _player.Play(fadeOut, () => _player.Play(fadeIn)))
                    }
                }
            }
        }.Use();

        // The player needs its target, and stops together with this component
        _player = RememberAnimationPlayer<IReactiveComponent>(_box);

        return go;
    }

    private static BsTextButton CreateButton(string text, Action onClick) {
        return new BsTextButton {
            Text = text,
            OnClick = onClick
        };
    }
}
