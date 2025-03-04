﻿using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using HPPH;
using HPPH.System.Drawing;
using Microsoft.Win32;
using StableDiffusion.NET;

namespace ImageCreationUI;

public class MainWindowViewModel : INotifyPropertyChanged
{
    #region Properties & Fields

    private DiffusionModel? _model;

    private bool _isStableDiffusionSelected = true;
    public bool IsStableDiffusionSelected
    {
        get => _isStableDiffusionSelected;
        set => SetProperty(ref _isStableDiffusionSelected, value);
    }

    private bool _isFluxSelected;
    public bool IsFluxSelected
    {
        get => _isFluxSelected;
        set => SetProperty(ref _isFluxSelected, value);
    }

    private string _modelPath = string.Empty;
    public string ModelPath
    {
        get => _modelPath;
        set => SetProperty(ref _modelPath, value);
    }

    private string _clipLPath;
    public string ClipLPath
    {
        get => _clipLPath;
        set => SetProperty(ref _clipLPath, value);
    }

    private string _t5xxlPath;
    public string T5xxlPath
    {
        get => _t5xxlPath;
        set => SetProperty(ref _t5xxlPath, value);
    }

    private string _diffusionModelPath;
    public string DiffusionModelPath
    {
        get => _diffusionModelPath;
        set => SetProperty(ref _diffusionModelPath, value);
    }

    private string _vaePath = string.Empty;
    public string VaePath
    {
        get => _vaePath;
        set => SetProperty(ref _vaePath, value);
    }

    private Schedule _schedule = Schedule.Default;
    public Schedule Schedule
    {
        get => _schedule;
        set => SetProperty(ref _schedule, value);
    }

    private bool _flashAttention = true;
    public bool FlashAttention
    {
        get => _flashAttention;
        set => SetProperty(ref _flashAttention, value);
    }

    private string _prompt = string.Empty;
    public string Prompt
    {
        get => _prompt;
        set => SetProperty(ref _prompt, value);
    }

    private string _antiPrompt = string.Empty;
    public string AntiPrompt
    {
        get => _antiPrompt;
        set => SetProperty(ref _antiPrompt, value);
    }

    private int _width = 1024;
    public int Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    private int _height = 1024;
    public int Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    private float _cfg = 5f;
    public float Cfg
    {
        get => _cfg;
        set => SetProperty(ref _cfg, value);
    }

    private int _steps = 28;
    public int Steps
    {
        get => _steps;
        set => SetProperty(ref _steps, value);
    }

    private long _seed = -1;
    public long Seed
    {
        get => _seed;
        set => SetProperty(ref _seed, value);
    }

    private Sampler _sampleMethod = Sampler.Euler_A;
    public Sampler SampleMethod
    {
        get => _sampleMethod;
        set => SetProperty(ref _sampleMethod, value);
    }

    private string _image2ImageSourcePath = string.Empty;
    public string Image2ImageSourcePath
    {
        get => _image2ImageSourcePath;
        set
        {
            if (SetProperty(ref _image2ImageSourcePath, value))
            {
                try
                {
                    Image2ImageSource = ImageHelper.LoadImage(value).ConvertTo<ColorRGB>();
                }
                catch
                {
                    Image2ImageSource = null;
                }
            }
        }
    }

    private IImage<ColorRGB>? _image2ImageSource;
    public IImage<ColorRGB>? Image2ImageSource
    {
        get => _image2ImageSource;
        set => SetProperty(ref _image2ImageSource, value);
    }

    private IImage? _image;
    public IImage? Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    private string _log = string.Empty;
    public string Log
    {
        get => _log;
        set => SetProperty(ref _log, value);
    }

    private bool _isReady = true;
    public bool IsReady
    {
        get => _isReady;
        set => SetProperty(ref _isReady, value);
    }

    #endregion

    #region Commands

