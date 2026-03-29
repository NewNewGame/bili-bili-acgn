using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

public sealed class DynamicVarEntry : INotifyPropertyChanged
{
    private string _kind = "Damage";
    private decimal _baseValue;
    private decimal _upgradeValue;

    [JsonPropertyName("kind")]
    public string Kind
    {
        get => _kind;
        set
        {
            if (_kind == value) return;
            _kind = value;
            OnPropertyChanged();
        }
    }

    [JsonPropertyName("baseValue")]
    public decimal BaseValue
    {
        get => _baseValue;
        set
        {
            if (_baseValue == value) return;
            _baseValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FinalValue));
        }
    }

    /// <summary>升级增量，与 BaseValue 相加得到最终数值。</summary>
    [JsonPropertyName("upgradeValue")]
    public decimal UpgradeValue
    {
        get => _upgradeValue;
        set
        {
            if (_upgradeValue == value) return;
            _upgradeValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FinalValue));
        }
    }

    /// <summary>最终数值 = BaseValue + UpgradeValue（仅展示，不单独序列化）。</summary>
    [JsonIgnore]
    public decimal FinalValue => BaseValue + UpgradeValue;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
