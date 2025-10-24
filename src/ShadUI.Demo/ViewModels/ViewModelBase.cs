using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ShadUI.Demo.ViewModels;

public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo, IDisposable
{
    private readonly Dictionary<string, List<string>> _errors = new();
    private bool _disposed;

    public bool HasErrors => _errors.Count != 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return Array.Empty<string>();
        }

        return _errors.TryGetValue(propertyName, out var errors)
            ? errors
            : Array.Empty<string>();
    }

    protected void SetProperty<T>(ref T field, T value, bool validate = false,
        [CallerMemberName] string propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;

        field = value;
        OnPropertyChanged(propertyName);
        if (validate) ValidateProperty(value, propertyName);
    }

    protected void ValidateProperty<T>(T value, string propertyName)
    {
        ClearErrors(propertyName);

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, validationContext, validationResults)) return;

        foreach (var validationResult in validationResults)
        {
            AddError(propertyName, validationResult.ErrorMessage ?? string.Empty);
        }
    }

    protected void AddError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
        {
            _errors[propertyName] = new List<string>();
        }

        if (_errors[propertyName].Contains(error)) return;

        _errors[propertyName].Add(error);
        OnErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName)) OnErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
        var properties = _errors.Keys.ToList();
        _errors.Clear();
        foreach (var property in properties) OnErrorsChanged(property);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateAllProperties()
    {
        var properties = GetType().GetProperties()
            .Where(prop => prop.GetCustomAttributes(typeof(ValidationAttribute), true).Length != 0);

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            ValidateProperty(value, property.Name);
        }
    }

    /// <summary>
    ///     Disposes the ViewModel and cleans up resources. Override to add custom cleanup logic.
    /// </summary>
    public virtual void Dispose()
    {
        if (_disposed) return;

        // Clear errors and event handlers
        ClearAllErrors();
        ErrorsChanged = null;

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~ViewModelBase()
    {
        Dispose();
    }
}