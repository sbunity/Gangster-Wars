using UnityEngine;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class NullAimService : IAimService
    {
        public Vector2 CurrentAimPosition => Vector2.zero;
    }
}
