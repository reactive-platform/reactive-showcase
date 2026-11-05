using System.Collections.Generic;

namespace ReactiveShowcase.Examples;

/// <summary>
/// Every example in the order to learn them in: by topic, each step building on the previous ones.
/// </summary>
internal static class ExampleCatalog {
    public static readonly IReadOnlyList<Example> All = [
        // Introduction
        new HelloLabelExample(),
        new BuildingBlocksExample(),
        new ClickExample(),

        // State
        new RememberExample(),
        new BindingsExample(),
        new MapExample(),
        new WhereExample(),
        new DerivedStateExample(),
        new TwoWayBindingExample(),
        new AddCallbackExample(),
        new NestedBindingsExample(),
        new ComponentBindingsExample(),

        // Layout
        new DirectionGapExample(),
        new FlexboxExample(),
        new PaddingMarginExample(),
        new WrappingExample(),
        new GrowShrinkExample(),
        new SizingUnitsExample(),
        new AbsolutePositionExample(),
        new ConditionalContentExample(),
        new LayoutStitchingExample(),

        // Input
        new PointerEventsExample(),
        new ScrollAdjustExample(),
        new OwnButtonExample(),
        new HoldToConfirmExample(),
        new DragPadExample(),
        new DragDropExample(),

        // Animation & time
        new AnimatedStateExample(),
        new CurvesExample(),
        new AnimationPlayerExample(),
        new AnimationTracksExample(),
        new CustomInterpolatorExample(),
        new UpdateLoopExample(),
        new CoroutineLoadingExample(),

        // Lists and reuse
        new RepeaterBasicsExample(),
        new RepeaterExample(),
        new VirtualizedTableExample(),
        new SelectionExample(),
        new PoolingExample(),
        new PoolableComponentsExample(),
    ];
}
