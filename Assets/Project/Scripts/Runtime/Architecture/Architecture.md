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
- `IVibrationSettingsService` wraps the saved vibration setting.
- `IVibrationService` plays one-shot `VibrationConfig` haptics through the platform `IHapticDevice` and drops a request while the previous vibration is still running.
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

- `Init`
- `MainScene`
- `LevelSelect`
- `GameScene`

`GameScene` also contains `GameSceneInstaller`, which binds scene references such as `LevelController`, `BarricadeController`, wave bar, enemy spawn points, and enemy target points.

`GameSceneInstaller` also binds scene adapters discovered in the scene:

- `ILevelRuntimeService` from `LevelController`
- `ILeaderWeaponController` from `LeaderGangsterController`
- `IHandService` from `HandController`

## Game Bootstrap (Init Scene)

- `Init` (`Assets/Project/Scenes/Init.unity`) is the first scene in Build Settings. Its `SceneContext` is the first one to wake up, so the `ProjectContext` is created there.
- `PlayModeStartScene` (editor) sets `EditorSceneManager.playModeStartScene` to the first enabled build scene, so Play in the Editor always starts from `Init`, whatever scene is open, and then goes to `MainScene`.
- `InitSceneInstaller` binds the bootstrap steps (`IBootstrapStep`), `GameBootstrapper` and `InitSceneEntryPoint`. Its `_settings` (`BootstrapSettings`) holds the minimum duration (5 s), the overtime tail (1 s) and the first scene (`MainScene`).
- `GameBootstrapper` runs the steps one by one in binding order, one frame apart, so the loading screen renders before any heavy work. Steps:
  1. `LegacySaveFilesCleanup` deletes old `Main_*.pso` saves.
  2. `PlayerProgressLoadStep` loads the progress (`IPlayerProgressRepository.LoadAsync`).
- `InitSceneEntryPoint` starts the minimum-duration timer and the bootstrapper together (unscaled time). When the timer ends: if the steps are done, it moves on; if not, it waits for them and then for the overtime tail. Then it goes to the first scene through `ISceneTransitionService`. The transition loading screen fades in over the `Init` screen, which looks the same, so the change is not visible.
- The `Init` screen (`[LoadingScreen]` in the scene) is a static canvas copied from `LoadingScreen.prefab`, without `LoadingScreenView`. It is visible from the first frame and is unloaded with the scene.
- `PlayerProgressRepository.Progress` throws until `Load` has run. Nothing may read the progress while the `ProjectContext` is being built: `AudioService` and `MusicService` apply the audio settings when their `[Audio]`/`[Music]` objects are created on first use, not in `Initialize`.
- New startup work is a new `IBootstrapStep` bound in `InitSceneInstaller.BindBootstrapSteps`.

## Progress Persistence

- `IProgressStorage` is the async source of the progress: `LoadAsync` (returns `null` when there is no save) and `SaveAsync`. `FileProgressStorage` is the current implementation (`progress.json` in `Application.persistentDataPath`, written through a `.tmp` file, a broken file is moved to `.corrupted`). Another source (cloud, another format) is a new `IProgressStorage` bound in `ProjectInstaller.BindProgress`.
- `SaveAsync` serializes the progress synchronously, before its first `await`, and then writes the text. The services keep changing `PlayerProgress` while a write is in flight, so every new storage must take its snapshot the same way.
- `PlayerProgressRepository` keeps the progress in memory. `LoadAsync` loads it (or creates the default one and normalizes it) once, in the `Init` scene. `Save()` stays synchronous for the services: it starts the write through `CoalescingTaskRunner`, which never runs two writes at once. Calls made while a write is running are merged into one more write of the latest state.
- `CoalescingTaskRunner` (`Architecture`) runs an async task without overlap and repeats it once if it was asked again during the run. A failed run is logged and does not stop the next one.
- `Exists` and `Delete` are file-only, so they are on `FileProgressStorage` and not in the contract. The editor `PlayerProgressWindow` uses `FileProgressStorage` directly outside Play Mode and the live repository in Play Mode once the progress is loaded.

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

