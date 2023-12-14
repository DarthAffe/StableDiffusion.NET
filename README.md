# StableDiffusion.NET

Based on
https://github.com/seasonjs/stable-diffusion.cpp-build
https://github.com/leejet/stable-diffusion.cpp

## Usage
### Setup
Run `build.bat` to build the native libs (modify params like CUDA-builds if needed)

### Example
```csharp
using StableDiffusionModel sd = new(@"<path_to_model>", new ModelParameter());
using StableDiffusionParameter parameter = new StableDiffusionParameter();
using Image image = sd.TextToImage("<prompt>", parameter);
```