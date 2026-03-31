using System.Collections.ObjectModel;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;
using MessageBox = System.Windows.MessageBox;

namespace CardEditorGui;

public partial class CardKeywordOptionsWindow : Window
{
    private readonly ObservableCollection<KeywordOptionEntry> _rows = [];

    public CardKeywordOptionsWindow()
    {
        InitializeComponent();
        GridKeywords.ItemsSource = _rows;
        foreach (var k in CardKeywordOptionsJson.LoadOrCreateDefault())
            _rows.Add(new KeywordOptionEntry { Name = k.Name, Notes = k.Notes, MemberPrefix = k.MemberPrefix });
    }

    private void BtnAddKeyword_Click(object sender, RoutedEventArgs e)
    {
        var name = TxtNewKeywordName.Text.Trim();
        if (string.IsNullOrEmpty(name))
            return;
        var prefix = TxtNewMemberPrefix.Text.Trim();
        var entry = new KeywordOptionEntry
        {
            Name = name,
            MemberPrefix = string.IsNullOrEmpty(prefix) ? null : prefix,
            Notes = string.IsNullOrEmpty(TxtNewKeywordNotes.Text.Trim()) ? null : TxtNewKeywordNotes.Text.Trim()
        };
        if (_rows.Any(x => string.Equals(x.QualifiedKey, entry.QualifiedKey, StringComparison.Ordinal)))
        {
            MessageBox.Show("列表中已有相同限定名（前缀+name）的项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _rows.Add(entry);
        TxtNewKeywordName.Clear();
        TxtNewMemberPrefix.Clear();
        TxtNewKeywordNotes.Clear();
        GridKeywords.SelectedItem = _rows[^1];
    }

    private void BtnRemoveKeyword_Click(object sender, RoutedEventArgs e)
    {
        if (GridKeywords.SelectedItem is not KeywordOptionEntry row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _rows.Remove(row);
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var k in _rows)
        {
            var n = k.Name?.Trim() ?? "";
            if (n.Length == 0)
            {
                MessageBox.Show("「name」不能为空。", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var q = k.QualifiedKey;
            if (!keys.Add(q))
            {
                MessageBox.Show($"重复的限定名：{q}", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        CardKeywordOptionsJson.SaveDefault(_rows.Select(x => new KeywordOptionEntry
        {
            Name = x.Name,
            MemberPrefix = x.MemberPrefix,
            Notes = x.Notes
        }).ToList());
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
