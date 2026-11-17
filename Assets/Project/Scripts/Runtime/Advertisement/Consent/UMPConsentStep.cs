using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Services.Contracts;

#if MODULE_ADMOB
using GoogleMobileAds.Ump.Api;
using UnityEngine;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class UMPConsentStep : IBootstrapStep
    {
        private const int ConsentFormSortingOrder = 9999;

        private readonly UMPSettings _settings;
        private readonly IConsentData _consentData;
        private readonly IServiceLogger _logger;

        public UMPConsentStep(UMPSettings settings, IConsentData consentData, IServiceLogger logger)
        {
            _settings = settings;
            _consentData = consentData;
            _logger = logger;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_settings.IsUMPEnabled)
            {
                _logger.Log("[Loading]: UMP is disabled, skipping task.");
                return UniTask.CompletedTask;
            }

#if MODULE_ADMOB
            return RequestConsentAsync(cancellationToken);
#else
            _logger.Log("[Loading]: UMP SDK is missing, skipping task.");
            return UniTask.CompletedTask;
#endif
        }

#if MODULE_ADMOB
        private async UniTask RequestConsentAsync(CancellationToken cancellationToken)
        {
            if (ConsentInformation.CanRequestAds())
            {
                _logger.Log("[Loading]: UMP is already completed, ad can be loaded.");

                _consentData.IsConsentGiven = true;

                return;
            }

            var completion = new UniTaskCompletionSource();
            var requestParameters = GetRequestParameters();

            ConsentInformation.Update(requestParameters, (FormError updateError) =>
            {
                if (updateError != null)
                {
                    Debug.LogError("[Loading]: Failed to gather consent: " + updateError.Message);

                    completion.TrySetResult();

                    return;
                }

                ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
                {
                    if (showError != null)
                    {
                        Debug.LogError("[Loading]: Failed to show consent: " + showError.Message);

                        completion.TrySetResult();

                        return;
                    }

                    if (ConsentInformation.CanRequestAds())
                    {
                        _logger.Log("[Loading]: UMP successfully completed.");

                        _consentData.IsConsentGiven = true;
                    }

                    completion.TrySetResult();
                });

#if UNITY_EDITOR
                var canvasObject = GameObject.Find("ConsentForm(Clone)");
                if (canvasObject != null)
                {
                    var canvas = canvasObject.GetComponent<Canvas>();
                    if (canvas != null)
                        canvas.sortingOrder = ConsentFormSortingOrder;
                }
#endif
            });

            await completion.Task.AttachExternalCancellation(cancellationToken);
            await UniTask.SwitchToMainThread();
        }

        private ConsentRequestParameters GetRequestParameters()
        {
            var requestParameters = new ConsentRequestParameters();

            if (_settings.UMPDebugMode)
            {
                requestParameters.ConsentDebugSettings = new ConsentDebugSettings()
                {
                    DebugGeography = (GoogleMobileAds.Ump.Api.DebugGeography)_settings.UMPDebugGeography,
                    TestDeviceHashedIds = _settings.TestDevices
                };
            }

            return requestParameters;
        }
#endif
    }
}
