using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class HoldToConfirmExample : Example {
    public override string Title => "Press duration";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "A button that only fires when held, built from pointer state and the update loop.";

    public override string Explanation =>
        "Pointer events only say when the press starts and ends. Holding is time, so the press is stored in a state " +
        "and OnUpdate fills the progress while it's true and drains it otherwise. Reaching the end confirms once and " +
        "waits for a release, so keeping the pointer down doesn't repeat it.";

    public override IReactiveComponent CreateDemo() => new HoldToConfirmDemo();
}

internal class HoldToConfirmDemo : ReactiveComponent {
    private const float HoldTime = 1.2f;
    private const float Width = 44f;

    private State<bool> _pressed = null!;
    private State<float> _progress = null!;
    private State<int> _confirmed = null!;
    private bool _waitingForRelease;

    protected override GameObject Construct() {
        _pressed = Remember(false);
        _progress = Remember(0f);
        _confirmed = Remember(0);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = Width.pt, y = 9.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.1f },

                    Do = x => x.WithPointerEvents(
                        onDown: _ => _pressed.Value = true,
                        onUp: _ => Release(),
                        onLeave: _ => Release()
                    ),

                    Children = {
                        new Background {
                            FlexItem = {
                                PositionType = PositionType.Absolute,
                                Position = new() { left = 0.pt, top = 0.pt, bottom = 0.pt },
                                sSize = _progress.Map(x => new YogaVector((x * Width).pt, YogaValue.Auto))
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            Color = new Color(1f, 0.3f, 0.4f, 0.7f)
                        },

                        new Label {
                            FontSize = 3.5f,
                            sText = _pressed.Map(x => x ? "Keep holding..." : "Hold to delete")
                        }
                    }
                },

                new Label {
                    FontSize = 3.2f,
                    Color = Color.white with { a = 0.6f },
                    sText = _confirmed.Map(x => x == 0 ? "Nothing deleted yet" : $"Deleted {x} time{(x == 1 ? "" : "s")}")
                }
            }
        }.Use();
    }

    protected override void OnUpdate() {
        var holding = _pressed.Value && !_waitingForRelease;

        // Filling takes the hold time, draining is three times faster
        var step = Time.deltaTime / HoldTime * (holding ? 1f : -3f);
        _progress.Value = Mathf.Clamp01(_progress.Value + step);

        if (holding && _progress.Value >= 1f) {
            _confirmed.Value++;
            _waitingForRelease = true;
        }
    }

    private void Release() {
        _pressed.Value = false;
        _waitingForRelease = false;
    }
}
