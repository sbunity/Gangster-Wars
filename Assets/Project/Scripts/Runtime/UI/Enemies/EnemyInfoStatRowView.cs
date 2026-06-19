using SBabchuk.Runtime.Services.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.Enemies
{
    public sealed class EnemyInfoStatRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Image _fill;
        [SerializeField] private Color _fillColor = new(1f, 0.48f, 0.08f, 1f);

        public void Render(EnemyInfoStatSnapshot stat)
        {
            if (stat == null)
                return;

            if (_nameText != null)
                _nameText.text = stat.Name;

            if (_valueText != null)
                _valueText.text = FormatValue(stat.Value);

            if (_fill != null)
                RenderFill(stat.NormalizedValue);
        }

        private void RenderFill(float normalizedValue)
        {
            var amount = Mathf.Clamp01(normalizedValue);
            _fill.color = _fillColor;
            _fill.type = Image.Type.Filled;
            _fill.fillMethod = Image.FillMethod.Horizontal;
            _fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            _fill.fillAmount = amount;

            var fillRect = _fill.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(amount, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }

        private string FormatValue(float value)
            => Mathf.Approximately(value, Mathf.Round(value))
                ? Mathf.RoundToInt(value).ToString()
                : value.ToString("0.##");
    }
}
