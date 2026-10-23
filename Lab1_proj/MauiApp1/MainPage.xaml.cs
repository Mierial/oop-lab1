using Antlr4.Runtime;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private const int Size = 5; 
        private readonly Dictionary<string, Cell> _cells = new();
        private readonly Dictionary<string, Entry> _entries = new();
        private bool _showFormulas = false;

        public MainPage()
        {
            InitializeComponent();
            BuildGrid();
            ModeBtn.Clicked += (s, e) => ToggleMode();
        }

        private void BuildGrid()
        {
            SpreadsheetGrid.RowDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            for (int c = 0; c < Size; c++)
            {
                SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                var header = new Label
                {
                    Text = ((char)('A' + c)).ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold
                };
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, c + 1);
                SpreadsheetGrid.Children.Add(header);
            }

            SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int r = 0; r < Size; r++)
            {
                SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var rowHeader = new Label
                {
                    Text = (r + 1).ToString(),
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetRow(rowHeader, r + 1);
                Grid.SetColumn(rowHeader, 0);
                SpreadsheetGrid.Children.Add(rowHeader);

                for (int c = 0; c < Size; c++)
                {
                    string name = $"{((char)('A' + c))}{(r + 1)}";
                    var cell = new Cell { Name = name };
                    _cells[name] = cell;

                    var entry = new Entry
                    {
                        Text = "0",
                        HorizontalTextAlignment = TextAlignment.Center
                    };

                    entry.Unfocused += (s, e) =>
                    {
                        cell.Expression = entry.Text ?? "";
                        Calculate();
                        UpdateGridUI();
                    };

                    _entries[name] = entry;
                    Grid.SetRow(entry, r + 1);
                    Grid.SetColumn(entry, c + 1);
                    SpreadsheetGrid.Children.Add(entry);
                }
            }
        }

        private void Calculate()
        {
            foreach (var cell in _cells.Values)
            {
                if (string.IsNullOrWhiteSpace(cell.Expression))
                {
                    cell.Value = 0;
                    cell.ValueText = "0";
                    continue;
                }

                try
                {
                    string expr = cell.Expression.StartsWith("=") ? cell.Expression[1..] : cell.Expression;
                    var lexer = new LabCalculatorLexer(new AntlrInputStream(expr));
                    var parser = new LabCalculatorParser(new CommonTokenStream(lexer));
                    var visitor = new CalculatorVisitor(_cells);

                    cell.Value = visitor.Visit(parser.compileUnit());
                    cell.ValueText = cell.Value.ToString();
                }
                catch
                {
                    cell.ValueText = "#ERROR!";
                }
            }
        }

        private void UpdateGridUI()
        {
            foreach (var (name, entry) in _entries)
            {
                if (_cells.TryGetValue(name, out var cell))
                {
                    entry.Text = _showFormulas ? cell.Expression : cell.ValueText;
                }
            }
        }

        private void ToggleMode()
        {
            _showFormulas = !_showFormulas;
            ModeBtn.Text = _showFormulas ? "Режим: Формули" : "Режим: Значення";
            UpdateGridUI();
        }
    }
}