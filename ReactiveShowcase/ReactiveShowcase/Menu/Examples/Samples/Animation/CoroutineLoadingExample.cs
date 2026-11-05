using System.Collections;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class CoroutineLoadingExample : Example {
    public override string Title => "Coroutines";
    public override Topic Topic => Topic.Animation;
    public override Difficulty Difficulty => Difficulty.Advanced;
    public override bool Centered => true;
    public override string Summary => "StartCoroutine waiting for a fake request, with a loading state around it.";

    public override string Explanation =>
        "Components can run coroutines with StartCoroutine, and they stop together with the component. The fake " +
        "request flips a loading state, waits, then writes the result. The spinner and the result only read that " +
        "state, and Cancel stops the coroutine with StopAllCoroutines.";

    public override IReactiveComponent CreateDemo() => new CoroutineLoadingDemo();
}

internal class CoroutineLoadingDemo : ReactiveComponent {
    private static readonly string[] songs = ["Night Drive", "Neon Tides", "Glass Hearts", "Static Bloom", "Low Orbit", "Paper Moon"];

    private State<bool> _loading = null!;
    private State<string> _result = null!;
    private int _requests;

    protected override GameObject Construct() {
        _loading = Remember(false);
        _result = Remember("Press Load to fetch songs");

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 56.pt, y = 16.pt }
                    },

                    Children = {
                        new Spinner {
                            FlexItem = {
                                Size = 8.pt
                            },

                            sEnabled = _loading
                        },

                        new Label {
                            FontSize = 3.5f,
                            EnableWrapping = true,
                            Alignment = TextAlignmentOptions.Center,
                            sEnabled = _loading.Map(x => !x),
                            sText = _result
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        new BsPrimaryTextButton {
                            Text = "Load",
                            sInteractable = _loading.Map(x => !x),
                            OnClick = () => StartCoroutine(FetchSongs())
                        },

                        new BsTextButton {
                            Text = "Cancel",
                            sInteractable = _loading,
                            OnClick = Cancel
                        }
                    }
                }
            }
        }.Use();
    }

    private IEnumerator FetchSongs() {
        _loading.Value = true;

        // Stands in for a web request
        yield return new WaitForSecondsRealtime(1.5f);

        _requests++;

        var first = Random.Range(0, songs.Length - 3);
        _result.Value = $"Request {_requests}:\n{songs[first]}, {songs[first + 1]}, {songs[first + 2]}";
        _loading.Value = false;
    }

    private void Cancel() {
        StopAllCoroutines();

        _result.Value = "Cancelled";
        _loading.Value = false;
    }
}
