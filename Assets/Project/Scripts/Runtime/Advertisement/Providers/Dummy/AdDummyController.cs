using System;
using SBabchuk.Runtime.Services.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class AdDummyController : MonoBehaviour
    {
        private const float CanvasMatchAspect = 9f / 16f;

        private GameObject _bannerObject;

        private GameObject _interstitialObject;
        private Button _interstitialCloseButton;

        private GameObject _rewardedVideoObject;
        private Button _rewardedVideoCloseButton;
        private Button _rewardedVideoRewardButton;

        private RectTransform _bannerRectTransform;

        private string _providerName;
        private IAdProviderCallbacks _callbacks;

        public void Init(AdsSettings settings, string providerName, IAdProviderCallbacks callbacks)
        {
            _providerName = providerName;
            _callbacks = callbacks;

            _bannerRectTransform = (RectTransform)_bannerObject.transform;

            AddPointerDownEvent(_interstitialCloseButton, CloseInterstitialButton);
            AddPointerDownEvent(_rewardedVideoCloseButton, CloseRewardedVideoButton);
            AddPointerDownEvent(_rewardedVideoRewardButton, GetRewardButton);

#if UNITY_EDITOR
            SceneVisibilityManager.instance.ToggleVisibility(gameObject, true);
            SceneVisibilityManager.instance.TogglePicking(gameObject, true);
#endif

            DontDestroyOnLoad(gameObject);

            var container = settings.GetContainer<AdDummyContainer>();
            var bannerPosition = container?.BannerPosition ?? BannerPosition.Bottom;

            switch (bannerPosition)
            {
                case BannerPosition.Bottom:
                    _bannerRectTransform.pivot = new Vector2(0.5f, 0.0f);

                    _bannerRectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    _bannerRectTransform.anchorMax = new Vector2(1.0f, 0.0f);

                    _bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
                case BannerPosition.Top:
                    _bannerRectTransform.pivot = new Vector2(0.5f, 1.0f);

                    _bannerRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    _bannerRectTransform.anchorMax = new Vector2(1.0f, 1.0f);

                    _bannerRectTransform.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        public void ShowBanner()
        {
            _bannerObject.SetActive(true);
        }

        public void HideBanner()
        {
            _bannerObject.SetActive(false);
        }

        public void ShowInterstitial()
        {
            _interstitialObject.SetActive(true);
        }

        public void CloseInterstitial()
        {
            _interstitialObject.SetActive(false);

            _callbacks.OnProviderAdClosed(_providerName, AdType.Interstitial);
        }

        public void ShowRewardedVideo()
        {
            _rewardedVideoObject.SetActive(true);
        }

        public void CloseRewardedVideo()
        {
            _rewardedVideoObject.SetActive(false);

            _callbacks.OnProviderAdClosed(_providerName, AdType.RewardedVideo);
        }

        public void CloseInterstitialButton()
        {
            _callbacks.ExecuteInterstitialCallback(true);

            CloseInterstitial();
        }

        public void CloseRewardedVideoButton()
        {
            _callbacks.ExecuteRewardVideoCallback(false);

            CloseRewardedVideo();
        }

        public void GetRewardButton()
        {
            _callbacks.ExecuteRewardVideoCallback(true);

            CloseRewardedVideo();
        }

        private static void AddPointerDownEvent(Component target, Action call)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = target.gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener(_ => call());

            trigger.triggers.Add(entry);
        }

        public static AdDummyController CreateObject()
        {
            var backgroundColor = new Color(0.211f, 0.211f, 0.321f, 1.0f);
            var mainColor = new Color(0.207f, 0.305f, 0.717f, 1.0f);

            var go = new GameObject("[ADS DUMMY CANVAS]");

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            var canvasScaler = go.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = ((float)Screen.width / Screen.height) > CanvasMatchAspect ? 1.0f : 0.0f;

            var graphicRaycaster = go.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            var bannerObject = new GameObject("Banner");
            bannerObject.SetActive(false);

            var bannerRectTransform = bannerObject.AddComponent<RectTransform>();
            bannerRectTransform.SetParent(go.transform);
            bannerRectTransform.anchorMin = new Vector2(0, 1);
            bannerRectTransform.anchorMax = new Vector2(1, 1);
            bannerRectTransform.pivot = new Vector2(0.5f, 1);
            bannerRectTransform.sizeDelta = new Vector2(0, 84);
            bannerRectTransform.anchoredPosition = new Vector2(0, 0);

            var bannerImage = bannerObject.AddComponent<Image>();
            bannerImage.color = mainColor;

            var bannerTextObject = new GameObject("Text");

            var bannerTextRectTransform = bannerTextObject.AddComponent<RectTransform>();
            bannerTextRectTransform.SetParent(bannerRectTransform);
            bannerTextRectTransform.anchorMin = new Vector2(0, 0);
            bannerTextRectTransform.anchorMax = new Vector2(1, 1);
            bannerTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            bannerTextRectTransform.sizeDelta = new Vector2(0, 0);
            bannerTextRectTransform.anchoredPosition = new Vector2(0, 0);

            var bannerText = bannerTextObject.AddComponent<TextMeshProUGUI>();
            bannerText.text = "TEST BANNER";
            bannerText.alignment = TextAlignmentOptions.Converted;
            bannerText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            bannerText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            bannerText.fontSize = 30;
            bannerText.fontWeight = FontWeight.Bold;

            var interstitialObject = new GameObject("Interstitial");
            interstitialObject.SetActive(false);

            var interstitialRectTransform = interstitialObject.AddComponent<RectTransform>();
            interstitialRectTransform.SetParent(go.transform);
            interstitialRectTransform.anchorMin = new Vector2(0, 0);
            interstitialRectTransform.anchorMax = new Vector2(1, 1);
            interstitialRectTransform.pivot = new Vector2(0.5f, 0.5f);
            interstitialRectTransform.sizeDelta = new Vector2(0, 0);
            interstitialRectTransform.anchoredPosition = new Vector2(0, 0);

            var interstitialImage = interstitialObject.AddComponent<Image>();
            interstitialImage.color = backgroundColor;

            var interstitialTitleObject = new GameObject("Title");

            var interstitialTitleRectTransform = interstitialTitleObject.AddComponent<RectTransform>();
            interstitialTitleRectTransform.SetParent(interstitialRectTransform);
            interstitialTitleRectTransform.anchorMin = new Vector2(0, 0.5f);
            interstitialTitleRectTransform.anchorMax = new Vector2(1, 0.5f);
            interstitialTitleRectTransform.pivot = new Vector2(0.5f, 0.5f);
            interstitialTitleRectTransform.sizeDelta = new Vector2(0, 100);
            interstitialTitleRectTransform.anchoredPosition = new Vector2(0, 200);

            var interstitialText = interstitialTitleObject.AddComponent<TextMeshProUGUI>();
            interstitialText.text = "TEST INTERSTITIAL";
            interstitialText.alignment = TextAlignmentOptions.Converted;
            interstitialText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            interstitialText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            interstitialText.fontSize = 62;
            interstitialText.fontWeight = FontWeight.Bold;

            var interstitialCloseButtonObject = new GameObject("Close Button");

            var interstitialCloseButtonRectTransform = interstitialCloseButtonObject.AddComponent<RectTransform>();
            interstitialCloseButtonRectTransform.SetParent(interstitialRectTransform);
            interstitialCloseButtonRectTransform.anchorMin = new Vector2(0.5f, 0);
            interstitialCloseButtonRectTransform.anchorMax = new Vector2(0.5f, 0);
            interstitialCloseButtonRectTransform.pivot = new Vector2(0.5f, 0);
            interstitialCloseButtonRectTransform.sizeDelta = new Vector2(600, 200);
            interstitialCloseButtonRectTransform.anchoredPosition = new Vector2(0, 360);

            var interstitialCloseButtonImage = interstitialCloseButtonObject.AddComponent<Image>();
            interstitialCloseButtonImage.color = mainColor;

            var interstitialCloseButton = interstitialCloseButtonObject.AddComponent<Button>();
            interstitialCloseButton.targetGraphic = interstitialCloseButtonImage;

            var interstitialCloseButtonTextObject = new GameObject("Text");

            var interstitialCloseButtonTextRectTransform = interstitialCloseButtonTextObject.AddComponent<RectTransform>();
            interstitialCloseButtonTextRectTransform.SetParent(interstitialCloseButtonRectTransform);
            interstitialCloseButtonTextRectTransform.anchorMin = new Vector2(0, 0);
            interstitialCloseButtonTextRectTransform.anchorMax = new Vector2(1, 1);
            interstitialCloseButtonTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            interstitialCloseButtonTextRectTransform.sizeDelta = new Vector2(0, 0);
            interstitialCloseButtonTextRectTransform.anchoredPosition = new Vector2(0, 0);

            var interstitialCloseButtonText = interstitialCloseButtonTextObject.AddComponent<TextMeshProUGUI>();
            interstitialCloseButtonText.text = "CLOSE";
            interstitialCloseButtonText.alignment = TextAlignmentOptions.Converted;
            interstitialCloseButtonText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            interstitialCloseButtonText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            interstitialCloseButtonText.fontSize = 60;
            interstitialCloseButtonText.fontWeight = FontWeight.Bold;

            var rvObject = new GameObject("Rewarded Video");
            rvObject.SetActive(false);

            var rvRectTransform = rvObject.AddComponent<RectTransform>();
            rvRectTransform.SetParent(go.transform);
            rvRectTransform.anchorMin = new Vector2(0, 0);
            rvRectTransform.anchorMax = new Vector2(1, 1);
            rvRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rvRectTransform.sizeDelta = new Vector2(0, 0);
            rvRectTransform.anchoredPosition = new Vector2(0, 0);

            var rvImage = rvObject.AddComponent<Image>();
            rvImage.color = backgroundColor;

            var rvTitleObject = new GameObject("Title");

            var rvTitleRectTransform = rvTitleObject.AddComponent<RectTransform>();
            rvTitleRectTransform.SetParent(rvRectTransform);
            rvTitleRectTransform.anchorMin = new Vector2(0, 0.5f);
            rvTitleRectTransform.anchorMax = new Vector2(1, 0.5f);
            rvTitleRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rvTitleRectTransform.sizeDelta = new Vector2(0, 100);
            rvTitleRectTransform.anchoredPosition = new Vector2(0, 200);

            var rvText = rvTitleObject.AddComponent<TextMeshProUGUI>();
            rvText.text = "TEST REWARDED VIDEO";
            rvText.alignment = TextAlignmentOptions.Converted;
            rvText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            rvText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            rvText.fontSize = 62;
            rvText.fontWeight = FontWeight.Bold;

            var rvCloseButtonObject = new GameObject("Close Button");

            var rvCloseButtonRectTransform = rvCloseButtonObject.AddComponent<RectTransform>();
            rvCloseButtonRectTransform.SetParent(rvRectTransform);
            rvCloseButtonRectTransform.anchorMin = new Vector2(0.5f, 0);
            rvCloseButtonRectTransform.anchorMax = new Vector2(0.5f, 0);
            rvCloseButtonRectTransform.pivot = new Vector2(0.5f, 0);
            rvCloseButtonRectTransform.sizeDelta = new Vector2(600, 200);
            rvCloseButtonRectTransform.anchoredPosition = new Vector2(0, 360);

            var rvCloseButtonImage = rvCloseButtonObject.AddComponent<Image>();
            rvCloseButtonImage.color = mainColor;

            var rvCloseButton = rvCloseButtonObject.AddComponent<Button>();
            rvCloseButton.targetGraphic = rvCloseButtonImage;

            var rvCloseButtonTextObject = new GameObject("Text");

            var rvCloseButtonTextRectTransform = rvCloseButtonTextObject.AddComponent<RectTransform>();
            rvCloseButtonTextRectTransform.SetParent(rvCloseButtonRectTransform);
            rvCloseButtonTextRectTransform.anchorMin = new Vector2(0, 0);
            rvCloseButtonTextRectTransform.anchorMax = new Vector2(1, 1);
            rvCloseButtonTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rvCloseButtonTextRectTransform.sizeDelta = new Vector2(0, 0);
            rvCloseButtonTextRectTransform.anchoredPosition = new Vector2(0, 0);

            var rvCloseButtonText = rvCloseButtonTextObject.AddComponent<TextMeshProUGUI>();
            rvCloseButtonText.text = "CLOSE";
            rvCloseButtonText.alignment = TextAlignmentOptions.Converted;
            rvCloseButtonText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            rvCloseButtonText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            rvCloseButtonText.fontSize = 60;
            rvCloseButtonText.fontWeight = FontWeight.Bold;

            var rvRewardButtonObject = new GameObject("Get Reward Button");

            var rvRewardButtonRectTransform = rvRewardButtonObject.AddComponent<RectTransform>();
            rvRewardButtonRectTransform.SetParent(rvRectTransform);
            rvRewardButtonRectTransform.anchorMin = new Vector2(0.5f, 0);
            rvRewardButtonRectTransform.anchorMax = new Vector2(0.5f, 0);
            rvRewardButtonRectTransform.pivot = new Vector2(0.5f, 0);
            rvRewardButtonRectTransform.sizeDelta = new Vector2(600, 200);
            rvRewardButtonRectTransform.anchoredPosition = new Vector2(0, 600);

            var rvRewardButtonImage = rvRewardButtonObject.AddComponent<Image>();
            rvRewardButtonImage.color = mainColor;

            var rvRewardButton = rvRewardButtonObject.AddComponent<Button>();
            rvRewardButton.targetGraphic = rvRewardButtonImage;

            var rvRewardButtonTextObject = new GameObject("Text");

            var rvRewardButtonTextRectTransform = rvRewardButtonTextObject.AddComponent<RectTransform>();
            rvRewardButtonTextRectTransform.SetParent(rvRewardButtonRectTransform);
            rvRewardButtonTextRectTransform.anchorMin = new Vector2(0, 0);
            rvRewardButtonTextRectTransform.anchorMax = new Vector2(1, 1);
            rvRewardButtonTextRectTransform.pivot = new Vector2(0.5f, 0.5f);
            rvRewardButtonTextRectTransform.sizeDelta = new Vector2(0, 0);
            rvRewardButtonTextRectTransform.anchoredPosition = new Vector2(0, 0);

            var rvRewardButtonText = rvRewardButtonTextObject.AddComponent<TextMeshProUGUI>();
            rvRewardButtonText.text = "GET REWARD";
            rvRewardButtonText.alignment = TextAlignmentOptions.Converted;
            rvRewardButtonText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            rvRewardButtonText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            rvRewardButtonText.fontSize = 60;
            rvRewardButtonText.fontWeight = FontWeight.Bold;

            var controller = go.AddComponent<AdDummyController>();
            controller._bannerObject = bannerObject;

            controller._interstitialObject = interstitialObject;
            controller._interstitialCloseButton = interstitialCloseButton;

            controller._rewardedVideoObject = rvObject;
            controller._rewardedVideoCloseButton = rvCloseButton;
            controller._rewardedVideoRewardButton = rvRewardButton;

            return controller;
        }
    }
}
