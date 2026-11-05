using System.Collections.Generic;
using System.Linq;
using HMUI;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class DragDropExample : Example {
    public override string Title => "Drag and drop";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override bool Centered => true;
    public override string Summary => "Moving items between two lists by dragging them.";

    public override string Explanation =>
        "Pressing an item keeps the press's event data, and OnUpdate reads the pointer from it every frame. Pointer " +
        "events alone would stop as soon as the pointer is over another graphic. That one point moves the ghost and " +
        "tells which zone it's over. Each item's zone is a state, and both zones show only the items whose state " +
        "points at them, so dropping is a single assignment.";

    public override IReactiveComponent CreateDemo() => new DragDropDemo();
}

internal class DragDropDemo : ReactiveComponent {
    private const float ItemWidth = 24f;
    private const float ItemHeight = 5f;

    private enum Zone {
        None,
        Backlog,
        Done
    }

    private static readonly string[] items = ["Expert+", "Practice", "Map pack", "Tutorial"];

    private State<Zone>[] _itemZones = null!;
    private readonly Dictionary<Zone, RectTransform> _zoneRects = new();

    private State<int> _dragged = null!;
    private State<Zone> _hovered = null!;
    private State<Vector2> _pointer = null!;
    private CurvedCanvasSettings? _curvedCanvas;

    // The press being followed. The input module keeps updating it every frame, wherever the pointer goes
    private PointerEventData? _press;

    protected override GameObject Construct() {
        _itemZones = [Remember(Zone.Backlog), Remember(Zone.Backlog), Remember(Zone.Backlog), Remember(Zone.Done)];

        _dragged = Remember(-1);
        _hovered = Remember(Zone.None);
        _pointer = Remember(Vector2.zero);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    FontSize = 3f,
                    Color = Color.white with { a = 0.6f },
                    sText = _dragged.Map(x => x < 0 ? "Drag an item to the other list" : $"Dragging {items[x]}")
                },

                new Layout {
                    FlexController = {
                        AlignItems = Align.FlexStart,
                        Gap = 3.pt
                    },

                    Children = {
                        CreateZone(Zone.Backlog, "Backlog"),
                        CreateZone(Zone.Done, "Done")
                    }
                },

                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        PositionType = PositionType.Absolute,
                        Size = new() { x = ItemWidth.pt, y = ItemHeight.pt },
                        sPosition = _pointer.Map(p => new YogaFrame {
                            left = (p.x - ItemWidth / 2f).pt,
                            top = (p.y - ItemHeight / 2f).pt
                        })
                    },

                    sEnabled = _dragged.Map(x => x >= 0),
                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(1f, 0.2f, 0.8f, 0.9f),

                    // Always under the pointer, so it must not catch it
                    RaycastTarget = false,

                    Children = {
                        new Label {
                            FontSize = 3f,
                            RaycastTarget = false,
                            sText = _dragged.Map(x => x < 0 ? string.Empty : items[x])
                        }
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private Background CreateZone(Zone zone, string title) {
        var color = RememberDerived(
            _dragged, _hovered,
            (dragged, hovered) => dragged >= 0 && hovered == zone ? new Color(0f, 0.75f, 1f, 0.25f) : Color.white with { a = 0.06f }
        );

        return new Background {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                Padding = 1.5f.pt,
                Gap = 1.pt
            },

            FlexItem = {
                Size = new() { x = 28.pt, y = 32.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            sColor = color,

            // Every zone holds every item, each shown only where its state says it is
            Children = {
                new Label {
                    Text = title,
                    FontSize = 3.2f,
                    FontStyle = FontStyles.Bold,
                    Color = Color.white with { a = 0.7f }
                },

                CreateItem(0, zone),
                CreateItem(1, zone),
                CreateItem(2, zone),
                CreateItem(3, zone)
            }
        }.With(x => _zoneRects[zone] = x.ContentTransform);
    }

    [StateGen]
    private Background CreateItem(int index, Zone zone) {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { y = ItemHeight.pt }
            },

            sEnabled = _itemZones[index].Map(x => x == zone),
            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,

            sColor = _dragged.Map(x => x == index ? new Color(1f, 0.2f, 0.8f, 0.2f) : new Color(1f, 0.2f, 0.8f, 0.7f)),

            Do = x => x.WithPointerEvents(
                onDown: h => {
                    _dragged.Value = index;
                    _press = h.EventData;
                },
                onUp: _ => Drop()
            ),

            Children = {
                new Label {
                    Text = items[index],
                    FontSize = 3f
                }
            }
        };
    }

    // Polled instead of waiting for pointer events, which stop once the pointer is over another graphic.
    protected override void OnUpdate() {
        // Some events of a press come without a position yet
        if (_press is not { } data || data.position == Vector2.zero || Canvas is not { } canvas) {
            return;
        }

        _curvedCanvas ??= Content.GetComponentInParent<CurvedCanvasSettings>();

        var rect = ContentTransform.rect;
        var local = data.TranslateToLocalPoint(ContentTransform, canvas, _curvedCanvas);

        // The layout measures from the top left, local space from the pivot with Y going up
        _pointer.Value = new Vector2(local.x - rect.xMin, rect.yMax - local.y);
        _hovered.Value = FindZone(data, canvas);
    }

    private Zone FindZone(PointerEventData data, Canvas canvas) {
        return _zoneRects
            .FirstOrDefault(x => x.Value.rect.Contains(data.TranslateToLocalPoint(x.Value, canvas, _curvedCanvas)))
            .Key;
    }

    // Moving an item is pointing its zone state somewhere else
    private void Drop() {
        var index = _dragged.Value;
        var zone = _hovered.Value;

        _press = null;
        _dragged.Value = -1;
        _hovered.Value = Zone.None;

        if (index >= 0 && zone != Zone.None) {
            _itemZones[index].Value = zone;
        }
    }
}
