using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class AddCallbackExample : Example {
    public override string Title => "AddCallback";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "Running your own code when a state changes, now or only on changes.";

    public override string Explanation =>
        "Bindings set properties, AddCallback runs anything. By default the callback also runs once right away " +
        "with the current value, which is handy to apply it initially. With lazy: true it only runs on actual " +
        "changes. Both callbacks below add a line to the log, so you can see the immediate one start first.";

    public override IReactiveComponent CreateDemo() => new AddCallbackDemo();
}

internal class AddCallbackDemo : ReactiveComponent {
    private const int LogSize = 5;

    protected override GameObject Construct() {
        var number = Remember(1);
        var log = Remember<IReadOnlyList<string>>([]);

        void Write(string line) {
            var lines = log.Value.Append(line);
            log.Value = lines.Skip(Mathf.Max(0, log.Value.Count + 1 - LogSize)).ToArray();
        }

        // Runs now with 1, then on every change
        number.AddCallback(x => Write($"immediate: {x}"));

        // Only runs on changes
        number.AddCallback(x => Write($"lazy: {x}"), lazy: true);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 2.pt
                    },

                    Children = {
                        new Label {
                            FlexItem = {
                                Size = new() { x = 12.pt }
                            },

                            FontSize = 5f,
                            sText = number.Map(x => x.ToString())
                        },

                        new BsPrimaryTextButton {
                            Text = "+1",
                            OnClick = () => number.Value++
                        }
                    }
                },

                new Label {
                    FlexItem = {
                        Size = new() { y = 18.pt }
                    },

                    FontSize = 3f,
                    Alignment = TMPro.TextAlignmentOptions.Top,
                    sText = log.Map(x => string.Join("\n", x))
                }
            }
        }.Use();
    }
}
