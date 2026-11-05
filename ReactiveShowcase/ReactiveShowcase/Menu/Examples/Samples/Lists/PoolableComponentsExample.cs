using System;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class PoolableComponentsExample : Example {
    public override string Title => "IPoolableComponent";
    public override Topic Topic => Topic.Lists;
    public override Difficulty Difficulty => Difficulty.Expert;
    public override bool Centered => true;
    public override string Summary => "Toasts from the global pool, reset by OnSpawn and OnDespawn.";

    public override string Explanation =>
        "A component implementing IPoolableComponent can be spawned from GlobalReactivePool and given back when it's " +
        "done. The pool calls OnSpawn and OnDespawn, which is where a toast resets its timer and drops its callback, " +
        "since a reused component keeps whatever state it had. The pool is global, so leaving the example and coming " +
        "back still reuses the toasts built before.";

    public override IReactiveComponent CreateDemo() => new PoolableComponentsDemo();
}

internal class PoolableComponentsDemo : ReactiveComponent {
    private const int MaxToasts = 3;

    private State<int> _built = null!;
    private Layout _toasts = null!;
    private int _next;

    protected override GameObject Construct() {
        _built = Remember(Toast.Built);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsPrimaryTextButton {
                    Text = "Notify",
                    OnClick = Notify
                },

                new Label {
                    FontSize = 3.2f,
                    Color = Color.white with { a = 0.6f },
                    sText = _built.Map(x => $"Toasts ever built: {x}")
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Center,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = 50.pt, y = 20.pt }
                    }
                }.Bind(ref _toasts)
            }
        }.Use();
    }

    private void Notify() {
        if (_toasts.Children.Count >= MaxToasts) {
            Despawn((Toast)_toasts.Children.First());
        }

        // Reuses a despawned toast if there is one, builds a new one otherwise
        var toast = GlobalReactivePool.Spawn<Toast>();

        toast.Text = $"Notification #{++_next}";
        toast.OnFinished = Despawn;

        _toasts.Children.Add(toast);
        _built.Value = Toast.Built;
    }

    private void Despawn(Toast toast) {
        _toasts.Children.Remove(toast);
        GlobalReactivePool.Despawn(toast);
    }

    protected override void OnDestroy() {
        // Toasts still showing go back to the pool instead of dying with the demo
        Array.ForEach(_toasts.Children.OfType<Toast>().ToArray(), Despawn);
    }
}

/// <summary>
/// A message that fades out on its own and reports when it's done.
/// </summary>
internal class Toast : ReactiveComponent, IPoolableComponent {
    private const float Lifetime = 2.5f;

    /// <summary>
    /// How many toasts were ever constructed, across every visit.
    /// </summary>
    public static int Built { get; private set; }

    public string Text {
        set => _label.Text = value;
    }

    public Action<Toast>? OnFinished { get; set; }

    private Label _label = null!;
    private State<float> _age = null!;

    protected override GameObject Construct() {
        Built++;
        _age = Remember(0f);

        return new Background {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Center
            },

            FlexItem = {
                Size = new() { x = 44.pt, y = 6.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 10f,

            sColor = _age.Map(x => new Color(0f, 0.75f, 1f, 0.6f * Mathf.Clamp01(Lifetime - x))),

            Children = {
                new Label {
                    FontSize = 3.2f
                }.Bind(ref _label)
            }
        }.Use();
    }

    protected override void OnUpdate() {
        _age.Value += Time.deltaTime;

        if (_age.Value >= Lifetime) {
            OnFinished?.Invoke(this);
        }
    }

    // A reused toast keeps its state, so spawning starts it over
    void IPoolableComponent.OnSpawn() {
        _age.Value = 0f;
    }

    void IPoolableComponent.OnDespawn() {
        OnFinished = null;
    }
}