- Ranged enemies that shoot bullets (`RangeEnemyController`: Enemy_5, Enemy_10; `BigBossEnemyController`: Enemy_12) aim at the barricade instead of shooting straight left. `EnemyControllerBase.GetShotTarget()` asks `IBarricadeTarget` for the hit point of the enemy's lane (`Target`, the lane point at the barricade). `RangeEnemyController` fires its bullet from the muzzle to that point.
- `BigBossEnemyController` (Enemy_12) fires along the barrel instead: the bullet flies along `Muzzle.right` (`_shotDistance`, 20 units). `Muzzle` is the bullet point under `New BoneFollower` (follows the `Weapon` bone); its local rotation (23.43°) is the `Weapon` region attachment rotation (-156.57°) + 180°, so `right` points out of the barrel, not along the bone.
- To make the barrel hit the barricade, `EnemyWeaponAim` on Enemy_12 rotates the `Hand B1` bone (shoulder; the arm chain `Hand B1 → Hand B2 → Weapon` carries the gun) in `SkeletonAnimation.UpdateWorld` so the muzzle ray passes through the hit point. `Attack()` calls `AimAt(GetShotTarget())`, `StopAllTweens()` calls `Release()`; the aim blends in/out over `_blendTime` and is clamped to `_maxAngle`. Every Enemy_12 animation keys `Hand B1`, so the offset never accumulates. The `Attack` animation alone raises the barrel 5–20° and the boss shoots from 12 units away, so without aiming its bullets passed over the barricade on every lane; with it the barrel tilts down by about 5–7° and all shots hit.
- `IBarricadeTarget` is implemented by `BarricadeHitArea` on the `Barrcade` object in `GameScene` and bound by `GameSceneInstaller` (`_barricadeHitArea`). The hit point is the lane point raised by `_hitHeight` (1 unit), clamped into the barricade `PolygonCollider2D` with `ClosestPoint`, so it stays inside the barricade for every lane.
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

## Vibration

- `IVibrationService` (`VibrationService`, project context) is the single entry point for haptics. `Vibrate(VibrationConfig)` does nothing when vibration is off in settings or the device has no vibrator. Vibrations never run in parallel or queue: while one is running (`TotalDurationMs` of the last played config, unscaled time), a request with the same or lower `Priority` is dropped, and a request with a higher `Priority` cancels the running one and starts.
- `VibrationConfig` (ScriptableObject, `Assets/Resources/Vibrations/`) is a pattern: `_pulses` (`VibrationPulse`: `DelayMs` pause before the pulse, `DurationMs`, `Intensity` 0–1, `Sharpness` 0–1, used on iOS only) and `_priority`.
- Configs:
  - `Barricade_Damage`: priority 0, one 40 ms pulse at 0.7.
  - `Level_Win`: priority 10, 1080 ms. Three quick crisp taps of rising strength, a strong 350 ms hold, then two light sparkle taps ("ta-da-da-DAAA, tik-tik").
  - `Level_Lose`: priority 10, 1660 ms. Three long dull pulses that get weaker and slower, the last one 600 ms ("womp, womp, wooomp").
- The game-end configs need the higher priority: the lose panel has no `_showDelay`, so the lose vibration starts in the same frame as the killing hit's `Barricade_Damage` and would be dropped otherwise.
- `GameResultPanelController._showVibration` plays together with `_showSound` in `Show`, so the win vibration waits for the win panel's `_showDelay` like the sound. In `GameScene` `Win` uses `Level_Win`, `Lose` uses `Level_Lose`. Surrendering shows the lose panel, so it vibrates too.
- `IHapticDevice` is the platform adapter, picked in `ProjectInstaller.BindVibration` by define:
  - `AndroidHapticDevice`: JNI `Vibrator`. API 26+ with amplitude control plays `VibrationEffect.createWaveform(timings, amplitudes)`; without amplitude control and on API 25 it plays the on/off timings at the device's default strength. Waveforms are built once per config.
  - `IosHapticDevice`: Core Haptics through `Assets/Plugins/iOS/HapticFeedback.mm` (compiled with `-fobjc-arc`, links `CoreHaptics`). Pulses up to 50 ms become transient taps, longer ones continuous events with the pulse's intensity and sharpness. Devices without Core Haptics fall back to a `UIImpactFeedbackGenerator` tap per pulse.
  - `NullHapticDevice`: Editor and other platforms.
