using System;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class PoolingExample : Example {
    public override string Title => "ReactivePool";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override string Summary => "Reusing components instead of building new ones every time.";

    public override string Explanation =>
        "A ReactivePool hands out components and takes them back. Despawning disables and detaches a chip, spawning " +
        "reuses one if there is any, so the Created counter only grows when the pool runs dry. Pooled components keep " +
        "their state, which is why every spawn sets the number again.";

    public override IReactiveComponent CreateDemo() => new PoolingDemo();
}

internal class PoolingDemo : ReactiveComponent {
    private const int MaxChips = 10;

    private ReactivePool<PoolChip> _pool = null!;
    private Layout _chips = null!;
    private State<int> _active = null!;
    private State<int> _created = null!;
    private int _next;

    protected override GameObject Construct() {
        _active = Remember(0);
        _created = Remember(0);

        // Construct only runs when there is nothing to reuse
        _pool = new ReactivePool<PoolChip> {
            Construct = () => {
                _created.Value++;
                return new PoolChip();
            }
        };

        var stats = RememberDerived(_active, _created, (active, created) => $"Active {active}   Created {created}   Pooled {created - active}");

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
                        new BsPrimaryTextButton {
                            Text = "Spawn",
                            sInteractable = _active.Map(x => x < MaxChips),
                            OnClick = Spawn
                        },

                        new BsTextButton {
                            Text = "Clear",
                            OnClick = Clear
                        }
                    }
                },

                new Label {
                    FontSize = 3.2f,
                    sText = stats
                },

                new Label {
                    Text = "Click a chip to return it to the pool",
                    FontSize = 2.8f,
                    Color = Color.white with { a = 0.5f }
                },

                new Layout {
                    FlexController = {
                        FlexWrap = Wrap.Wrap,
                        JustifyContent = Justify.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 60.pt }
                    }
                }.Bind(ref _chips)
            }
        }.Use();
    }

    private void Spawn() {
        var chip = _pool.Spawn();

        chip.Number = ++_next;
        chip.OnClicked = Despawn;

        _chips.Children.Add(chip);
        _active.Value++;
    }

    private void Despawn(PoolChip chip) {
        _chips.Children.Remove(chip);
        _pool.Despawn(chip);
        _active.Value--;
    }

    private void Clear() {
        Array.ForEach(_pool.SpawnedComponents.ToArray(), Despawn);
    }
}

internal class PoolChip : ReactiveComponent {
    public int Number {
        set => _label.Text = $"#{value}";
    }

    public Action<PoolChip>? OnClicked { get; set; }

    private Label _label = null!;

    protected override GameObject Construct() {
        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { x = 10.pt, y = 6.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,
            Color = new Color(0f, 0.75f, 1f, 0.35f),

            Do = x => x.WithPointerEvents(onDown: _ => OnClicked?.Invoke(this)),

            Children = {
                new Label {
                    FontSize = 3f
                }.Bind(ref _label)
            }
        }.Use();
    }
}