    private ActionCommand? _loadModelCommand;
    public ActionCommand LoadModelCommand => _loadModelCommand ??= new ActionCommand(LoadModel);

    private ActionCommand? _createImageCommand;
    public ActionCommand CreateImageCommand => _createImageCommand ??= new ActionCommand(CreateImage);

    private ActionCommand? _saveImageCommand;
    public ActionCommand SaveImageCommand => _saveImageCommand ??= new ActionCommand(SaveImage);

    private ActionCommand? _selectModelCommand;
    public ActionCommand SelectModelCommand => _selectModelCommand ??= new ActionCommand(SelectModel);

    private ActionCommand? _selectDiffusionModelCommand;
    public ActionCommand SelectDiffusionModelCommand => _selectDiffusionModelCommand ??= new ActionCommand(SelectDiffusionModel);

    private ActionCommand? _selectClipLCommand;
    public ActionCommand SelectClipLCommand => _selectClipLCommand ??= new ActionCommand(SelectClipL);

    private ActionCommand? _selectT5xxlCommand;
    public ActionCommand SelectT5xxlCommand => _selectT5xxlCommand ??= new ActionCommand(SelectT5xxl);

    private ActionCommand? _selectVaeCommand;
    public ActionCommand SelectVaeCommand => _selectVaeCommand ??= new ActionCommand(SelectVae);

    private ActionCommand? _selectImage2ImageSourceCommand;
    public ActionCommand SelectImage2ImageSourceCommand => _selectImage2ImageSourceCommand ??= new ActionCommand(SelectImage2ImageSource);

    #endregion

    #region Constructors

    public MainWindowViewModel()
    {
        try
        {
            StableDiffusionCpp.Log += (_, args) => LogLine($"LOG [{args.Level}]: {args.Text}", false);
            StableDiffusionCpp.Progress += (_, args) => LogLine($"PROGRESS {args.Step} / {args.Steps} ({(args.Progress * 100):N2} %) {args.IterationsPerSecond:N2} it/s ({args.Time})");
        }
        catch (Exception ex)
        {
            LogLine($"Failed to load stable-diffusion.cpp libraries!{Environment.NewLine}{ex.Message}");
        }
    }

    #endregion

    #region Methods

    private async void LoadModel()
    {
        try
        {
            IsReady = false;

            _model?.Dispose();

            bool restoreDefaultParameters = false;

            if (IsStableDiffusionSelected)
            {
                restoreDefaultParameters = _model?.ModelParameter.DiffusionModelType != DiffusionModelType.StableDiffusion;

                LogLine($"Loading stable diffusion-model '{ModelPath}'");
                _model = await Task.Run(() => ModelBuilder.StableDiffusion(ModelPath).WithMultithreading().WithVae(VaePath).WithSchedule(Schedule).WithFlashAttention(FlashAttention).Build());
            }
            else if (IsFluxSelected)
            {
                restoreDefaultParameters = _model?.ModelParameter.DiffusionModelType != DiffusionModelType.Flux;

                LogLine($"Loading flux-model '{DiffusionModelPath}'");
                _model = await Task.Run(() => ModelBuilder.Flux(DiffusionModelPath, ClipLPath, T5xxlPath, VaePath).WithMultithreading().WithSchedule(Schedule).WithFlashAttention(FlashAttention).Build());
            }
            else
            {
                LogLine("No model-type selected :(");
            }

            if (restoreDefaultParameters && (_model != null))
            {
                DiffusionParameter parameter = _model.GetDefaultParameter();
                AntiPrompt = parameter.NegativePrompt;
                Width = parameter.Width;
                Height = parameter.Height;
                Steps = parameter.SampleSteps;
                Seed = parameter.Seed;
                SampleMethod = parameter.SampleMethod;
                Cfg = _model.ModelParameter.DiffusionModelType == DiffusionModelType.Flux ? parameter.Guidance : parameter.CfgScale;
            }
        }
        catch (Exception ex)
        {
            LogLine($"Failed to load model ...{Environment.NewLine}{ex.Message}");
        }
        finally
        {
            IsReady = true;
        }
    }