- Android needs `android.permission.VIBRATE`. Unity adds it only when scripts call `Handheld.Vibrate`, so `VibratePermissionInjector` (`IPostGenerateGradleAndroidProject`) adds it to the generated manifest.
- The setting is `PlayerPrefs.Vibration` (`mySwitch`, `On` by default, so older saves without the field keep vibration on). `SettingsPopupController.vibrationToggle` drives it in `MainScene` and `GameScene`.
- `BarricadeController.TakeDamage` fires `BarricadeDamagedSignal` for every hit (bullets, melee, explosions). `BarricadeDamageVibrationFeedback` on `[Barricade]/Barrcade` listens to it and plays its `_vibration` config. New haptic feedback follows the same shape: the gameplay or UI code that owns the moment calls `IVibrationService` with its own `VibrationConfig`, directly or through a signal.

## Tutorial

The first-time tutorial teaches the core loop in the order the player meets it: the first battle (level 1, `TutorialConfig.TutorialLevelId`), the first upgrade in the menu, grenades on level 2 (`TutorialConfig.GrenadeLevelId`) and two one-off hints (first bonus, second weapon). It is built from small steps that wait for gameplay signals, show one hint at a time and are saved once completed.

- `TutorialConfig` (`Assets/Resources/Databases/TutorialConfig.asset`, `IAssetProvider.TutorialConfig`) holds the tutorial and grenade level ids, the English captions and every threshold (slow-motion scales, how long the first enemy must be on the field before the shoot and grenade hints, the highlight size around an enemy, rounds that count as reloaded, minimum seconds before the next wave, the countdown value at which level 1 waits for the field to be cleared, barricade health floor, weapon to upgrade, gift grenade and count, hint durations) and the store tour: the level after which it starts, the minimum time a page stays before a tap is accepted and its pages (`StoreTourPage`: store tab to open or `None`, target, caption).
- Progress: `PlayerProgress.CompletedTutorialSteps` keeps `TutorialStepId` values (the numbers are persisted, never renumber them). `PlayerProgress.TutorialInitialized` marks saves that already went through the tutorial migration: `PlayerProgressNormalizer.EnsureTutorial` runs once per save and, when the tutorial level is already completed, marks every `TutorialStepId` as done, so players from before the tutorial never see it. Steps added later are not migrated for saves that are already initialized.
- `ITutorialProgressService` (`TutorialProgressService`, project context) answers whether a step or a level is completed, completes a step and saves, and tells whether the tutorial level is completed or currently played for the first time (`IsTutorialLevelActive`).
- First launch: `InitSceneEntryPoint` asks `IFirstSceneRouter` (`FirstSceneRouter`, bound in `InitSceneInstaller`) for the first scene. Until the tutorial level is completed it selects that level and opens `GameScene` directly; afterwards it returns `BootstrapSettings.FirstScene`. Play in the Editor starts from `Init` too, so with a save where level 1 is not completed Play goes straight into the battle.
- Binding: every scene that teaches something calls `TutorialInstaller.Install(Container, view, stepTypes)` from its installer. It binds the scene's `ITutorialView`, one `ITutorialStep` per type and `TutorialDirector`. `GameSceneInstaller.BindTutorial` binds the battle steps; `MainSceneInstaller` and `LevelSelectInstaller` (on the `SceneContext` of `MainScene` and `LevelSelect`) bind the menu steps.
- `TutorialDirector` (`IInitializable`/`IDisposable`) receives every `ITutorialStep` bound in the scene. For each step that is not completed it waits for the step trigger (`WaitForTriggerAsync`), queues the step (ordered by `TutorialStepId`, queued steps start on the next frame so steps triggered together keep their order) and runs one step at a time (`RunAsync`). `true` completes and saves the step, `false` postpones it and the director waits for its trigger again. `TutorialStepScope.Battle` steps are cancelled and disposed when `GameFinishedSignal` fires; `TutorialStepScope.Scene` steps live until the scene is disposed (the result panel, the menus).
- Battle steps, level 1 (`Tutorial/Steps`):
  - `ShootTutorialStep`: `ShootDelayAfterSpawn` seconds after the first enemy spawns (`EnemySpawnedSignal`, `TutorialEnemies.WaitForSpawnedEnemyAsync`), slows the game down (`TutorialTimeScale`), dims the screen around the enemy (`WorldTutorialAnchor`, touches pass through) and shows the hold-and-drag hand until `ILeaderWeaponController.IsAttacking`. A player who already shoots completes it without a hint.
  - `ReloadTutorialStep`: on the first `IWeaponAmmoService.MagazineEmptied` points at the magazine until `ReloadCompleteRounds` rounds are back (or `ReloadMaxDuration`).
  - `BarricadeTutorialStep`: on the first `BarricadeDamagedSignal` points at the barricade health bar for `BarricadeHintDuration`.
  - `NewEnemyTutorialStep`: new-enemy notifications appear as usual, as soon as the enemy spawns. A player who opens a notification on their own (`NewEnemyNotificationSelectedSignal`) completes the step without a hint. Otherwise, on `WaveClearedSignal`, when a notification icon is still shown and another wave is still to come (however little time is left, the hint holds the countdown itself), it holds the wave countdown, shows a tap hint where only the icon can be pressed and completes when the enemy card is closed (`EnemyInfoClosedSignal`). Without a shown icon the step waits for the next wave break, in this battle or a later one.
  - `NextWaveTutorialStep`: a player who presses the next-wave button on their own (`WaveCountdownSkippedSignal`) before the hint completes the step without a hint. Otherwise, in the same wave break, it holds the countdown and shows a tap hint on the next-wave button (only the button can be pressed) until `WaveCountdownSkippedSignal`. The skip reward is granted as usual.
  - `TutorialWaveBreakGuard` (bound next to the battle steps, active only while the tutorial level is played for the first time and `NextWaveTutorialStep` is not completed) makes sure that break exists: when the countdown reaches `WaveBreakSeconds` while enemies are still on the field it holds the countdown and releases it on `WaveClearedSignal`, so the next wave waits for the player. Without it a normal player clears level 1 waves with less than `MinSecondsUntilNextWave` left and never sees these hints. It stops for good on `WaveCountdownSkippedSignal`: a player who starts the next wave on their own no longer needs the break, and the scheduler raises the signal before the new countdown is created, so the new countdown never starts held. After that the countdown is held only while a hint is shown (and, as always, stands still while a popup sets the time scale to 0), and a `NewEnemyTutorialStep` still pending shows its hint in the next natural break, whatever time is left.
  - `StarsTutorialStep` (scene scope): after a win shows a caption over the stars and is saved at once, so pressing Continue right away does not bring the hint back after the next win. The hint stays on screen until the scene closes.
