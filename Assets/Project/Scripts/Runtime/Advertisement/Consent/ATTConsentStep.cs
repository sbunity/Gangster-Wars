using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Services.Contracts;

#if MODULE_IDFA && UNITY_IOS && !UNITY_EDITOR
using System;
using Unity.Advertisement.IosSupport;
using UnityEngine;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class ATTConsentStep : IBootstrapStep
    {
        private const float RequestDelay = 0.5f;
        private const float StatusCheckInterval = 0.5f;

        private readonly ATTSettings _settings;
        private readonly IConsentData _consentData;
        private readonly IServiceLogger _logger;

        public ATTConsentStep(ATTSettings settings, IConsentData consentData, IServiceLogger logger)
        {
            _settings = settings;
            _consentData = consentData;
            _logger = logger;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_settings.IsATTEnabled)
            {
                _logger.Log("[Loading]: ATT is disabled, skipping task.");
                return UniTask.CompletedTask;
            }

#if !MODULE_IDFA
            _logger.Log("[Loading] ATT package has not been imported. Skipping task.");
            return UniTask.CompletedTask;
#elif !UNITY_IOS
            _logger.Log("[Loading] ATT is not supported on this platform, skipping task.");
            return UniTask.CompletedTask;
#elif UNITY_EDITOR
            _logger.Log("[Loading] ATT is not supported in the Unity Editor. Skipping task.");
            return UniTask.CompletedTask;
#else
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                _logger.Log($"[Loading]: IDFA status - {status} (already set, skipping request).");
                return UniTask.CompletedTask;
            }

            _logger.Log("[Loading]: Requesting IDFA..");

            return RequestWithDelayAsync(cancellationToken);
#endif
        }

#if MODULE_IDFA && UNITY_IOS && !UNITY_EDITOR
        private async UniTask RequestWithDelayAsync(CancellationToken cancellationToken)
        {
            while (!Application.isFocused)
                await UniTask.Yield(cancellationToken);

            await UniTask.Delay(TimeSpan.FromSeconds(RequestDelay), DelayType.Realtime, cancellationToken: cancellationToken);

            ATTrackingStatusBinding.RequestAuthorizationTracking();

            while (true)
            {
                var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

                _logger.Log($"[Loading]: IDFA status - {status}");

                if (status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    _consentData.ATTStatus = (AuthorizationTrackingStatus)status;
                    return;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(StatusCheckInterval), DelayType.Realtime, cancellationToken: cancellationToken);
            }
        }
#endif
    }
}
