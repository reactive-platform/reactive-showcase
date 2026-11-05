using System;
using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class LayoutStitchingExample : Example {
    public override string Title => "Custom layout controllers";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Expert;
    public override bool Centered => true;
    public override string Summary => "How separate layout trees join, and a custom layout controller.";

    public override string Explanation =>
        "Nested flex layouts are stitched: a child layout claims its parent's Yoga context, so both component " +
        "hierarchies form one native tree that's calculated once. A controller of another kind can't join that tree. " +
        "The ring on the right is laid out by RadialLayoutController, its own tree, which the flex parent " +
        "sizes as a single box and measures through ILayoutMeasurable. Changing the radius raises " +
        "LayoutControllerUpdatedEvent, which recalculates it.";

    public override IReactiveComponent CreateDemo() => new LayoutStitchingDemo();
}

internal class LayoutStitchingDemo : ReactiveComponent {
    private const int ItemCount = 8;
    private const float MinRadius = 6f;
    private const float MaxRadius = 11f;

    protected override GameObject Construct() {
        var radius = Remember(10f);
        var controller = new RadialLayoutController();

        radius.AddCallback(x => controller.Radius = x);

        // Its children are laid out by the radial controller instead of flexbox
        var ring = new Layout {
            LayoutController = controller,

            FlexItem = {
                sSize = radius.Map(x => new YogaVector(RingSize(x).pt))
            },

            Children = {
                CreateNumber(0),
                CreateNumber(1),
                CreateNumber(2),
                CreateNumber(3),
                CreateNumber(4),
                CreateNumber(5),
                CreateNumber(6),
                CreateNumber(7)
            }
        };

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSlider {
                    MinValue = MinRadius,
                    MaxValue = MaxRadius,
                    ValueStep = 1f,
                    Value = radius.Value,
                    ValueFormatter = x => $"radius {x:0}",
                    OnValueChanged = x => radius.Value = x
                },

                new Layout {
                    FlexController = {
                        AlignItems = Align.FlexStart,
                        Gap = 4.pt
                    },

                    Children = {
                        // Stitched: three layout levels, one Yoga tree
                        CreatePanel(
                            "Stitched flex",
                            new Layout {
                                FlexController = {
                                    FlexDirection = FlexDirection.Column,
                                    Gap = 1.pt
                                },

                                Children = {
                                    CreateRow(new Color(0f, 0.75f, 1f)),
                                    CreateRow(new Color(1f, 0.2f, 0.8f)),
                                    CreateRow(new Color(1f, 0.8f, 0.2f))
                                }
                            }
                        ),

                        // Separate tree, joined through measuring. The slot fits the largest ring,
                        // so only the ring changes size and nothing around it moves
                        CreatePanel(
                            "Custom controller",
                            new Layout {
                                FlexController = {
                                    JustifyContent = Justify.Center,
                                    AlignItems = Align.Center
                                },

                                FlexItem = {
                                    Size = RingSize(MaxRadius).pt
                                },

                                Children = {
                                    ring
                                }
                            }
                        )
                    }
                }
            }
        }.Use();
    }

    private static float RingSize(float radius) => radius * 2f + 6f;

    private static Label CreateNumber(int index) {
        return new Label {
            Text = (index + 1).ToString(),
            FontSize = 4f,
            Color = Color.HSVToRGB(index / (float)ItemCount, 0.6f, 1f)
        };
    }

    private static Layout CreatePanel(string caption, ILayoutItem content) {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 1.5f.pt
            },

            Children = {
                new Label {
                    Text = caption,
                    FontSize = 3.2f,
                    FontStyle = FontStyles.Bold,
                    Color = Color.white with { a = 0.7f }
                },

                content
            }
        };
    }

    private static Layout CreateRow(Color color) {
        return new Layout {
            FlexController = {
                Gap = 1.pt
            },

            Children = {
                CreateBlock(color, 4f),
                CreateBlock(color, 8f),
                CreateBlock(color, 4f)
            }
        };
    }

    private static Background CreateBlock(Color color, float width) {
        return new Background {
            FlexItem = {
                Size = new() { x = width.pt, y = 4.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = color
        };
    }
}

/// <summary>
/// Places children evenly on a circle around the center of the box it's given.
/// </summary>
internal class RadialLayoutController : ILayoutController {
    public float Radius {
        get => _radius;
        set {
            _radius = value;
            LayoutControllerUpdatedEvent?.Invoke();
        }
    }

    public int ChildCount => _children.Count;

    public event Action? LayoutControllerUpdatedEvent;

    private readonly List<ILayoutItem> _children = new();
    private readonly List<LayoutDimensions> _dimensions = new();
    private float _radius = 10f;
    private Vector2 _size;

    #region Context

    // No context type, so a parent never stitches this controller into its tree and measures it instead
    public Type? ContextType => null;

    public object CreateContext() => new object();

    public void ProvideContext(object? context) { }

    #endregion

    #region Children

    public void InsertChild(ILayoutItem child, int index) {
        _children.Insert(index, child);
        LayoutControllerUpdatedEvent?.Invoke();
    }

    public void RemoveChild(ILayoutItem child) {
        _children.RemoveAll(x => x.EqualsToLayoutItem(child));
        LayoutControllerUpdatedEvent?.Invoke();
    }

    public void RemoveAllChildren() {
        _children.Clear();
        LayoutControllerUpdatedEvent?.Invoke();
    }

    public bool HasChild(ILayoutItem child) {
        return _children.Exists(x => x.EqualsToLayoutItem(child));
    }

    public void UpdateChild(ILayoutItem item) { }

    public void UpdateSelf(ILayoutItem item) { }

    #endregion

    #region Calculation

    public void Recalculate(Vector2 constraints) {
        _dimensions.Clear();

        var largest = Vector2.zero;
        var sizes = new Vector2[_children.Count];

        for (var i = 0; i < _children.Count; i++) {
            sizes[i] = _children[i].Measure(float.NaN, Reactive.MeasureMode.Undefined, float.NaN, Reactive.MeasureMode.Undefined);
            largest = Vector2.Max(largest, sizes[i]);
        }

        var preferred = Vector2.one * (_radius * 2f) + largest;
        _size = constraints.x > 0f && constraints.y > 0f ? constraints : preferred;

        var center = _size / 2f;

        for (var i = 0; i < _children.Count; i++) {
            var angle = i / (float)_children.Count * Mathf.PI * 2f - Mathf.PI / 2f;
            var position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
            var size = sizes[i];

            _dimensions.Add(new LayoutDimensions(position.y - size.y / 2f, position.x - size.x / 2f, size.x, size.y));
        }
    }

    public LayoutDimensions GetSelfDimensions() {
        return new LayoutDimensions(0f, 0f, _size.x, _size.y);
    }

    public LayoutDimensions GetChildDimensions(ILayoutItem child) {
        var index = _children.FindIndex(x => x.EqualsToLayoutItem(child));
        return index >= 0 && index < _dimensions.Count ? _dimensions[index] : default;
    }

    // How the flex parent asks this tree for its size when it isn't fixed
    public Vector2 Measure(float width, Reactive.MeasureMode widthMode, float height, Reactive.MeasureMode heightMode) {
        Recalculate(Vector2.zero);
        return LayoutTool.MeasureNode(_size, width, widthMode, height, heightMode);
    }

    #endregion
}
