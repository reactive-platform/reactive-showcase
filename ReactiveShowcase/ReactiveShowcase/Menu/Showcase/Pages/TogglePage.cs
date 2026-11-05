using System.Collections;
using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class TogglePage : ShowcasePage {
    private readonly BsToggle[] _wave = new BsToggle[10];
    private readonly BsToggle[] _bits = new BsToggle[6];

    private State<int> _counter = null!;
    private bool _counting;
    private float _countTimer;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.Toggle);
        _counter = Remember(0);

        var waveRow = Row();
        for (var i = 0; i < _wave.Length; i++) {
            waveRow.Children.Add(new BsToggle { sColors = colors }.Bind(ref _wave[i]));
        }

        var bitsRow = Row();
        for (var i = _bits.Length - 1; i >= 0; i--) {
            bitsRow.Children.Add(new BsToggle { sColors = colors, Interactable = false }.Bind(ref _bits[i]));
        }

        _counter.AddCallback(x => {
            for (var i = 0; i < _bits.Length; i++) {
                _bits[i].Toggled = (x & (1 << i)) != 0;
            }
        }, lazy: true);

        return [
            Section(
                "Variants",
                "The knob slides by default. Animated = false snaps it, Interactable = false ignores clicks.",
                Row(
                    Labeled("Stock", new BsToggle()),
                    Labeled("Themed", new BsToggle { Toggled = true, sColors = colors }),
                    Labeled("Not animated", new BsToggle { Animated = false, sColors = colors }),
                    Labeled("Disabled off", new BsToggle { Interactable = false, sColors = colors }),
                    Labeled("Disabled on", new BsToggle { Toggled = true, Interactable = false, sColors = colors })
                )
            ),

            Section(
                "Wave",
                "Toggles flipped from code one after another, each knob animating on its own.",
                waveRow,
                Row(
                    new BsTextButton {
                        Text = "Wave",
                        sColors = Palette.Map(ShowcaseStyles.Button),
                        OnClick = () => StartCoroutine(PlayWave())
                    },

                    new BsTextButton {
                        Text = "Invert",
                        sColors = Palette.Map(ShowcaseStyles.Button),
                        OnClick = () => {
                            foreach (var toggle in _wave) {
                                toggle.Toggled = !toggle.Toggled;
                            }
                        }
                    },

                    new BsTextButton {
                        Text = "Random",
                        sColors = Palette.Map(ShowcaseStyles.Button),
                        OnClick = () => {
                            foreach (var toggle in _wave) {
                                toggle.Toggled = Random.value > 0.5f;
                            }
                        }
                    }
                )
            ),

            Section(
                "Binary counter",
                "Read-only toggles displaying a counter in binary.",
                Row(
                    bitsRow,
                    new Label {
                        FontSize = 5f,
                        sText = _counter.Map(x => x.ToString()),
                        sColor = Palette.Map(x => x.Primary)
                    }
                ),
                ToggleRow("Count", false, x => _counting = x)
            ),

            Sizes(
                "The knob keeps its aspect and slides across whatever width it gets.",
                CreateSizeSample,
                new(6f, 3f),
                new(30f, 12f),
                new(8f, 4f), new(12f, 5f), new(20f, 8f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsToggle {
            Toggled = true,
            sColors = Palette.Map(ShowcaseStyles.Toggle)
        };
    }

    private IEnumerator PlayWave() {
        var target = !_wave[0].Toggled;

        foreach (var toggle in _wave) {
            toggle.Toggled = target;
            yield return new WaitForSeconds(0.06f);
        }
    }

    protected override void OnUpdate() {
        if (!_counting) {
            return;
        }

        _countTimer += Time.deltaTime;

        if (_countTimer >= 0.3f) {
            _countTimer = 0f;
            _counter.Value = (_counter.Value + 1) % (1 << _bits.Length);
        }
    }
}
