using System.Collections.Generic;
using HMUI;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Image = Reactive.BeatSaber.Components.Image;
using UImage = UnityEngine.UI.Image;

namespace ReactiveShowcase.Showcase;

internal class ImagesPage : ShowcasePage {
    private readonly List<Image> _fills = new();
    private Spinner _spinner = null!;
    private Image _heart = null!;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var vertical = Remember(false);
        var direction = vertical.Map(x => x ? ImageView.GradientDirection.Vertical : ImageView.GradientDirection.Horizontal);

        var skew = RememberAnimated(0f, 900.ms, AnimationCurve.EaseInOutBack);
        skew.OnFinish = x => skew.TargetValue = x > 0f ? -0.4f : 0.4f;
        skew.TargetValue = 0.4f;

        var icons = new[] {
            GameResources.EditIcon,
            GameResources.SearchIcon,
            GameResources.CloseIcon,
            GameResources.ArrowIcon,
            GameResources.CaretIcon,
            GameResources.CircleIcon,
            GameResources.ArrowUpIcon,
            BeatSaberResources.Sprites.spinnerIcon
        };

        var gallery = Row();

        for (var i = 0; i < icons.Length; i++) {
            var shift = i / (float)icons.Length;

            gallery.Children.Add(
                new Image {
                    FlexItem = { Size = 7.pt },
                    Sprite = icons[i],
                    PreserveAspect = true,
                    sColor = Palette.Map(x => ShowcaseTheme.ShiftHue(x.Primary, shift))
                }
            );
        }

        return [
            Section(
                "Gradients",
                "Gradient colors are separate from the main color, which tints the whole image on top of them.",
                Row(
                    CreateGradient(direction, Palette.Map(x => x.Primary), Palette.Map(x => x.Secondary)),
                    CreateGradient(direction, Palette.Map(x => x.Primary), Palette.Map(x => x.Primary with { a = 0f })),
                    CreateGradient(direction, Palette.Map(x => x.Secondary), Palette.Map(x => x.Surface))
                ),
                ToggleRow("Vertical", false, x => vertical.Value = x)
            ),

            Section(
                "Fill",
                "Filled images with every fill method, the amounts are animated every frame.",
                Row(
                    CreateFill(UImage.FillMethod.Radial360, GameResources.CircleIcon, 12.pt),
                    CreateFill(UImage.FillMethod.Radial180, GameResources.CircleIcon, 12.pt),
                    CreateFill(UImage.FillMethod.Radial90, GameResources.CircleIcon, 12.pt),
                    CreateFill(UImage.FillMethod.Horizontal, BeatSaberResources.Sprites.rectangle, new YogaVector(30.pt, 4.pt)),
                    CreateFill(UImage.FillMethod.Vertical, BeatSaberResources.Sprites.rectangle, new YogaVector(4.pt, 12.pt))
                )
            ),

            Section(
                "Skew",
                "Skew is animated with a back curve, so it overshoots both ways.",
                Row(
                    new Background {
                        FlexItem = {
                            Size = new() { x = 30.pt, y = 8.pt }
                        },

                        Sprite = BeatSaberResources.Sprites.background,
                        PixelsPerUnit = 10f,
                        sSkew = skew,
                        sColor = Palette.Map(x => x.Primary with { a = 0.6f })
                    },

                    new Image {
                        FlexItem = { Size = 8.pt },
                        Sprite = GameResources.ArrowIcon,
                        PreserveAspect = true,
                        sSkew = skew,
                        sColor = Palette.Map(x => x.Secondary)
                    }
                )
            ),

            Section(
                "Spinner and pulse",
                "The spinner speed comes from the slider, the circle is scaled and recolored every frame.",
                Row(
                    new Spinner {
                        FlexItem = { Size = 10.pt },
                        Image = { sColor = Palette.Map(x => x.Primary) }
                    }.Bind(ref _spinner),

                    new BsSlider {
                        sColors = Palette.Map(ShowcaseStyles.Slider),
                        MinValue = -100f,
                        MaxValue = 200f,
                        ValueStep = 5f,
                        Value = 50f,
                        OnValueChanged = x => _spinner.RotationSpeed = x
                    },

                    new Image {
                        FlexItem = { Size = 10.pt },
                        Sprite = GameResources.CircleIcon,
                        PreserveAspect = true
                    }.Bind(ref _heart)
                )
            ),

            Section(
                "Sprites",
                "Icons from GameResources, spread around the primary hue.",
                gallery
            ),

            Sizes(
                "Sliced backgrounds keep their corners, the gradient spans the full size.",
                CreateSizeSample,
                new(6f, 4f),
                new(60f, 24f),
                new(10f, 6f), new(24f, 12f), new(40f, 18f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new Background {
            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 8f,
            Color = Color.white,
            UseGradient = true,
            sGradientColor0 = Palette.Map(x => x.Primary),
            sGradientColor1 = Palette.Map(x => x.Secondary)
        };
    }

    [StateGen]
    private static Background CreateGradient(
        MappedState<bool, ImageView.GradientDirection> direction,
        MappedState<ShowcasePalette, Color> from,
        MappedState<ShowcasePalette, Color> to
    ) {
        return new Background {
            FlexItem = {
                Size = new() { x = 24.pt, y = 12.pt }
            },

            Sprite = BeatSaberResources.Sprites.background,
            PixelsPerUnit = 8f,
            Color = Color.white,
            UseGradient = true,
            sGradientDirection = direction,
            sGradientColor0 = from,
            sGradientColor1 = to
        };
    }

    [StateGen]
    private Image CreateFill(UImage.FillMethod method, Sprite sprite, YogaVector size) {
        return new Image {
            FlexItem = { Size = size },
            Sprite = sprite,
            ImageType = UImage.Type.Filled,
            FillMethod = method,
            sColor = Palette.Map(x => x.Primary)
        }.With(x => _fills.Add(x));
    }

    protected override void OnUpdate() {
        for (var i = 0; i < _fills.Count; i++) {
            _fills[i].FillAmount = Mathf.PingPong(Time.time * 0.6f + i * 0.2f, 1f);
        }

        var pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 4f);
        var palette = Palette.Value;

        _heart.ContentTransform.localScale = Vector3.one * (0.8f + 0.25f * pulse);
        _heart.Color = Color.Lerp(palette.Primary, palette.Secondary, pulse);
    }
}
