using UnityEngine;

namespace ReactiveShowcase.Examples;

internal static class DifficultyInfo {
    public static string Name(Difficulty difficulty) {
        return difficulty switch {
            Difficulty.Basics => "Basics",
            Difficulty.Easy => "Easy",
            Difficulty.Intermediate => "Intermediate",
            Difficulty.Advanced => "Advanced",
            _ => "Expert"
        };
    }

    public static Color Color(Difficulty difficulty) {
        return difficulty switch {
            Difficulty.Basics => new(0.35f, 0.9f, 0.45f),
            Difficulty.Easy => new(0.55f, 0.85f, 1f),
            Difficulty.Intermediate => new(1f, 0.85f, 0.3f),
            Difficulty.Advanced => new(1f, 0.55f, 0.25f),
            _ => new(1f, 0.3f, 0.4f)
        };
    }

    public static string Dots(Difficulty difficulty) {
        var level = (int)difficulty;
        return new string('●', level) + new string('○', 5 - level);
    }
}