    private async void CreateImage()
    {
        if (_model == null) return;

        try
        {
            IsReady = false;

            DiffusionParameter parameter = _model.GetDefaultParameter()
                                                 .WithNegativePrompt(AntiPrompt)
                                                 .WithSize(Width, Height)
                                                 .WithSteps(Steps)
                                                 .WithSeed(Seed)
                                                 .WithSampler(SampleMethod);
            if (_model.ModelParameter.DiffusionModelType == DiffusionModelType.StableDiffusion)
                parameter = parameter.WithCfg(Cfg);
            else if (_model.ModelParameter.DiffusionModelType == DiffusionModelType.Flux)
                parameter = parameter.WithGuidance(Cfg);

            if (Image2ImageSource == null)
            {
                LogLine("Creating image ...");
                Image = await Task.Run(() => _model?.TextToImage(Prompt, parameter));
            }
            else
            {
                LogLine("Manipulating image ...");
                Image = await Task.Run(() => _model?.ImageToImage(Prompt, Image2ImageSource, parameter));
            }

            LogLine("done!");
        }
        catch (Exception ex)
        {
            LogLine($"Failed to create image ...{Environment.NewLine}{ex.Message}");
        }
        finally
        {
            IsReady = true;
        }
    }

    private void SaveImage()
    {
        try
        {
            if (Image == null) return;

            SaveFileDialog saveFileDialog = new() { Filter = "PNG File (*.png)|*.png" };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, Image.ToPng());
                LogLine($"Image saved to '{saveFileDialog.FileName}'!");
            }
        }
        catch (Exception ex)
        {
            LogLine($"Failed to save image ...{Environment.NewLine}{ex.Message}");
        }
    }

    private void SelectModel()
    {
        OpenFileDialog openFileDialog = new() { Filter = "Stable Diffusion Model|*.*" };
        if (openFileDialog.ShowDialog() == true)
            ModelPath = openFileDialog.FileName;
    }

    private void SelectDiffusionModel()
    {
        OpenFileDialog openFileDialog = new() { Filter = "Diffusion Model|*.*" };
        if (openFileDialog.ShowDialog() == true)
            DiffusionModelPath = openFileDialog.FileName;
    }

    private void SelectClipL()
    {
        OpenFileDialog openFileDialog = new() { Filter = "ClipL|*.*" };
        if (openFileDialog.ShowDialog() == true)
            ClipLPath = openFileDialog.FileName;
    }

    private void SelectT5xxl()
    {
        OpenFileDialog openFileDialog = new() { Filter = "T5xxl|*.*" };
        if (openFileDialog.ShowDialog() == true)
            T5xxlPath = openFileDialog.FileName;
    }

    private void SelectVae()
    {
        OpenFileDialog openFileDialog = new() { Filter = "VAE|*.*" };
        if (openFileDialog.ShowDialog() == true)
            VaePath = openFileDialog.FileName;
    }

    private void SelectImage2ImageSource()
    {
        IEnumerable<string> codecs = ["All Files (*.*)|*.*", .. ImageCodecInfo.GetImageDecoders().Select(c =>
                                      {
                                          string codecName = c.CodecName![8..].Replace("Codec", "Files").Trim();
                                          return $"{codecName} ({c.FilenameExtension})|{c.FilenameExtension}";
                                      })];

        OpenFileDialog openFileDialog = new() { Filter = string.Join('|', codecs) };
        if (openFileDialog.ShowDialog() == true)
            Image2ImageSourcePath = openFileDialog.FileName;
    }

    private void LogLine(string line, bool appendNewLine = true)
    {
        if (appendNewLine)
            Log += line + Environment.NewLine;
        else
            Log += line;
    }

    #endregion

    #region NotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}