- Menu steps, after the first win:
  - `UpgradeWeaponTutorialStep` (`MainScene`): once the tutorial level is completed and the player can afford the next upgrade of `UpgradeWeapon` (waits for `CoinsChangedSignal` otherwise), shows a tap hint on that weapon's "Level Up" button (only the button can be pressed) until `ProgressUpgradedSignal` reports the upgrade. A weapon that is already upgraded completes it without a hint.
  - `PlayTutorialStep` (`MainScene`) and `SelectLevelTutorialStep` (`LevelSelect`) share `TapToTransitionTutorialStep` (template method: target, caption, destination scene). Between the first win and completing the grenade level they show a tap hint on the "Select Level" button, then on the grenade level's button, and complete on `SceneTransitionStartedSignal` for `LevelSelect` / `GameScene`. `SceneTransitionService` raises that signal before the loading screen shows, so the step is saved before the scene unloads.
- Menu step, after level 2 (`StoreTourLevelId`):
  - `StoreTourTutorialStep` (`MainScene`): the first time `MainScene` opens after that level is completed, it walks through `StoreTourPages` one by one: opens the page's store tab through `IStoreTabs`, dims the screen around the page's target and shows its caption in `TutorialInputMode.TapToContinue`. On a store tab page the caption is lined up with that tab's button (`TutorialTargetKey.StoreTabButton`, passed as the hint's `CaptionAnchor`). A tap anywhere moves to the next page once `StoreTourMinPageDuration` has passed. The default pages are the store on the weapons, grenades, barricades and coins tabs, then the car and the heroes. When the tour ends, the tab that was open before it is selected again. Steps triggered together run in `TutorialStepId` order, so a pending `UpgradeWeaponTutorialStep` goes first.
  - `IStoreTabs` is bound in `MainSceneInstaller` from `StoreTabsView` (on `SafeArea/BuyPanel`), which maps each `StoreTab` to its `TabController` and selects it the same way the tab button does.
