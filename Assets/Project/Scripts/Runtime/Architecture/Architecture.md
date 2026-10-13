# Gangster Wars Runtime Architecture

## Service Layer

The project now has a Zenject-driven service layer under `Assets/Project/Scripts/Runtime`.

- `IAssetProvider` centralizes access to existing ScriptableObject databases and future prefab loading.
- `ISaveService` wraps the current `PersistableSO` save/load flow and preserves the existing BinaryFormatter plus JsonUtility file format.
- `IPlayerProgressService` wraps `PlayerPrefsDatabase` for coins, level progress, selected equipment, ammo, grenades, and upgrades.
- `IPoolService` wraps the legacy `PoolManager` and keeps pool names compatible with existing prefabs.
- `IGameFactory` creates enemies, bullets, grenades, collisions, and bonuses through the pool service, then injects dependencies into pooled objects.
- `ISceneLoaderService` loads scenes through `ZenjectSceneLoader` when available and falls back to Unity scene loading.
- `ILevelService` and `ILevelFlowService` track the current level state and finish panel.
- `IDamageService` and `ICombatService` isolate common damage, hit, and reward operations.
- `IWaveSkipRewardService` grants the coin reward for manually starting a wave early.
- `IAudioSettingsService` wraps saved music and sound settings.
- `IAudioService` plays one-shot `SoundConfig` sounds from a pooled set of voices.
- `IMusicService` plays looped background `MusicTrack`s with fade out/in and survives scene loads.
- `IInputService` wraps pointer input and world pointer position.
- `IHandService` exposes grenade dragging state and placement without `HandController.Instance`.
- `ILeaderWeaponController` exposes the leader weapon controls used by the sight/aim UI.

## Contexts

`Assets/Resources/ProjectContext.prefab` contains `ProjectContext` and `ProjectInstaller`.

`ProjectInstaller` binds global services as singletons and declares typed Zenject signals:

- `EnemyDiedSignal`
- `GameFinishedSignal`
- `CoinsChangedSignal`
- `ProgressUpgradedSignal`
- `WeaponAmmoChangedSignal`
- `BarricadeHealthChangedSignal`
- `GrenadeDamageSignal`

The main scenes now contain `SceneContext`:

- `MainScene`
- `LevelSelect`
- `GameScene`

`GameScene` also contains `GameSceneInstaller`, which binds scene references such as `LevelController`, `BarricadeController`, wave bar, enemy spawn points, and enemy target points.

`GameSceneInstaller` also binds scene adapters discovered in the scene:

- `ILevelRuntimeService` from `LevelController`
- `ILeaderWeaponController` from `LeaderGangsterController`
- `IHandService` from `HandController`

## Entity Composition

The current prefabs keep their legacy controller components for compatibility, but pooled objects are injected through `IGameFactory` before initialization. Focused runtime blocks have been added to existing prefabs:

- enemies: `EnemyView`, `EnemyHealth`, `EnemyMovement`, `EnemyAttack`, `EnemyDeath`, `EnemyReward`
- characters: `CharacterView`, `CharacterWeapon`
- projectiles: `ProjectileView`, `ProjectileMovement`
- grenades: `GrenadeView`
- bonuses: `BonusView`
- collisions: `CollisionView`

This keeps existing visuals, Spine animations, colliders, tags, sorting, pool setup, balance, and scene references stable while moving dependencies behind contracts.

## Save Flow

The save format is unchanged.

`SaveService` delegates to `PersistableSO.Save`, `PersistableSO.Load`, and `PersistableSO.SaveSO`. Data is still serialized to `Application.persistentDataPath` as `Main_{ScriptableObjectName}.pso` using the existing JsonUtility/BinaryFormatter flow.

Runtime progress mutations in gameplay, level select, store UI, ammo UI, bonuses, player characters, and settings go through `IPlayerProgressService` or `IAudioSettingsService`. These services persist via `ISaveService` and raise typed Zenject signals (`CoinsChangedSignal`, `ProgressUpgradedSignal`, `WeaponAmmoChangedSignal`, ...) directly. The earlier legacy static-event / `LegacySignalBridge` path has been removed.

