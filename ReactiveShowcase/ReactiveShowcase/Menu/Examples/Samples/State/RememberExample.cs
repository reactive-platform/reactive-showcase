using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Examples;

internal class RememberExample : Example {
    public override string Title => "Remember";
    public override Topic Topic => Topic.State;
    public override Difficulty Difficulty => Difficulty.Basics;
    public override bool Centered => true;
    public override string Summary => "Creating a state and changing its value.";

    public override string Explanation =>
        "Remember creates a state: a value that tells whoever reads it when it changes. The buttons only change " +
        "count.Value and know nothing about the label. The label reads the count through sText, so it follows on " +
        "its own. That s-property is a binding, the next example is about them.";

    public override IReactiveComponent CreateDemo() => new RememberDemo();
}

internal class RememberDemo : ReactiveComponent {
    protected override GameObject Construct() {
        var count = Remember(0);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Label {
                    FontSize = 9f,
                    sText = count.Map(x => x.ToString())
                },

                new Layout {
                    FlexController = {
                        Gap = 1.pt
                    },

                    Children = {
                        new BsTextButton {
                            Text = "-1",
                            OnClick = () => count.Value--
                        },

                        new BsPrimaryTextButton {
                            Text = "+1",
                            OnClick = () => count.Value++
                        }
                    }
                }
            }
        }.Use();
    }
}
