using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.WeaponStore.Info
{
    public sealed class StoreInfoStatRowView : MonoBehaviour
    {
        [SerializeField] private Color _currentTextColor = new(1f, 0.45f, 0.05f, 1f);
        [SerializeField] private Color _upgradeTextColor = new(0.23f, 0.82f, 0.24f, 1f);
        [SerializeField] private Color _currentFillColor = new(1f, 0.45f, 0.05f, 1f);
        [SerializeField] private Color _upgradeFillColor = new(0.23f, 0.82f, 0.24f, 1f);
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _currentValueText;
        [SerializeField] private Text _upgradeDeltaText;
        [SerializeField] private Image _currentFill;
        [SerializeField] private Image _upgradeFill;

        public void Render(StoreStatSnapshot stat)
        {
            if (_nameText)
                _nameText.text = stat.Name;

            if (_currentValueText)
            {
                _currentValueText.color = _currentTextColor;
                _currentValueText.text = FormatValue(stat.CurrentValue);
            }

            if (_upgradeDeltaText)
            {
                _upgradeDeltaText.color = _upgradeTextColor;
                _upgradeDeltaText.text = stat.HasUpgradeDelta ? "+" + FormatValue(stat.UpgradeDelta) : string.Empty;
            }

            if (_upgradeFill)
                RenderFill(_upgradeFill, _upgradeFillColor, stat.UpgradedValue / stat.MaxValue);

            if (_currentFill)
                RenderFill(_currentFill, _currentFillColor, stat.CurrentValue / stat.MaxValue);
        }

        private void RenderFill(Image fill, Color color, float normalizedValue)
        {
            var amount = Mathf.Clamp01(normalizedValue);
            fill.color = color;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = amount;

            var fillRect = fill.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(amount, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }

        private string FormatValue(float value) 
            => Mathf.Approximately(value, Mathf.Round(value))
                ? Mathf.RoundToInt(value).ToString()
                : value.ToString("0.#");
    }
}
