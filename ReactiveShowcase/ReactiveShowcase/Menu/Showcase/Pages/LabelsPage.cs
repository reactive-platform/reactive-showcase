using System.Collections.Generic;
using System.Text;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class LabelsPage : ShowcasePage {
    private const string TypewriterText = "Reactive builds Beat Saber UIs out of components and states. This line types itself.";
    private const string RainbowText = "Every letter here has its own color";

    private Label _typewriter = null!;
    private Label _rainbow = null!;
    private float _typed;

    private readonly StringBuilder _builder = new();

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var width = RememberAnimated(20f, 1500.ms, AnimationCurve.EaseInOutSine);
        width.OnFinish = x => width.TargetValue = x > 40f ? 20f : 70f;
        width.TargetValue = 70f;

        return [
            Section(
                "Styles",
                "FontStyle flags and rich text.",
                Row(
                    CreateStyled("Bold", FontStyles.Bold),
                    CreateStyled("Italic", FontStyles.Italic),
                    CreateStyled("Underline", FontStyles.Underline),
                    CreateStyled("Strikethrough", FontStyles.Strikethrough),
                    CreateStyled("Small caps", FontStyles.SmallCaps),
                    CreateStyled("Upper", FontStyles.UpperCase)
                ),
                new Label {
                    RichText = true,
                    FontSize = 4f,
                    sText = Palette.Map(x =>
                        $"Rich text with <color=#{ColorUtility.ToHtmlStringRGB(x.Primary)}>primary</color>, " +
                        $"<color=#{ColorUtility.ToHtmlStringRGB(x.Secondary)}>secondary</color>, " +
                        "<size=150%>big</size> and <alpha=#66>faded</alpha> parts"
                    )
                }
            ),

            Section(
                "Typewriter",
                "The text is cut every frame, with a blinking caret at the end.",
                new Label {
                    FlexItem = {
                        Size = new() { y = 10.pt }
                    },

                    FontSize = 4f,
                    EnableWrapping = true,
                    RichText = true,
                    Alignment = TextAlignmentOptions.TopLeft
                }.Bind(ref _typewriter),
                new BsTextButton {
                    FlexItem = {
                        AlignSelf = Align.FlexStart
                    },

                    Text = "Restart",
                    sColors = Palette.Map(ShowcaseStyles.Button),
                    OnClick = () => _typed = 0f
                }
            ),

            Section(
                "Per-letter colors",
                "Rich text rebuilt every frame from the palette, a wave of hues running through it.",
                new Label {
                    FontSize = 5f,
                    RichText = true
                }.Bind(ref _rainbow)
            ),

            Section(
                "Auto sizing",
                "The box width is animated, the label shrinks the font to fit between FontSizeMin and FontSizeMax.",
                new Background {
                    FlexItem = {
                        sSize = width.Map(x => new YogaVector(x.pt, 10.pt))
                    },

                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center,
                        Padding = 1.pt
                    },

                    Sprite = Reactive.BeatSaber.BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = Palette.Map(x => x.Primary with { a = 0.25f }),

                    Children = {
                        new Label {
                            FlexItem = {
                                Flex = 1f,
                                Size = new() { y = 100.pct }
                            },

                            Text = "Squeeze me",
                            EnableAutoSizing = true,
                            FontSizeMin = 1.5f,
                            FontSizeMax = 7f,
                            EnableWrapping = false
                        }
                    }
                }
            ),

            Sizes(
                "Wrapping text reflows as the width changes, auto sizing shrinks it to fit the height.",
                CreateSizeSample,
                new(10f, 4f),
                new(70f, 24f),
                new(20f, 8f), new(36f, 12f), new(56f, 16f)
            )
        ];
    }

    private ILayoutItem CreateSizeSample() {
        return new Label {
            Text = "Labels wrap and shrink to fit the space they get",
            EnableWrapping = true,
            EnableAutoSizing = true,
            FontSizeMin = 1.5f,
            FontSizeMax = 5f
        };
    }

    [StateGen]
    private static Label CreateStyled(string text, FontStyles style) {
        return new Label {
            Text = text,
            FontStyle = style,
            FontSize = 4f,
            sColor = Palette.Map(x => Color.Lerp(Color.white, x.Primary, 0.4f))
        };
    }

    protected override void OnUpdate() {
        UpdateTypewriter();
        UpdateRainbow();
    }

    private void UpdateTypewriter() {
        if (_typed < TypewriterText.Length) {
            _typed += Time.deltaTime * 25f;
        }

        var count = Mathf.Min(TypewriterText.Length, (int)_typed);
        var caret = Mathf.Repeat(Time.time, 1f) < 0.5f ? "|" : "<alpha=#00>|</alpha>";

        _typewriter.Text = TypewriterText.Substring(0, count) + caret;
    }

    private void UpdateRainbow() {
        // Saturation is floored, so grayscale palettes still get a rainbow
        Color.RGBToHSV(Palette.Value.Primary, out var hue, out var saturation, out var value);
        saturation = Mathf.Max(saturation, 0.6f);
        value = Mathf.Max(value, 0.8f);

        _builder.Clear();

        for (var i = 0; i < RainbowText.Length; i++) {
            var color = Color.HSVToRGB(Mathf.Repeat(hue + i * 0.03f - Time.time * 0.3f, 1f), saturation, value);

            _builder
                .Append("<color=#")
                .Append(ColorUtility.ToHtmlStringRGB(color))
                .Append('>')
                .Append(RainbowText[i])
                .Append("</color>");
        }

        _rainbow.Text = _builder.ToString();
    }
}