- Battle steps, from level 2:
  - `GrenadeTutorialStep`: on the grenade level it tops the gift grenade up to `GiftGrenadeCount` for free (`BuyGrenade(id, true)`, which also unlocks grenade bonus drops) while the step is not completed, so the grenade button appears in the battle. At the start of the battle, `GrenadeDelayAfterSpawn` seconds after the first enemy spawns, it slows the game down, dims the screen with two holes (the grenade slot and that enemy) and drags the hand from the slot to the enemy. It completes on `GrenadeThrownSignal` (raised by `HandController` after a grenade is placed) or after `GrenadeHintMaxDuration`; if the enemy dies first it waits for the next spawned enemy. A player who throws before the hint completes it without a hint.
  - `BonusTutorialStep`: on the first `BonusSpawnedSignal` (raised by `LevelSpawnService`) shows a tap hint on the bonus (no dim, no pause) until it starts flying (`BonusController.IsCollecting`), then points at the weapon or grenade slot it was added to for `BonusSlotHintDuration`. Bonuses drop only for owned weapons and grenades, so this happens on level 2 at the earliest.
  - `SwitchWeaponTutorialStep`: once enemies spawn and a weapon slot other than the current weapon is shown (a bought weapon with ammo), shows a tap hint on that slot until the weapon changes (`ILeaderWeaponSelectionService.CurrentWeapon`) or `SwitchWeaponHintDuration` passes.