## Asset Loading Flow

`AssetProvider` resolves databases from `PersistableSO` when present and falls back to the existing editor/resource database lookup. This preserves current data sources and allows a later catalog, Resources, or Addressables-backed implementation without changing gameplay systems.

Prefab spawning currently flows through:

`IGameFactory` -> `IPoolService` -> legacy `PoolManager` -> existing pooled prefabs.

## Level Backgrounds

Each level's background is a prefab with a `LevelBackground` component on its root. The prefab owns the layered art, the gameplay zones (`Place` and `FireZone` colliders), the enemy paths (`[SpawnPoints]`, `[Targets]`) and the anchors for the barricade and the gangsters.

- `LevelBackgroundDatabase` (`Assets/Resources/Databases/LevelBackgroundDatabase.asset`) maps level id to background prefab, with a default prefab as fallback. It is kept outside `LevelDatabase`/`ChapterDatabase` on purpose: those are overwritten from `.pso` saves through JsonUtility, which cannot restore prefab references.
- `ILevelBackgroundFactory` (`LevelBackgroundFactory`) instantiates the prefab for a level through the container.
- `GameSceneInstaller` binds `LevelBackground` as a non-lazy single, created under `_backgroundRoot` for `IPlayerProgressService.CurrentLevelId`.
- `LevelController` takes its enemy spawn and target points from the injected `LevelBackground`.
- `LevelActorsLayout` (on `[Scripts]` in `GameScene`) moves the barricade and the gangsters to the background anchors, keeping their z.
- The car sprites (`Car_Red`, `Car_Black`) in every background prefab use sorting `Default/-1000`, and the sniper renderer uses `Default/-1001`. That puts the sniper behind the car, leaning on its roof, while the leader, bomber and barricade still draw in front. New backgrounds must keep the car at this sorting.

## UI Sounds

- `UISoundsConfig` (`Assets/Resources/Sounds/UI/UISoundsConfig.asset`) holds the shared UI `SoundConfig`s. `ProjectInstaller` binds it from its `_uiSounds` field.
- `UIClickSound` goes on any clickable UI object (`Button`, `Toggle`, `ScrollNudgeButton`). On a left click it plays `UISoundsConfig.Click` through `IAudioService`. It plays only when the object was interactable at pointer down, so a disabled button stays silent, and a buy button that becomes disabled after it is clicked still plays the sound.
- `UIClickSound._type` picks the sound: `Click` (default) plays `UISoundsConfig.Click`, `Purchase` plays `UISoundsConfig.Purchase` (`Assets/Resources/Sounds/Shop/Shop_BuyUpgrade.asset`), `Supply` plays `UISoundsConfig.Supply` (`Assets/Resources/Sounds/Shop/Shop_BuyPatronsSelect.asset`). In `MainScene` the `Purchase` type is set on the buy and upgrade buttons of weapons (`LockElementController`, `UnlockElementController.BuyUpgrade`) and barricades (`LockDElementController`, `UnlockDElementController`), and on the character upgrade buttons (`MainPlayerUnlockElementController`). The `Supply` type is set on the magazine buttons (`UnlockElementController.BuyMagazine`), the grenade buy buttons (`UnlockGElementController`) and the barricade select buttons (`UnlockDElementController.Select`). `UIClickSound._customSound` overrides the type when set: the Bomber buy button (`[Canvas]/BuyBomber`) plays `Shop_BuyBomber.asset`, the Sniper buy button (`[Canvas]/Sniper`) plays `Shop_BuySniper.asset`.
- Buttons without the component stay silent: the in-game weapon switch buttons, the grenade drag buttons, and the store card backgrounds that do nothing on click. New buttons need the component added. UI prefabs created at runtime must be instantiated through the container (`IInstantiator`) so the component gets its dependencies.

