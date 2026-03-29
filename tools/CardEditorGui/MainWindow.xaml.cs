using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CardEditor.Shared;
using CardEditor.Shared.Models;
using Microsoft.Win32;

namespace CardEditorGui;

public partial class MainWindow : Window
{
    private string? _currentPath;
    private bool _dirty;
    private bool _suppressDirty;
    private readonly ObservableCollection<DynamicVarEntry> _dynamicVars = [];

    public MainWindow()
    {
        InitializeComponent();
        GridDynamicVars.ItemsSource = _dynamicVars;
        FillComboDefaults();
        NewDocument();
        HookFieldChanges();
        Closing += (_, e) =>
        {
            if (_dirty && !ConfirmDiscard())
                e.Cancel = true;
        };
    }

    private void FillComboDefaults()
    {
        CmbCardType.ItemsSource = new[] { "Attack", "Skill", "Power", "Status" };
        CmbRarity.ItemsSource = new[] { "Common", "Uncommon", "Rare", "Special" };
        CmbTargetType.ItemsSource = new[]
        {
            "AnyEnemy", "Self", "AllEnemies", "None", "Player"
        };
    }

    private void NewDocument()
    {
        _currentPath = null;
        ApplyModelToUi(CreateDefaultCard());
        _dirty = false;
        UpdateTitle();
        StatusText.Text = "新建文档";
    }

    private static CardDefinition CreateDefaultCard() => new()
    {
        ClassName = "NewCard",
        Namespace = "BiliBiliACGN.BiliBiliACGNCode.Cards",
        EnergyCost = 1,
        CardType = "Attack",
        Rarity = "Common",
        TargetType = "AnyEnemy",
        ShowInCardLibrary = true,
        PoolTypeName = "ColorlessCardPool",
        DynamicVars =
        [
            new DynamicVarEntry { Kind = "Damage", BaseValue = 6m, ValueProp = "Move" }
        ],
        Notes = ""
    };