- `TutorialTimeScale` sets the slow-motion scale and restores the previous one only if nobody changed `Time.timeScale` meanwhile, so it never fights the enemy card, the settings popup or the result panel pauses.
- On the tutorial level, while it is not completed, `GameSceneInstaller` binds `TutorialBarricadeDamagePolicy` as `IBarricadeDamagePolicy`: `BarricadeController.TakeDamage` never takes the barricade below `BarricadeMinHealth`, so the first battle cannot be lost. Every other level uses `DefaultBarricadeDamagePolicy`.
- Wave countdown hold: `ILevelWaveControlService.HoldCountdown()` returns a handle; while any handle is alive `LevelWaveScheduler` pauses its countdown tweens (also tweens created during the hold) and resumes them from the same point on release. `LevelController` raises `WaveCountdownHeldSignal`/`WaveCountdownReleasedSignal`, and `WaveCountdownIndicator` pauses its ring meanwhile. `SecondsUntilNextWave` exposes the remaining time.
- `LevelController` also raises `EnemySpawnedSignal` for every spawned enemy and `WaveClearedSignal` when the field is empty after a wave fully spawned and another wave is still to come.
- View: `ITutorialView` is implemented by `TutorialOverlayView` on the `Prefabs/UI/TutorialOverlay` prefab, placed as the last child of the main canvas in `GameScene`, `MainScene` and `LevelSelect` (nested canvas, sorting order 10100, above the result panels). One `RawImage` with the `UI/TutorialSpotlight` shader (`Assets/Project/Shaders/UI`) draws the dim, up to two rounded holes (the target and the optional `TutorialHint.Destination`) and the pulsing yellow frames. `TutorialHoleRaycastFilter` lets raycasts through the target hole only; in `TutorialInputMode.TapToContinue` the overlay blocks every raycast, including the hole, and adds a smaller "Tap to continue" line to the caption; in `TutorialInputMode.PassThrough` the overlay does not block raycasts at all, which keeps EasyTouch shooting and grenade dragging working under the dim (EasyTouch ignores touches over raycast-target UI). `TutorialPointerView` animates the hand (`Sprites/UI/TutorialPointer.png`, pivot on the fingertip) for the `Tap`, `HoldAndDrag` and `Point` gestures; `HoldAndDrag` drags to the destination when the hint has one, and the hand points down at targets in the lower part of the screen. The caption plate (`Sprites/UI/TutorialCaptionPlate.png`, 9-sliced) picks the side of the target that keeps it off both holes with the least overlap with the hand. With a `TutorialHint.CaptionAnchor` the caption is centered on that anchor instead of the target: horizontally above or below the hole, vertically to its left or right. All overlay animations use unscaled time. `ShowUntilAsync` / `ShowForAsync` (`TutorialViewExtensions`) show a hint until a task or a delay ends and always hide it. `ITutorialView.WaitForTapAsync` completes on the next click on the overlay.
- Targets: `TutorialTargetBase` (a `TutorialTargetId` and an index) registers itself in `ITutorialTargetRegistry` (project context) under its `TutorialTargetKey` while it is active and is itself the `ITutorialAnchor` that steps highlight. The index tells apart targets of the same kind: the weapon or grenade id for slots and upgrade buttons, the level id for level buttons; keys without an index convert implicitly from `TutorialTargetId`. `TutorialTarget` highlights a UI `RectTransform` (with an optional "fit active children" bounds mode); `WorldTutorialTarget` highlights a world-space box of `_size` around its position, seen through its camera (the gizmo shows the box). In `GameScene`: `LeftColumn/Reload` (Magazine, fits the active rounds), `HealthBar/Slider/Background` (BarricadeHealth), `BottomHud/NextWaveButton`, `Win/Panel/Stars` (WinStars), `WeaponsPanel/Element/Panel/BG` (WeaponSlot) and `Grenades/Element/Panel/BG` (GrenadeSlot); the `NewEnemyNotificationElement` prefab carries `NewEnemyIcon`. In `MainScene`: every weapon store element's `UnLockElements/BuyUpgrade/Bttn` (WeaponUpgradeButton), `SafeArea/Button` (PlayButton), `SafeArea/BuyPanel/TutorialHighlight` (Store, stretched over the panel and widened to the left to cover the tab buttons), every `SafeArea/BuyPanel/<tab>/Gun` tab button (StoreTabButton, index = `StoreTab`) and `[Main Players]/ShowroomTutorialTarget` (Showroom, a `WorldTutorialTarget` around the car and the heroes). In `LevelSelect`: every `BttnSelectLvl` (LevelButton); its index must match the level that `LevelChapterPanelView` binds to the button, so update it when levels are reordered.
- `SignalBus.WaitAsync<TSignal>` (`SignalBusAsyncExtensions`) awaits a signal as a `UniTask`; `DisposableAction` turns a release callback into an `IDisposable` handle.
- Editor: `Tools/Player Progress/Open Window` has a "Reset Tutorial" button that clears the completed steps of the loaded progress (press Save to write it).
- A new step is a new `ITutorialStep` class plus a `TutorialStepId` value and its type in the `TutorialInstaller.Install` call of the scene where it runs.
