using System.Collections.ObjectModel;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;
using MessageBox = System.Windows.MessageBox;

namespace CardEditorGui;

public partial class BuffOptionsWindow : Window
{
    private readonly ObservableCollection<BuffOptionEntry> _rows = [];

    public BuffOptionsWindow()
    {
        InitializeComponent();
        GridBuffs.ItemsSource = _rows;
        foreach (var b in BuffOptionsJson.LoadOrCreateDefault())
            _rows.Add(new BuffOptionEntry { Name = b.Name, Notes = b.Notes });
    }

    private void BtnAddBuff_Click(object sender, RoutedEventArgs e)
    {
        var name = TxtNewBuffName.Text.Trim();
        if (string.IsNullOrEmpty(name))
            return;
        if (_rows.Any(x => string.Equals(x.Name?.Trim(), name, StringComparison.Ordinal)))
        {
            MessageBox.Show("BUFF 列表中已有该项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var notes = TxtNewBuffNotes.Text.Trim();
        _rows.Add(new BuffOptionEntry { Name = name, Notes = string.IsNullOrEmpty(notes) ? null : notes });
        TxtNewBuffName.Clear();
        TxtNewBuffNotes.Clear();
        GridBuffs.SelectedItem = _rows[^1];
    }

    private void BtnRemoveBuff_Click(object sender, RoutedEventArgs e)
    {
        if (GridBuffs.SelectedItem is not BuffOptionEntry row)
        {
            MessageBox.Show("请先选中一行 BUFF。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _rows.Remove(row);
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var b in _rows)
        {
            var n = b.Name?.Trim() ?? "";
            if (n.Length == 0)
            {
                MessageBox.Show("BUFF「名称」不能为空。", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!names.Add(n))
            {
                MessageBox.Show($"重复的 BUFF 名称：{n}", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        BuffOptionsJson.SaveDefault(_rows.Select(b => new BuffOptionEntry { Name = b.Name, Notes = b.Notes }).ToList());
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

