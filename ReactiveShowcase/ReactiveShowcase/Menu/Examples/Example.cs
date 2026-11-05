using Reactive;

namespace ReactiveShowcase.Examples;

internal enum Difficulty {
    Basics = 1,
    Easy,
    Intermediate,
    Advanced,
    Expert
}

/// <summary>
/// What an example is about. The hub groups examples by it, in this order.
/// </summary>
internal enum Topic {
    Introduction,
    State,
    Layout,
    Input,
    Animation,
    Lists
}

/// <summary>
/// A single playground entry: a live demo, the text explaining it and the source file it lives in.
/// </summary>
internal abstract class Example {
    public abstract string Title { get; }
    public abstract Topic Topic { get; }
    public abstract Difficulty Difficulty { get; }

    public abstract string Summary { get; }

    /// <summary>
    /// A few sentences shown next to the demo, explaining what to look at in the code.
    /// </summary>
    public abstract string Explanation { get; }

    /// <summary>
    /// Whether the demo can sit in the middle of its area. Only for demos that never change size:
    /// the others hang from the top, so growing or shrinking never moves their controls.
    /// </summary>
    public virtual bool Centered => false;

    /// <summary>
    /// The file shown in the code viewer. Every example file is embedded under its own name.
    /// </summary>
    public virtual string SourceFile => $"{GetType().Name}.cs";

    public abstract IReactiveComponent CreateDemo();
}
