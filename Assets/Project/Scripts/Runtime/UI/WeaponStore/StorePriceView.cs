using SBabchuk.Runtime.Services.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public sealed class StorePriceView : MonoBehaviour
    {
        [SerializeField] private Text _label;
        [SerializeField] private GameObject _coinIcon;

        private RectTransform _labelRect;
        private Vector2 _coinLabelAnchorMin;
        private Vector2 _coinLabelAnchorMax;
        private Vector2 _coinLabelPosition;
        private Vector2 _coinLabelSize;

        public void Show(StoreItemPrice price)
        {
            CacheCoinLayout();

            _label.text = price.Label ?? string.Empty;
            _coinIcon.SetActive(!price.IsRealMoney);

            if (price.IsRealMoney)
                FillPriceBox();
            else
                RestoreCoinLayout();
        }

        private void CacheCoinLayout()
        {
            if (_labelRect != null)
                return;

            _labelRect = _label.rectTransform;
            _coinLabelAnchorMin = _labelRect.anchorMin;
            _coinLabelAnchorMax = _labelRect.anchorMax;
            _coinLabelPosition = _labelRect.anchoredPosition;
            _coinLabelSize = _labelRect.sizeDelta;
        }

        private void FillPriceBox()
        {
            _labelRect.anchorMin = Vector2.zero;
            _labelRect.anchorMax = Vector2.one;
            _labelRect.anchoredPosition = Vector2.zero;
            _labelRect.sizeDelta = Vector2.zero;
        }

        private void RestoreCoinLayout()
        {
            _labelRect.anchorMin = _coinLabelAnchorMin;
            _labelRect.anchorMax = _coinLabelAnchorMax;
            _labelRect.anchoredPosition = _coinLabelPosition;
            _labelRect.sizeDelta = _coinLabelSize;
        }
    }
}
