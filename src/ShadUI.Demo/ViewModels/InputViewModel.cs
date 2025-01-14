﻿using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShadUI.Demo.ViewModels;

public sealed partial class InputViewModel : ViewModelBase
{
    public InputViewModel()
    {
        PropertyChanged += (_, _) => SubmitCommand.NotifyCanExecuteChanged();
        ErrorsChanged += (_, _) => SubmitCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private string _defaultInputCode = """
                                       <StackPanel>
                                           <TextBox Width="225" Watermark="Insert here..." />
                                       </StackPanel>
                                       """;

    [ObservableProperty]
    private string _disabledCode = """
                                    <StackPanel>
                                        <TextBox Width="225" IsEnabled="False" Watermark="Email" />
                                    </StackPanel>
                                    """;
    
    [ObservableProperty]
    private string _withLabelCode = """
                                    <StackPanel>
                                        <TextBox Classes="Clearable" Width="225" UseFloatingWatermark="True" Watermark="Email" />
                                    </StackPanel>
                                    """;

    [ObservableProperty]
    private string _withCustomLabelCode = """
                                    <StackPanel>
                                        <TextBox Classes="Clearable" Width="225" extensions:TextBoxExt.Label="Email"
                                                 Watermark="user@example.com" />
                                    </StackPanel>
                                    """;

    [ObservableProperty]
    private string _formValidationCode = """
                                          <Border Classes="Card Bordered" Padding="40" HorizontalAlignment="Center">
                                              <StackPanel Spacing="16" Width="275">
                                                  <TextBox Classes="Clearable"
                                                           extensions:TextBoxExt.Label="Email"
                                                           extensions:TextBoxExt.Hint="This is your public display name."
                                                           Watermark="user@example.com"
                                                           Text="{Binding Email, Mode=TwoWay}" />
                                                  <TextBox Classes="PasswordReveal"
                                                           extensions:TextBoxExt.Label="Password"
                                                           PasswordChar="•"
                                                           Watermark="Enter password"
                                                           Text="{Binding Password, Mode=TwoWay}" />
                                                  <TextBox Classes="PasswordReveal"
                                                           PasswordChar="•"
                                                           extensions:TextBoxExt.Label="Confirm"
                                                           Watermark="Confirm password"
                                                           Text="{Binding ConfirmPassword, Mode=TwoWay}" />
                                                  <Button Margin="0,20,0,0" Classes="Primary" Command="{Binding SubmitCommand}">Submit</Button>
                                              </StackPanel>
                                          </Border>
                                          """;
    
    private string _email = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailValidation]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value, true);
    }

    private string _password = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value, true);
    }

    private string _confirmPassword = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [IsMatchWith(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value, true);
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void Submit()
    {
        ClearAllErrors();
        ValidateAllProperties();

        if (HasErrors) return;

        Console.WriteLine("Success"); //TODO: Implement submit logic
    }

    private bool CanSubmit() => !HasErrors;

    public void Initialize()
    {
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;

        ClearAllErrors();
    }
}

public sealed class IsMatchWithAttribute(string matchProperty, string errorMessage = "Not match")
    : ValidationAttribute(errorMessage)
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var instance = validationContext.ObjectInstance;
        var otherValue = instance.GetType().GetProperty(matchProperty)?.GetValue(instance);

        return ((IComparable) value!).CompareTo(otherValue) == 0
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}

public sealed class EmailValidationAttribute : ValidationAttribute
{
    private static readonly string DefaultErrorMessage = "Please enter a valid email address";

    public EmailValidationAttribute() : base(DefaultErrorMessage)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email)
            return new ValidationResult(ErrorMessage);

        if (string.IsNullOrWhiteSpace(email))
            return new ValidationResult(ErrorMessage);

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return new ValidationResult(ErrorMessage);

        var dotIndex = email.IndexOf('.', atIndex);
        if (dotIndex == -1 || dotIndex == atIndex + 1 || dotIndex == email.Length - 1)
            return new ValidationResult(ErrorMessage);

        for (var i = 0; i < email.Length - 1; i++)
            if (email[i] == '.' && email[i + 1] == '.')
                return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}