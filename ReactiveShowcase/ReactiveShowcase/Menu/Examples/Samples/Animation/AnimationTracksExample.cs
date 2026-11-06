using System;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class AnimationTracksExample : Example {
    public override string Title => "Animation tracks";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Combining built-in tracks and writing your own.";

    public override string Explanation =>
        "A track is what an animation does with its progress. The built-in ones fade and scale, + combines tracks " +
        "into one that runs them together, and AnimationTracks.Create turns any function of the target and progress " +
        "into a track, like the spin and the decaying wobble here.";

    public override IReactiveComponent CreateDemo() => new AnimationTracksDemo();
}

internal class AnimationTracksDemo : ReactiveComponent {
    private static readonly Animation<IReactiveComponent> pop = new(
        AnimationTracks.Scale(0.4f, 1f) + AnimationTracks.Fade(0.3f, 1f),
        450.ms,
        AnimationCurve.EaseOutBack
    );

    private static readonly Animation<IReactiveComponent> fadeOut = new(AnimationTracks.Fade(1f, 0f), 300.ms, AnimationCurve.EaseInCubic);
    private static readonly Animation<IReactiveComponent> fadeIn = new(AnimationTracks.Fade(0f, 1f), 300.ms, AnimationCurve.EaseOutCubic);

    private static readonly Animation<IReactiveComponent> spin = new(
        AnimationTracks.Create<IReactiveComponent>((x, t) => x.ContentTransform.localEulerAngles = new(0f, 0f, t * 360f)),
        700.ms,
        AnimationCurve.EaseInOutCubic
    );

    private static readonly Animation<IReactiveComponent> wobble = new(
        AnimationTracks.Create<IReactiveComponent>((x, t) => {
            var angle = Mathf.Sin(t * Mathf.PI * 6f) * (1f - t) * 20f;
            x.ContentTransform.localEulerAngles = new(0f, 0f, angle);
        }),
        600.ms
    );

    private AnimationPlayer<IReactiveComponent> _player = null!;
    private Background _box = null!;

    protected override GameObject Construct() {
        var go = new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 5.pt
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
                        CreateButton("Pop", () => _player.Play(pop)),
                        CreateButton("Fade", () => _player.Play(fadeOut, () => _player.Play(fadeIn))),
                        CreateButton("Spin", () => _player.Play(spin)),
                        CreateButton("Wobble", () => _player.Play(wobble))
                    }
                }
            }
        }.Use();

        // The target has to exist before the player is created
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
