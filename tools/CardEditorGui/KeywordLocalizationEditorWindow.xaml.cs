using System.Windows;
using System.Windows.Input;
using CardEditor.Shared;
using CardEditor.Shared.Models;
using Button = System.Windows.Controls.Button;
using WpfTextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.MessageBox;

namespace CardEditorGui;

public partial class KeywordLocalizationEditorWindow : Window
{
    private KeywordEditorSettings _settings = new();
    private WpfTextBox? _activeBbTextBox;
    private int? _savedSelStart;
    private int? _savedSelLen;

    public KeywordLocalizationEditorWindow()
    {
        InitializeComponent();
        _settings = KeywordEditorSettingsJson.LoadOrCreateDefault();
        _activeBbTextBox = TxtDescription;
        TxtDescription.TextChanged += (_, _) => RefreshPreview();
        RefreshPreview();
        TxtDescription.PreviewKeyDown += BbTextBox_PreviewKeyDown;
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        var w = new KeywordEditorSettingsWindow { Owner = this };
        if (w.ShowDialog() == true)
            _settings = KeywordEditorSettingsJson.LoadOrCreateDefault();
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e) => SearchAndLoad();

    private void SearchAndLoad()
    {
        _settings = KeywordEditorSettingsJson.LoadOrCreateDefault();
        var path = (_settings.KeywordLocalizationJsonPath ?? "").Trim();
        var prefix = (_settings.KeywordLocalizationNamespacePrefix ?? "").Trim();
        var keyword = (TxtKeyword.Text ?? "").Trim();

        if (keyword.Length == 0)
        {
            MessageBox.Show("请先输入 keyword（如 YYSY）。", "搜索/读取", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            StatusText.Text = "未配置 card_keywords.json 路径（请点上方「设置…」配置）";
            return;
        }

        var (title, desc) = KeywordLocalizationJsonMerger.TryReadKeywordTexts(path, prefix, keyword);
        if (title != null)
            TxtTitle.Text = title;
        if (desc != null)
            TxtDescription.Text = desc;
        RefreshPreview();

        if (title == null && desc == null)
            StatusText.Text = $"未找到条目：{keyword}（{System.IO.Path.GetFullPath(path)}）";
        else
            StatusText.Text = $"已加载：{System.IO.Path.GetFullPath(path)}";
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        _settings = KeywordEditorSettingsJson.LoadOrCreateDefault();
        var path = (_settings.KeywordLocalizationJsonPath ?? "").Trim();
        var prefix = (_settings.KeywordLocalizationNamespacePrefix ?? "").Trim();
        var keyword = (TxtKeyword.Text ?? "").Trim();

        if (keyword.Length == 0)
        {
            MessageBox.Show("请先输入 keyword（如 YYSY）。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            MessageBox.Show("请先点上方「设置…」配置 card_keywords.json 路径。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(prefix))
        {
            MessageBox.Show("请先点上方「设置…」配置 Key 前缀（命名空间）。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            KeywordLocalizationJsonMerger.MergeKeywordTexts(path, prefix, keyword, TxtTitle.Text.Trim(), TxtDescription.Text);
            StatusText.Text = $"已保存：{System.IO.Path.GetFullPath(path)}";
            MessageBox.Show("已更新 card_keywords.json。", "保存", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void RefreshPreview() =>
        RtbDescriptionPreview.Document = BbCodeFlowDocument.Parse(TxtDescription.Text);

    private void BbTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is WpfTextBox tb)
            _activeBbTextBox = tb;
    }

    private void BbToolbar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            SaveSelectionSnapshot();
    }

    private void SaveSelectionSnapshot()
    {
        var tb = _activeBbTextBox ?? TxtDescription;
        _savedSelStart = tb.SelectionStart;
        _savedSelLen = tb.SelectionLength;
    }

    private void ClearSelectionSnapshot()
    {
        _savedSelStart = null;
        _savedSelLen = null;
    }

    private void GetSelection(out int start, out int len)
    {
        if (_savedSelStart.HasValue && _savedSelLen.HasValue)
        {
            start = _savedSelStart.Value;
            len = _savedSelLen.Value;
            ClearSelectionSnapshot();
            return;
        }
        var tb = _activeBbTextBox ?? TxtDescription;
        start = tb.SelectionStart;
        len = tb.SelectionLength;
    }

    private void BtnColorMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;
        SaveSelectionSnapshot();
        if (sender is not Button btn || btn.ContextMenu == null)
            return;
        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        btn.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void BtnAnimationMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;
        SaveSelectionSnapshot();
        if (sender is not Button btn || btn.ContextMenu == null)
            return;
        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        btn.ContextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void BbMenu_Closed(object sender, RoutedEventArgs e) => ClearSelectionSnapshot();

    private void BbWrapTag_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement el || el.Tag is not string tag)
            return;
        var parts = tag.Split('|');
        if (parts.Length != 4)
            return;
        WrapPair(parts[0], parts[1], parts[2], parts[3]);
    }

    private void BbInsertColorRed_Click(object sender, RoutedEventArgs e) =>
        WrapExclusive("[color=red]", "[/color]", "color");

    private void BbExclusiveWrapTag_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement el || el.Tag is not string tag)
            return;
        var parts = tag.Split('|', 3);
        if (parts.Length != 3)
            return;
        WrapExclusive(parts[0], parts[1], parts[2]);
    }

    private void WrapPair(string tagNameForExpand, string tagNameForToggle, string openLiteral, string closeLiteral)
    {
        var tb = _activeBbTextBox ?? TxtDescription;
        var text = tb.Text ?? "";
        GetSelection(out var start, out var len);
        if (start > text.Length)
            start = text.Length;

        DescriptionTagPairUtilities.TryExpandSelectionInsideTagPair(text, ref start, ref len, tagNameForExpand);
        if (start < 0 || len < 0 || start + len > text.Length)
            return;

        var segment = text.Substring(start, len);
        if (DescriptionTagPairUtilities.TryParseExactSingleWrapper(segment, tagNameForToggle, out var inner))
        {
            var newText = string.Concat(text.AsSpan(0, start), inner, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start, inner.Length);
        }
        else
        {
            var core = segment;
            var wrapped = openLiteral + core + closeLiteral;
            var newText = string.Concat(text.AsSpan(0, start), wrapped, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start + openLiteral.Length, core.Length);
        }
        RefreshPreview();
    }

    private void WrapExclusive(string openTag, string closeTag, string logicalName)
    {
        var tb = _activeBbTextBox ?? TxtDescription;
        var text = tb.Text ?? "";
        GetSelection(out var start, out var len);
        if (start > text.Length)
            start = text.Length;

        DescriptionExclusiveBbCode.TryExpandSelectionToInnermostExclusiveWrapper(text, ref start, ref len);
        if (start < 0 || len < 0 || start + len > text.Length)
            return;

        var segment = text.Substring(start, len);
        if (IsSameExclusiveLayer(segment, logicalName, openTag, out var innerCore))
        {
            var newText = string.Concat(text.AsSpan(0, start), innerCore, text.AsSpan(start + len));
            tb.Text = newText;
            tb.Focus();
            tb.Select(start, innerCore.Length);
            RefreshPreview();
            return;
        }

        var core = DescriptionExclusiveBbCode.StripExclusiveWrappers(segment);
        var wrapped = openTag + core + closeTag;
        var newText2 = string.Concat(text.AsSpan(0, start), wrapped, text.AsSpan(start + len));
        tb.Text = newText2;
        tb.Focus();
        tb.Select(start + openTag.Length, core.Length);
        RefreshPreview();
    }

    private static bool IsSameExclusiveLayer(string segment, string logicalName, string openTag, out string innerCore)
    {
        innerCore = "";
        if (logicalName.Equals("color", StringComparison.OrdinalIgnoreCase) &&
            segment.StartsWith("[color=", StringComparison.OrdinalIgnoreCase) &&
            openTag.StartsWith("[color=", StringComparison.OrdinalIgnoreCase))
        {
            var valOpen = ExtractColorValue(openTag);
            var valSeg = ExtractColorValueFromSegment(segment);
            if (valOpen.Equals(valSeg, StringComparison.OrdinalIgnoreCase) &&
                TryParseColorBody(segment, out innerCore))
                return true;
            return false;
        }
        var simpleOpen = $"[{logicalName}]";
        var simpleClose = $"[/{logicalName}]";
        if (segment.StartsWith(simpleOpen, StringComparison.OrdinalIgnoreCase) &&
            segment.EndsWith(simpleClose, StringComparison.OrdinalIgnoreCase) &&
            openTag.Equals(simpleOpen, StringComparison.OrdinalIgnoreCase))
        {
            innerCore = segment.Substring(simpleOpen.Length, segment.Length - simpleOpen.Length - simpleClose.Length);
            return true;
        }
        return false;
    }

    private static string ExtractColorValue(string openTag)
    {
        var eq = openTag.IndexOf('=');
        var end = openTag.IndexOf(']');
        if (eq < 0 || end < 0)
            return "";
        return openTag.Substring(eq + 1, end - eq - 1).Trim();
    }

    private static string ExtractColorValueFromSegment(string segment)
    {
        var eq = segment.IndexOf('=');
        var rb = segment.IndexOf(']');
        if (eq < 0 || rb < 0)
            return "";
        return segment.Substring(eq + 1, rb - eq - 1).Trim();
    }

    private static bool TryParseColorBody(string segment, out string inner)
    {
        inner = "";
        var close = "[/color]";
        var last = segment.LastIndexOf(close, StringComparison.OrdinalIgnoreCase);
        if (last < 0)
            return false;
        var first = segment.IndexOf(']');
        if (first < 0 || last <= first)
            return false;
        inner = segment.Substring(first + 1, last - first - 1);
        return true;
    }

    private void BbTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.B)
        {
            WrapPair("b", "b", "[b]", "[/b]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.I)
        {
            WrapPair("i", "i", "[i]", "[/i]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.U)
        {
            WrapPair("u", "u", "[u]", "[/u]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.C)
        {
            WrapExclusive("[color=red]", "[/color]", "color");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.F)
        {
            WrapPair("font", "font", "[font=Arial]", "[/font]");
            e.Handled = true;
            return;
        }
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z)
        {
            WrapPair("size", "size", "[size=24]", "[/size]");
            e.Handled = true;
            return;
        }
    }
}

