## About

A showcase of ReactiveUI with every built-in component, and an in-game playground of examples 
that teach the library step by step, each with its source.

**Dependencies:** SiraUtil, BSML, Yoga

## Building

Reactive is included as a submodule and built together with the mod, so the only thing to set up is the game path.

1. Clone with the submodule:
   ```
   git clone --recurse-submodules https://github.com/reactive-platform/reactive-showcase
   ```
2. Create `ReactiveShowcase/ReactiveShowcase/ReactiveShowcase.csproj.user` with the path to your game:
   ```xml
   <Project>
       <PropertyGroup>
           <BeatSaberDir>path/to/Beat Saber</BeatSaberDir>
       </PropertyGroup>
   </Project>
   ```
   Reactive is built with the same path, so this is the only file to create.
3. Build `ReactiveShowcase/ReactiveShowcase.sln`. Reactive is part of the solution, so it's built too, and both get
   installed into the game.

To update Reactive, run `git submodule update --remote`.