## Coin Sound

`CoinInfo` has an `_addSound` field (`SoundConfig`). When coins go up (enemy kill reward, early wave start reward), the counter counts up and then pulses; the sound plays together with the pulse. A fast series of rewards restarts the count-up, so the sound plays once when it settles. Only the `GameScene` coin counter has the sound set (`Assets/Resources/Sounds/UI/UI_AddMoney.asset`); in `MainScene` the field is empty and coins only go down there.

## Leader Aiming

- The leader's weapon is aimed by Spine IK. `SightController` sits on the `Aim` bone (`SkeletonUtilityBone`, Override mode) and moves it to the touch point shifted by the current weapon's `AimOffsetY`. Bullets fly from the muzzle (`Center`) to `IAimService.CurrentAimPosition`, the touch point without the shift.
- Each weapon has its own Spine skeleton (`AssetDataController._skeletonDataAsset`) and its own IK setup, so the shift is per weapon: `LeaderGangsterController._weaponRigs` (`LeaderWeaponRig`: muzzle `Points` + `AimOffsetY`, indexed by weapon id). `ILeaderWeaponController.AimOffsetY` exposes the selected weapon's value to `SightController`.
- Weapons 0–3 aim their arms (`LeftArmAim`, `RightArmAim`, `RightHandAim`) at `Aim`, so the barrel ends up above the bone; their `AimOffsetY` is `-0.58`, which lowers `Aim` so the barrel points near the touch point. The minigun (weapon 4) aims the `Weapon` bone itself (`WeaponAim` IK), so its barrel points exactly at `Aim`; its `AimOffsetY` is `0`. With the old shared `-0.58` its bullets flew 4–5° above the barrel.
- A new weapon rig needs its `AimOffsetY` tuned: `0` when IK aims the weapon bone directly, a negative value when IK aims an arm and the barrel sits above it.

## Scene Transition And Pause

- `GameResultPanelController.Show` pauses the game (`Time.timeScale = 0`). The panel buttons (`TransitionTo`) do not unpause it; the game stays paused while the loading screen fades in and the next scene loads, so the leaving `GameScene` cannot play shots, reloads or other gameplay sounds under the fade.
- `SceneTransitionService.TransitionToAsync` sets `Time.timeScale = 1` right after the new scene has loaded, before the settle frames and the fade-out. Every scene reached through the transition service starts unpaused, whatever paused the previous one.
- The loading screen fade, the music fades and the result panel fade use unscaled time, so they keep running while the game is paused.

## Game Result Sounds

`GameResultPanelController` has a `_showSound` field (`SoundConfig`) and plays it through `IAudioService` when the panel is shown. The sound plays together with the panel, so the win sound waits for the win panel's `_showDelay`. In `GameScene` the win panel uses `Assets/Resources/Sounds/Endgame/Level_Win.asset` and the lose panel uses `Level_Lose.asset`. Surrendering also shows the lose panel, so it plays the same sound.

## Explosion Sounds

- `CollisionController` has a `_sound` field (`SoundConfig`) and plays it through `IAudioService` in `Init`, the moment a collision comes out of the pool. Collisions are created only by `GameFactory.CreateCollision`, which injects the dependencies.
- Explosion sounds live in `Assets/Resources/Sounds/Weapons/Explosions/`. `Collision_5` uses `Explosion_SpikyFire`, `Collision_4` uses `Explosion_RoundFire` and `Collision_3` uses `Explosion_TallFire`. All three use the Epic Toon FX `etfx_explosion_rocket` clip with a fixed pitch (1.1, 1.2 and 1.0).
- `Collision_5` is spawned by every dynamite (`DynamiteBulletController`: the player Bomber, Enemy_9, Enemy_14) and by Grenade_1. `Collision_4` is Grenade_2, `Collision_3` (`TallExplosionFire`) is Grenade_4. Grenade_3, the Molotov (`Collision_2`, fire), has no sound.
- The Epic Toon FX effect prefabs carry their own `AudioSource`s. `Collision_4` and `Collision_5` remove them as prefab overrides, so explosion audio goes only through `IAudioService`. New collision prefabs built from Epic Toon FX effects must remove the `AudioSource` too and set `_sound` instead.