    private void ApplyModelToUi(CardDefinition m)
    {
        _suppressDirty = true;
        try
        {
            TxtClassName.Text = m.ClassName;
            TxtNamespace.Text = m.Namespace;
            TxtEnergyCost.Text = m.EnergyCost.ToString();
            TxtPoolTypeName.Text = m.PoolTypeName;
            SelectCombo(CmbCardType, m.CardType);
            SelectCombo(CmbRarity, m.Rarity);
            SelectCombo(CmbTargetType, m.TargetType);
            ChkShowInLibrary.IsChecked = m.ShowInCardLibrary;
            TxtNotes.Text = m.Notes ?? "";

            _dynamicVars.Clear();
            foreach (var v in m.DynamicVars)
                _dynamicVars.Add(CloneVar(v));
            if (_dynamicVars.Count == 0)
                _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, ValueProp = "Move" });
        }
        finally
        {
            _suppressDirty = false;
        }
    }

    private static DynamicVarEntry CloneVar(DynamicVarEntry v) => new()
    {
        Kind = v.Kind,
        BaseValue = v.BaseValue,
        ValueProp = v.ValueProp,
        CustomKey = v.CustomKey
    };

    private static void SelectCombo(System.Windows.Controls.ComboBox box, string value)
    {
        for (var i = 0; i < box.Items.Count; i++)
        {
            if (box.Items[i]?.ToString() == value)
            {
                box.SelectedIndex = i;
                return;
            }
        }
        box.SelectedIndex = box.Items.Count > 0 ? 0 : -1;
    }

    private CardDefinition CollectModelFromUi()
    {
        if (!int.TryParse(TxtEnergyCost.Text.Trim(), out var energy))
            energy = 0;

        return new CardDefinition
        {
            SchemaVersion = 1,
            ClassName = TxtClassName.Text.Trim(),
            Namespace = TxtNamespace.Text.Trim(),
            EnergyCost = energy,
            CardType = CmbCardType.SelectedItem?.ToString() ?? "Attack",
            Rarity = CmbRarity.SelectedItem?.ToString() ?? "Common",
            TargetType = CmbTargetType.SelectedItem?.ToString() ?? "AnyEnemy",
            ShowInCardLibrary = ChkShowInLibrary.IsChecked == true,
            PoolTypeName = TxtPoolTypeName.Text.Trim(),
            DynamicVars = _dynamicVars.Select(CloneVar).ToList(),
            Notes = string.IsNullOrWhiteSpace(TxtNotes.Text) ? null : TxtNotes.Text
        };
    }

    private void MarkDirty()
    {
        if (_suppressDirty)
            return;
        _dirty = true;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var name = string.IsNullOrEmpty(_currentPath)
            ? "未保存"
            : Path.GetFileName(_currentPath);
        Title = _dirty ? $"卡牌定义编辑器 — {name} *" : $"卡牌定义编辑器 — {name}";
    }

    private bool ConfirmDiscard()
    {
        if (!_dirty)
            return true;
        var r = MessageBox.Show("当前内容未保存，是否放弃更改？", "确认",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        return r == MessageBoxResult.Yes;
    }

    private void MenuNew_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard())
            return;
        NewDocument();
    }

    private void MenuOpen_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard())
            return;
        var dlg = new OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "打开卡牌定义"
        };
        if (dlg.ShowDialog() != true)
            return;
        try
        {
            var model = CardDefinitionJson.LoadFromFile(dlg.FileName);
            _currentPath = dlg.FileName;
            ApplyModelToUi(model);
            _dirty = false;
            UpdateTitle();
            StatusText.Text = $"已打开: {_currentPath}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "打开失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentPath))
        {
            MenuSaveAs_Click(sender, e);
            return;
        }
        SaveToPath(_currentPath);
    }

    private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "保存卡牌定义",
            FileName = string.IsNullOrWhiteSpace(TxtClassName.Text) ? "card.json" : $"{TxtClassName.Text.Trim()}.json"
        };
        if (dlg.ShowDialog() != true)
            return;
        _currentPath = dlg.FileName;
        SaveToPath(_currentPath);
    }

    private void SaveToPath(string path)
    {
        try
        {
            var model = CollectModelFromUi();
            CardDefinitionJson.SaveToFile(model, path);
            _dirty = false;
            UpdateTitle();
            StatusText.Text = $"已保存: {path}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

    private void BtnAddVar_Click(object sender, RoutedEventArgs e)
    {
        _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, ValueProp = "Move" });
        MarkDirty();
    }

    private void BtnRemoveVar_Click(object sender, RoutedEventArgs e)
    {
        if (GridDynamicVars.SelectedItem is not DynamicVarEntry row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _dynamicVars.Remove(row);
        if (_dynamicVars.Count == 0)
            _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, ValueProp = "Move" });
        MarkDirty();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuSave_Click(this, e);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuNew_Click(this, e);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuOpen_Click(this, e);
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    private void HookFieldChanges()
    {
        TxtClassName.TextChanged += (_, _) => MarkDirty();
        TxtNamespace.TextChanged += (_, _) => MarkDirty();
        TxtEnergyCost.TextChanged += (_, _) => MarkDirty();
        TxtPoolTypeName.TextChanged += (_, _) => MarkDirty();
        TxtNotes.TextChanged += (_, _) => MarkDirty();
        CmbCardType.SelectionChanged += (_, _) => MarkDirty();
        CmbRarity.SelectionChanged += (_, _) => MarkDirty();
        CmbTargetType.SelectionChanged += (_, _) => MarkDirty();
        ChkShowInLibrary.Checked += (_, _) => MarkDirty();
        ChkShowInLibrary.Unchecked += (_, _) => MarkDirty();
        GridDynamicVars.CellEditEnding += (_, _) => MarkDirty();
    }
}
