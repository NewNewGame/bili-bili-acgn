using System.IO;
using System.Windows;
using System.Windows.Input;
using CardEditor.Shared;
using CardEditor.Shared.Models;
using Button = System.Windows.Controls.Button;
using WpfTextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.MessageBox;

namespace CardEditorGui;

public partial class PowerLocalizationEditorWindow : Window
{
    private PowerEditorSettings _settings = new();
    private string? _currentPowerClassName;
    private WpfTextBox? _activeBbTextBox;
    private int? _savedSelStart;
    private int? _savedSelLen;

    public PowerLocalizationEditorWindow()
    {
        InitializeComponent();
        _settings = PowerEditorSettingsJson.LoadOrCreateDefault();
        TxtPrefix.Text = (_settings.PowerLocalizationNamespacePrefix ?? "").Trim();
        _activeBbTextBox = TxtDescription;

        TxtTitle.TextChanged += (_, _) => StatusText.Text = "已修改（未保存）";
        TxtDescription.TextChanged += (_, _) => RefreshPreviews();
        TxtSmartDescription.TextChanged += (_, _) => RefreshPreviews();

        TxtDescription.PreviewKeyDown += BbTextBox_PreviewKeyDown;
        TxtSmartDescription.PreviewKeyDown += BbTextBox_PreviewKeyDown;
        RefreshPreviews();
    }

    private void BtnOpenPowerCs_Click(object sender, RoutedEventArgs e)
    {
        _settings = PowerEditorSettingsJson.LoadOrCreateDefault();
        TxtPrefix.Text = (_settings.PowerLocalizationNamespacePrefix ?? "").Trim();

        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "C# (*.cs)|*.cs|所有文件 (*.*)|*.*",
            Title = "选择 Power C# 文件（如 YYSYPower.cs）",
            InitialDirectory = GuessPowersDirectory()
        };
        if (dlg.ShowDialog() != true)
            return;

        var className = Path.GetFileNameWithoutExtension(dlg.FileName)?.Trim();
        if (string.IsNullOrWhiteSpace(className))
            return;

        _currentPowerClassName = className;
        TxtCurrentPower.Text = TrimPowerSuffixForUi(className);
        LoadFromPowersJson();
    }

    private string GuessPowersDirectory()
    {
        try
        {
            var configured = (_settings.DefaultPowersCsDirectory ?? "").Trim();
            if (!string.IsNullOrEmpty(configured) && Directory.Exists(configured))
                return configured;

            var baseDir = Directory.GetCurrentDirectory();
            var p = Path.Combine(baseDir, "BiliBiliACGNCode", "Powers");
            return Directory.Exists(p) ? p : baseDir;
        }
        catch
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }

    private static string TrimPowerSuffixForUi(string className) =>
        className.EndsWith("Power", StringComparison.Ordinal) ? className[..^"Power".Length] : className;

    private void LoadFromPowersJson()
    {
        var path = (_settings.PowerLocalizationJsonPath ?? "").Trim();
        var prefix = (_settings.PowerLocalizationNamespacePrefix ?? "").Trim();

        if (string.IsNullOrEmpty(_currentPowerClassName))
            return;

        if (string.IsNullOrEmpty(path))
        {
            StatusText.Text = "未配置 powers.json 路径（请点上方「设置…」配置）";
            return;
        }

        var (title, desc, smart) = PowerLocalizationJsonMerger.TryReadPowerTexts(path, prefix, _currentPowerClassName);
        if (title != null)
            TxtTitle.Text = title;
        if (desc != null)
            TxtDescription.Text = desc;
        if (smart != null)
            TxtSmartDescription.Text = smart;
        RefreshPreviews();
        if (title == null && desc == null && smart == null)
            StatusText.Text = $"未找到条目：{_currentPowerClassName}（{Path.GetFullPath(path)}）";
        else
            StatusText.Text = $"已加载：{Path.GetFullPath(path)}";
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        _settings = PowerEditorSettingsJson.LoadOrCreateDefault();
        var path = (_settings.PowerLocalizationJsonPath ?? "").Trim();
        var prefix = (_settings.PowerLocalizationNamespacePrefix ?? "").Trim();

        if (string.IsNullOrEmpty(_currentPowerClassName))
        {
            MessageBox.Show("请先打开一个 Power.cs。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            MessageBox.Show("请先点上方「设置…」配置 powers.json 路径。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(prefix))
        {
            MessageBox.Show("请先点上方「设置…」配置能力键前缀（命名空间）。", "保存", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            PowerLocalizationJsonMerger.MergePowerTexts(
                path,
                prefix,
                _currentPowerClassName,
                TxtTitle.Text.Trim(),
                TxtDescription.Text,
                TxtSmartDescription.Text);
            StatusText.Text = $"已保存：{Path.GetFullPath(path)}";
            MessageBox.Show("已更新 powers.json。", "保存", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        var w = new PowerEditorSettingsWindow { Owner = this };
        if (w.ShowDialog() == true)
        {
            _settings = PowerEditorSettingsJson.LoadOrCreateDefault();
            TxtPrefix.Text = (_settings.PowerLocalizationNamespacePrefix ?? "").Trim();
            if (!string.IsNullOrEmpty(_currentPowerClassName))
                LoadFromPowersJson();
        }
    }

    private void RefreshPreviews()
    {
        RtbDescriptionPreview.Document = BbCodeFlowDocument.Parse(TxtDescription.Text);
        RtbSmartPreview.Document = BbCodeFlowDocument.Parse(TxtSmartDescription.Text);
    }

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
        RefreshPreviews();
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
            RefreshPreviews();
            return;
        }

        var core = DescriptionExclusiveBbCode.StripExclusiveWrappers(segment);
        var wrapped = openTag + core + closeTag;
        var newText2 = string.Concat(text.AsSpan(0, start), wrapped, text.AsSpan(start + len));
        tb.Text = newText2;
        tb.Focus();
        tb.Select(start + openTag.Length, core.Length);
        RefreshPreviews();
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

