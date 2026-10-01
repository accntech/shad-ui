using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides functionality to include and load data templates from external XAML sources.
/// </summary>
/// <remarks>
///     Templates are loaded at runtime with <see cref="AvaloniaXamlLoader" />, which is not trim-safe.
///     In trimmed or NativeAOT applications, reference the templates directly instead.
/// </remarks>
public class DataTemplateInclude : IDataTemplate
{
    private const string TrimmingMessage =
        "DataTemplateInclude loads XAML at runtime with AvaloniaXamlLoader, which is not trim-safe.";

    private DataTemplates? _loaded;
    private bool _isLoading;

    /// <summary>
    ///     Gets or sets the source URI of the XAML file containing the data templates.
    /// </summary>
    public Uri Source { get; set; } = null!;

    /// <summary>
    ///     Gets the loaded data templates collection. Templates are loaded lazily when this property is first accessed.
    /// </summary>
    /// <returns>A collection of loaded data templates, or null if loading fails.</returns>
    public DataTemplates? Loaded
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "The constructor is annotated with RequiresUnreferencedCode.")]
        get
        {
            if (_loaded != null) return _loaded;

            _isLoading = true;
            _loaded = (DataTemplates)AvaloniaXamlLoader.Load(Source);
            _isLoading = false;

            return _loaded;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DataTemplateInclude" /> class.
    /// </summary>
    [RequiresUnreferencedCode(TrimmingMessage)]
    public DataTemplateInclude()
    {
    }

    /// <summary>
    ///     Determines whether this template can be used to display the specified data.
    /// </summary>
    /// <param name="data">The data object to test.</param>
    /// <returns>true if the template can be used for the specified data; otherwise, false.</returns>
    public bool Match(object? data)
    {
        if (_isLoading || Loaded == null) return false;

        return Loaded.Any(dt => dt.Match(data));
    }

    /// <summary>
    ///     Builds a control for the specified data object using the matching template.
    /// </summary>
    /// <param name="data">The data object to build a control for.</param>
    /// <returns>A new control instance if a matching template is found; otherwise, null.</returns>
    public Control? Build(object? data)
    {
        if (_isLoading || Loaded == null) return null;

        return Loaded.FirstOrDefault(dt => dt.Match(data))?.Build(data);
    }
}