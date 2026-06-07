using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace SBabchuk
{
    public class MeleeEnemyController : EnemyControllerBase
    {
        /// <summary>
        /// Запускаєм нову атаку
        /// </summary>
        public override void Attacked()
        {
            WaitNextAttack(properties.speedAtack);
        }
    }
}
