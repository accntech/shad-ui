using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace ShadUI.Controls;

/// <summary>
/// A vertical stacked set of interactive headings that each reveal a section of content.
/// </summary>
public class Accordion : SelectingItemsControl
{
    /// <summary>
    /// Defines the <see cref="InsertSeparators"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> InsertSeparatorsProperty =
        AvaloniaProperty.Register<Accordion, bool>(nameof(InsertSeparators), true);
    
    /// <summary>
    /// Gets or sets a value indicating whether to add separators between AccordionItem elements.
    /// </summary>
    public bool InsertSeparators
    {
        get => GetValue(InsertSeparatorsProperty);
        set => SetValue(InsertSeparatorsProperty, value);
    }
    
    /// <summary>
    /// Defines the <see cref="SeparatorsSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> SeparatorsSpacingProperty =
        AvaloniaProperty.Register<Accordion, double>(nameof(SeparatorsSpacing), 5);
    
    /// <summary>
    /// Gets or sets the size of the spacing to place between separators controls.
    /// </summary>
    public double SeparatorsSpacing
    {
        get => GetValue(SeparatorsSpacingProperty);
        set => SetValue(SeparatorsSpacingProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <remarks>
    /// Note that the selection mode only applies to selections made via user interaction.
    /// Multiple selections can be made programmatically regardless of the value of this property.
    /// </remarks>
    public new SelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }
    
    /// <summary>
    ///     Returns a new instance of the <see cref="Accordion" /> class.
    /// </summary>
    public Accordion()
    {
        SelectionMode = SelectionMode.Single | SelectionMode.Toggle;
    }
    
    private void UpdateSeparators()
    {
        if (InsertSeparators)
        {
            List<int> indexes = [];

            for (var index = 0; index < Items.Count; index++)
            {
                object? control = Items[index];
                if (index + 1 == Items.Count)
                    break;
            
                object? nextControl = Items[index + 1];

                if (control?.GetType() == typeof(AccordionItem) && nextControl?.GetType() != typeof(Separator))
                {
                    indexes.Add(index + indexes.Count + 1);
                }
                else if(control?.GetType() == typeof(AccordionItem) && nextControl is Separator separator)
                {
                    var margin = separator.Margin;
                    separator.Margin = new Thickness(
                        margin.Left,
                        margin.Top + SeparatorsSpacing,
                        margin.Right,
                        margin.Bottom + SeparatorsSpacing);
                }
            }

            foreach (int index in indexes)
            {
                Items.Insert(index, new Separator { Margin = new Thickness(0, SeparatorsSpacing) });
            }
        }
        else
        {
            List<int> indexes = [];

            for (var index = 0; index < Items.Count; index++)
            {
                object? control = Items[index];
                if (index + 1 == Items.Count)
                    break;
            
                object? nextControl = Items[index + 1];

                if (control?.GetType() == typeof(AccordionItem) && nextControl?.GetType() == typeof(Separator))
                {
                    indexes.Add(index - indexes.Count + 1);
                }
            }

            foreach (int index in indexes)
            {
                Items.RemoveAt(index);
            }
        }
    }
    
    /// <summary>
    ///     Called when the control is loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        UpdateSeparators();
    }
    
    /// <summary>
    ///     Called when a property changes.
    /// </summary>
    /// <param name="change"></param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if(change.Property == InsertSeparatorsProperty)
            UpdateSeparators();
    }

    /// <summary>
    ///     Returns if the container needs.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <param name="recycleKey"></param>
    /// <returns></returns>
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<AccordionItem>(item, out recycleKey);
}