## Gangster And Enemy Shot Sounds

- Shot sounds are data, set per entry in the database editors ("Звук пострілу"): `Weapon.Sounds` (`WeaponStoreDatabase`) for the leader's weapons, `Personage.Sounds` (`PersonageSounds`, `MainPlayerDatabase`) for the ally gangsters, `Enemy.Sounds` (`EnemySounds`, `EnemyDatabase`) for enemies. Each has a `Shot` `SoundConfig`; an empty one stays silent.
- `SniperGangsterController` and `BomberGangsterController` play `Sounds.Shot` through `IAudioService` in `SpawnBullet`, so the sound matches the Spine fire/throw event, not the start of the animation.
- `EnemyControllerBase.PlayShotSound()` plays the enemy's `Sounds.Shot`. Every ranged enemy controller calls it where it spawns bullets: `RangeEnemyController` (Enemy_5, Enemy_10) and `BomberEnemyController` (Enemy_9) once per attack, `BomberBossEnemyController` (Enemy_14) once for its two simultaneous dynamites, `BigBossEnemyController` (Enemy_12) once per bullet of its 4-shot burst (0.1 s apart), so the shot `SoundConfig` needs `MaxInstances` of at least 4.
- Enemy_5, Enemy_10 and Enemy_12 share `Weapons/Enemy/Enemy_Shot.asset` (`EnemyShots.wav`, trimmed to the 1.1 s shot so silent tails don't hold pool voices).
- The ally Bomber has its own `SoundConfig` (`Weapons/Gangsters/Bomber_Throw.asset`), separate from the enemies, so they can be tuned apart even though both use `ThrowDinamite.wav`. Enemy_9 and Enemy_14 share `Weapons/Enemy/Enemy_DynamiteThrow.asset`. Sound folders under `Assets/Resources/Sounds/Weapons/`: `Player` (leader weapons), `Gangsters` (Sniper, Bomber), `Enemy`, `Explosions`.

## Enemy Death Sounds

- `EnemySounds.Death` (`SoundConfig`, "Звук смерті" in the `EnemyDatabase` editor) is the enemy's death sound; an empty one stays silent.
- `EnemyControllerBase.OnHealthDied` plays it through `IAudioService` right after `EnemyDeath.Play` starts the `Death` animation. `EnemyHealth` raises `Died` only once per life, so the sound plays once per kill.
- All 15 enemies share `Assets/Resources/Sounds/Enemies/Enemy_Die.asset` (`Audio/Enemies/EnemyDie1.wav`, preloaded). It has `MaxInstances` 4 and `MinInterval` 0.05 s, so a grenade that kills a crowd doesn't stack a wall of identical voices. `EnemyDie2.wav` is imported but unused; adding it to `_clips` makes the sound pick one of the two at random.

## Enemy Melee Attack Sounds

- `EnemySounds.Attack` (`SoundConfig`, "Звук атаки" in the `EnemyDatabase` editor) is the enemy's melee hit sound; an empty one stays silent.
- `EnemyControllerBase.GiveDamage` plays it through `IAudioService` together with the barricade damage and the impact effect, so the sound matches the Spine `Fire` event, not the start of the `Attack` animation. Ranged controllers override `GiveDamage` and play `Sounds.Shot` instead, so only melee enemies (`MeleeEnemyController`) use `Attack`.
- The melee enemies (Enemy_1, 2, 3, 4, 6, 7, 8, 11, 13, 15) share `Assets/Resources/Sounds/Enemies/Enemy_Attack.asset` (`Audio/Enemies/EnemyAttack.wav`, preloaded, trimmed to the 0.4 s hit so silent tails don't hold pool voices). It has `MaxInstances` 4 and `MinInterval` 0.05 s, so a crowd at the barricade hitting on the same frame doesn't stack identical voices. The ranged enemies leave the field empty.
- The clip fades in over its first ~0.1 s and peaks at 0.13 s, so played from the start it lands late against the `Fire` event. `Enemy_Attack` sets `StartOffset` to 0.08 s to skip the fade-in.
- `SoundConfig.StartOffset` (seconds, default 0) is where playback starts inside the clip; `AudioSourcePool` sets `AudioSource.time` to it before `Play`. It works for any sound whose clip has a slow attack or a silent head, without re-cutting the wav.

## Enemy Shots At The Barricade

- Ranged enemies that shoot bullets (`RangeEnemyController`: Enemy_5, Enemy_10; `BigBossEnemyController`: Enemy_12) aim at the barricade instead of shooting straight left. `EnemyControllerBase.GetShotTarget()` asks `IBarricadeTarget` for the hit point of the enemy's lane (`Target`, the lane point at the barricade) and the bullet flies from the muzzle to that point.
- `IBarricadeTarget` is implemented by `BarricadeHitArea` on the `Barrcade` object in `GameScene` and bound by `GameSceneInstaller` (`_barricadeHitArea`). The hit point is the lane point raised by `_hitHeight` (1 unit), clamped into the barricade `PolygonCollider2D` with `ClosestPoint`, so it stays inside the barricade for every lane.
- Without it, tall enemies on the top lane missed: Enemy_12 raises its weapon in `Attack`, its muzzle ends up 1.9–2.5 units above the lane, and a horizontal shot passed over the barricade top. Aimed shots tilt down by about 4–10°.
- The barricade art and collider are unchanged. Dynamite throwers (`BomberEnemyController`, `BomberBossEnemyController`) already throw at `Target` and are not affected.

## Background Music

- `MusicService` (`IMusicService`, Project-scoped) owns one looping `AudioSource` on a `DontDestroyOnLoad` `[Music]` object and mutes it when music is disabled in the settings.
- Each scene has a `[SceneMusic]` root object with a `SceneMusic` component, which asks for that scene's `MusicTrack` in `Start`. If the requested track is already playing, the service leaves it alone. That is why the music keeps playing without a restart between `MainScene` and `LevelSelect` (both use `Music_Menu`).
- A different track first fades out over the current track's `FadeOutDuration` and then fades in over the new track's `FadeInDuration`. Fades ignore `Time.timeScale`.
- `GameResultPanelController.Show` calls `IMusicService.Stop()`, so the game music (`Music_Game`) fades out when the win/lose panel appears. The next scene's `SceneMusic` starts music again.
- Tracks live in `Assets/Resources/Sounds/Music/`. The music clips are imported as streamed Vorbis.

## Helpers Extracted From LevelController

- `BonusDropService` (`IBonusDropService`, Project-scoped) decides which bonus id is eligible to drop, based on owned weapons/grenades.
- `RandomPathPicker` is a plain class that yields spawn-path indices without repetition (refilling once exhausted).
- `CollisionEffectId.Impact` names the impact-effect pool id used by bullet/melee hits.

## Known Limitations

Remaining cleanup is limited to non-gameplay editor/constructor tooling and deeper replacement of legacy controller bodies.

- Constructor/debug scripts under `Assets/Project/Scripts/Runtime/UI/Constructor` still access legacy databases because they are editor-like tooling around the old level constructor workflow.
- `PoolManager` still owns the actual pool implementation; it is isolated behind `IPoolService` for migrated code.
- `LevelController` schedules waves internally (DOTween-based) for gameplay compatibility.
- `EnemyControllerBase` still hosts trigger-collision handling (`OnTriggerEnter2D`) directly; the focused runtime blocks own view, health, movement, attack, death, and reward responsibilities.
