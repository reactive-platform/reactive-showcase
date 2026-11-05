using System;
using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class UpdateLoopExample : Example {
    public override string Title => "OnUpdate";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override string Summary => "OnUpdate adds the time, everything else only reads it.";

    public override string Explanation =>
        "Components get Unity-style hooks: OnUpdate runs every frame while the component is active, next to " +
        "OnEnable, OnStart and OnDestroy. The stopwatch only adds time there, everything else reads the elapsed state. " +
        "Laps are a list state, so recording one just assigns a new list.";

    public override IReactiveComponent CreateDemo() => new UpdateLoopDemo();
}

internal class UpdateLoopDemo : ReactiveComponent {
    private const int MaxLaps = 4;

    private readonly record struct Lap(int Number, float Time);

    private State<float> _elapsed = null!;
    private State<bool> _running = null!;
    private State<IReadOnlyList<Lap>> _laps = null!;
    private int _lapCount;

    protected override GameObject Construct() {
        _elapsed = Remember(0f);
        _running = Remember(false);
        _laps = Remember<IReadOnlyList<Lap>>([]);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    FontSize = 9f,
                    sText = _elapsed.Map(Format)
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        new BsPrimaryTextButton {
                            sText = _running.Map(x => x ? "Stop" : "Start"),
                            OnClick = () => _running.Value = !_running.Value
                        },

                        new BsTextButton {
                            Text = "Lap",
                            sInteractable = _running,
                            OnClick = AddLap
                        },

                        new BsTextButton {
                            Text = "Reset",
                            OnClick = Reset
                        }
                    }
                },

                new Label {
                    FontSize = 3.2f,
                    Alignment = TMPro.TextAlignmentOptions.Top,
                    Color = Color.white with { a = 0.7f },
                    sText = _laps.Map(x => string.Join("\n", x.Select(y => $"Lap {y.Number}   {Format(y.Time)}")))
                }
            }
        }.Use();
    }

    protected override void OnUpdate() {
        if (_running.Value) {
            _elapsed.Value += Time.unscaledDeltaTime;
        }
    }

    private void AddLap() {
        var laps = _laps.Value.Append(new Lap(++_lapCount, _elapsed.Value));
        _laps.Value = laps.Skip(Math.Max(0, _laps.Value.Count + 1 - MaxLaps)).ToArray();
    }

    private void Reset() {
        _running.Value = false;
        _elapsed.Value = 0f;
        _lapCount = 0;
        _laps.Value = [];
    }

    private static string Format(float seconds) {
        var time = TimeSpan.FromSeconds(seconds);
        return $"{time.Minutes:00}:{time.Seconds:00}.{time.Milliseconds / 10:00}";
    }
}
