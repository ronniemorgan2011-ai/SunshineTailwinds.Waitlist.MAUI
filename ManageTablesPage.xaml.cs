using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI;

public partial class ManageTablesPage : ContentPage
{
    private DatabaseService _database;

    public ManageTablesPage()
    {
        InitializeComponent();

        _database = new DatabaseService();

        LoadTables();
    }

    private void LoadTables()
    {
        List<TableInfo> tables;

        if (chkShowDisabled.IsChecked)
        {
            tables = _database.GetAllTables();
        }
        else
        {
            tables = _database.GetTables();
        }

        lblTableCount.Text =
            $"Current Tables ({tables.Count})";

        TablePicker.ItemsSource = tables;

        lblActiveTables.Text =
            string.Join(
                "   ",
                tables.Select(t =>
                    t.IsActive
                        ? t.Name
                        : $"{t.Name} (Disabled)"));
    }

    private void ShowDisabled_Changed(
        object sender,
        CheckedChangedEventArgs e)
    {
        LoadTables();
    }

    private async void RenameTable_Clicked(
        object sender,
        EventArgs e)
    {
        if (TablePicker.SelectedItem is not TableInfo table)
            return;

        string newName =
            txtRenameTable.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(newName))
            return;

        bool exists =
            _database.GetAllTables()
            .Any(t =>
                t.Id != table.Id &&
                t.Name.Equals(
                    newName,
                    StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            await DisplayAlert(
                "Rename Table",
                "A table with that name already exists.",
                "OK");

            return;
        }

        _database.RenameTable(
            table.Id,
            newName);

        txtRenameTable.Text = "";

        LoadTables();
    }

    private async void AddTable_Clicked(
        object sender,
        EventArgs e)
    {
        var tableName =
            txtTableName.Text?.Trim();

        if (string.IsNullOrWhiteSpace(tableName))
            return;

        bool exists =
            _database.GetAllTables()
            .Any(t =>
                t.Name.Equals(
                    tableName,
                    StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            await DisplayAlert(
                "Duplicate Table",
                "Table already exists.",
                "OK");

            return;
        }

        _database.AddTable(
            new TableInfo
            {
                Name = tableName,
                IsActive = true
            });

        txtTableName.Text = "";

        LoadTables();
    }

    private void DisableTable_Clicked(
        object sender,
        EventArgs e)
    {
        if (TablePicker.SelectedItem is not TableInfo table)
            return;

        _database.DisableTable(table.Id);

        LoadTables();
    }

    private void EnableTable_Clicked(
        object sender,
        EventArgs e)
    {
        if (TablePicker.SelectedItem is not TableInfo table)
            return;

        _database.EnableTable(table.Id);

        LoadTables();
    }
}