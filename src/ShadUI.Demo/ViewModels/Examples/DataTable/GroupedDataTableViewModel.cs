using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ShadUI.Demo.ViewModels.Examples.DataTable;

public sealed partial class GroupedDataTableViewModel : ViewModelBase
{
    public GroupedDataTableViewModel()
    {
        var xamlPath = Path.Combine(AppContext.BaseDirectory, "views", "Examples", "DataTable",
            "GroupedDataTableContent.axaml");
        XamlCode = xamlPath.ExtractByLineRange(1, 58).CleanIndentation();

        var csharpPath = Path.Combine(AppContext.BaseDirectory, "viewModels", "Examples", "DataTable",
            "GroupedDataTableViewModel.cs");
		CSharpCode = csharpPath.ExtractByLineRange(1, 71).CleanIndentation();

        var people = new List<Person>
        {
            new() { FirstName = "Emily", LastName = "Johnson", Age = 28, State = "California", City = "Los Angeles" },
            new() { FirstName = "Daniel", LastName = "Martinez", Age = 34, State = "California", City = "San Diego" },
            new() { FirstName = "Sarah", LastName = "Kim", Age = 41, State = "California", City = "Burbank" },
            new() { FirstName = "Michael", LastName = "Lee", Age = 26, State = "California", City = "San Jose" },
            new() { FirstName = "Olivia", LastName = "Chen", Age = 30, State = "California", City = "Santa Barbara" },
            new() { FirstName = "Jacob", LastName = "Nguyen", Age = 39, State = "California", City = "Sacramento" },
            new() { FirstName = "Ethan", LastName = "Thompson", Age = 45, State = "Texas", City = "Austin" },
            new() { FirstName = "Ava", LastName = "Ramirez", Age = 27, State = "Texas", City = "Dallas" },
            new() { FirstName = "William", LastName = "Parker", Age = 38, State = "Texas", City = "Houston" },
            new() { FirstName = "Mia", LastName = "Flores", Age = 33, State = "Texas", City = "San Antonio" },
            new() { FirstName = "Benjamin", LastName = "Scott", Age = 24, State = "Texas", City = "El Paso" },
            new() { FirstName = "Sophia", LastName = "Jenkins", Age = 36, State = "Texas", City = "Fort Worth" },
            new() { FirstName = "Noah", LastName = "Brown", Age = 31, State = "New York", City = "New York" },
            new() { FirstName = "Isabella", LastName = "Rivera", Age = 29, State = "New York", City = "Albany" },
            new() { FirstName = "James", LastName = "Brooks", Age = 42, State = "New York", City = "Buffalo" },
            new() { FirstName = "Charlotte", LastName = "Davis", Age = 35, State = "New York", City = "Brentwood" },
            new() { FirstName = "Lucas", LastName = "Wilson", Age = 25, State = "New York", City = "Rochester" },
            new() { FirstName = "Amelia", LastName = "White", Age = 37, State = "New York", City = "Brooklyn" },
            new() { FirstName = "Henry", LastName = "Morgan", Age = 33, State = "Florida", City = "Miami" },
            new() { FirstName = "Harper", LastName = "Scott", Age = 40, State = "Florida", City = "Clearwater" },
            new() { FirstName = "Elijah", LastName = "Hall", Age = 23, State = "Florida", City = "Orlando" },
            new() { FirstName = "Abigail", LastName = "Sanders", Age = 29, State = "Florida", City = "Jacksonville" },
            new() { FirstName = "Logan", LastName = "Cox", Age = 38, State = "Florida", City = "Sarasota" },
            new() { FirstName = "Grace", LastName = "Reyes", Age = 32, State = "Florida", City = "Delray Beach" }
        };

        var view = new DataGridCollectionView(people);
        view.GroupDescriptions.Add(new DataGridPathGroupDescription("State"));
        GroupedPeople = view;
    }

    [ObservableProperty]
    private string _xamlCode = string.Empty;

    [ObservableProperty]
    private string _cSharpCode = string.Empty;

    [ObservableProperty]
    private DataGridCollectionView _groupedPeople;
}

public class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}