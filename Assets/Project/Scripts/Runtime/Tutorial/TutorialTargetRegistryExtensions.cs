using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Tutorial
{
    public static class TutorialTargetRegistryExtensions
    {
        public static bool TryGetFirst(this ITutorialTargetRegistry registry, IEnumerable<TutorialTargetKey> keys, out ITutorialAnchor target)
        {
            foreach (var key in keys)
            {
                if (registry.TryGet(key, out target))
                    return true;
            }

            target = null;
            return false;
        }
    }
}
