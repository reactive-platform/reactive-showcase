using HMUI;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using UnityEngine.EventSystems;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class DragPadExample : Example {
    public override string Title => "Pointer position";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override bool Centered => true;
    public override string Summary => "Raw pointer input turned into a position that springs back on release.";

    public override string Explanation =>
        "A press only grabs the knob if it lands on it, and the spot you grabbed stays under the pointer. The press's " +
        "event data is kept, since the input module updates it every frame wherever the pointer goes, so OnUpdate " +
        "follows it even off the pad. The knob's position is an animated state: dragging sets it immediately, " +
        "releasing springs it back to the center.";

    public override IReactiveComponent CreateDemo() => new DragPadDemo();
}

internal class DragPadDemo : ReactiveComponent {
    private const float PadSize = 30f;
    private const float KnobSize = 5f;

    private static readonly Vector2 center = new(0.5f, 0.5f);

    private AnimatedState<Vector2> _point = null!;
    private CurvedCanvasSettings? _curvedCanvas;
    private RectTransform _pad = null!;

    // The press being followed. The input module keeps updating it every frame, wherever the pointer goes
    private PointerEventData? _press;

    // Where the knob was grabbed, so that spot stays under the pointer instead of the knob's center
    private Vector2 _grabOffset;

    protected override GameObject Construct() {
        _point = RememberAnimated(center, 450.ms, AnimationCurve.EaseOutBack);

        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        AlignItems = Align.Stretch
                    },

                    FlexItem = {
                        Size = PadSize.pt
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.08f },

                    Do = x => x.WithPointerEvents(
                        onDown: Grab,
                        onUp: _ => Release()
                    ),

                    Children = {
                        new Background {
                            FlexItem = {
                                PositionType = PositionType.Absolute,
                                Size = KnobSize.pt,

                                // Y grows upwards in local space but downwards in the layout
                                sPosition = _point.Map(p => new YogaFrame {
                                    left = ((PadSize - KnobSize) * p.x).pt,
                                    top = ((PadSize - KnobSize) * (1f - p.y)).pt
                                })
                            },

                            Sprite = GameResources.CircleIcon,
                            Color = new Color(0f, 0.75f, 1f)
                        }
                    }
                }.With(x => _pad = x.ContentTransform),

                new Label {
                    FlexItem = {
                        Size = new() { x = 12.pt }
                    },

                    FontSize = 3.5f,
                    Alignment = TMPro.TextAlignmentOptions.CaplineLeft,
                    sText = _point.Map(p => $"x {p.x:0.00}\ny {p.y:0.00}")
                }
            }
        }.Use();
    }

    // Only a press on the knob itself grabs it, the rest of the pad ignores presses
    private void Grab(PointerEventsHandler handler) {
        if (handler.EventData is not { } data || ToKnobSpace(data) is not { } point || !IsOnKnob(point)) {
            return;
        }

        _press = data;

        // Grabbing mid-spring stops the knob where it is
        _point.SetValueImmediate(_point.Value);
        _grabOffset = point - _point.Value;
    }

    // Polled instead of waiting for pointer events, which stop once the pointer is over another graphic
    protected override void OnUpdate() {
        if (_press == null || ToKnobSpace(_press) is not { } point) {
            return;
        }

        var target = point - _grabOffset;
        _point.SetValueImmediate(new Vector2(Mathf.Clamp01(target.x), Mathf.Clamp01(target.y)));
    }

    private void Release() {
        if (_press == null) {
            return;
        }

        _press = null;
        _point.TargetValue = center;
    }

    private bool IsOnKnob(Vector2 point) {
        var distance = (point - _point.Value) * (PadSize - KnobSize);
        return distance.magnitude <= KnobSize / 2f;
    }

    private Vector2? ToKnobSpace(PointerEventData data) {
        // Some events of a press come without a position yet
        if (data.position == Vector2.zero || Canvas is not { } canvas) {
            return null;
        }

        _curvedCanvas ??= Content.GetComponentInParent<CurvedCanvasSettings>();

        var rect = _pad.rect;
        var local = data.TranslateToLocalPoint(_pad, canvas, _curvedCanvas);

        var travel = PadSize - KnobSize;
        var scale = rect.width / PadSize;

        return new Vector2(
            (local.x - rect.xMin - KnobSize / 2f * scale) / (travel * scale),
            (local.y - rect.yMin - KnobSize / 2f * scale) / (travel * scale)
        );
    }
}
