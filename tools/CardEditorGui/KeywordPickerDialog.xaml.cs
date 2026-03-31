using System.Windows;
using System.Windows.Input;
using CardEditor.Shared.Models;
using MessageBox = System.Windows.MessageBox;

namespace CardEditorGui;

public partial class KeywordPickerDialog : Window
{
    public string? SelectedName { get; private set; }

    public KeywordPickerDialog(IEnumerable<KeywordOptionEntry> options, IReadOnlyCollection<string>? exclude = null)
    {
        InitializeComponent();
        var ex = exclude != null ? new HashSet<string>(exclude, StringComparer.Ordinal) : [];
        foreach (var o in options)
        {
            var n = o.Name?.Trim() ?? "";
            if (n.Length == 0 || IsKeywordExcluded(o, ex))
                continue;
            LstOptions.Items.Add(o);
        }
    }

    private static bool IsKeywordExcluded(KeywordOptionEntry o, HashSet<string> ex)
    {
        var q = o.QualifiedKey;
        var n = o.Name.Trim();
        if (ex.Contains(q)) return true;
        if (ex.Contains(n)) return true;
        if (ex.Contains(KeywordOptionEntry.FormatQualified(n, null))) return true;
        return false;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (LstOptions.Items.Count == 0)
        {
            MessageBox.Show("没有可选项：请先在主窗口「关键字配置」中添加关键字，或当前列表已全部加入。", "提示",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        if (LstOptions.SelectedItem is not KeywordOptionEntry k || string.IsNullOrWhiteSpace(k.Name))
        {
            MessageBox.Show("请先选中一项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        SelectedName = k.QualifiedKey;
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void LstOptions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (LstOptions.SelectedItem is KeywordOptionEntry)
            BtnOk_Click(sender, e);
    }
}
