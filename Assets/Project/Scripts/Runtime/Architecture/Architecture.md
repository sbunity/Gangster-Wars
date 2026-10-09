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
- Buttons without the component stay silent: the in-game weapon switch buttons, the grenade drag buttons, and the store card backgrounds that do nothing on click. New buttons need the component added. UI prefabs created at runtime must be instantiated through the container (`IInstantiator`) so the component gets its dependencies.

## Game Result Sounds

`GameResultPanelController` has a `_showSound` field (`SoundConfig`) and plays it through `IAudioService` when the panel is shown. The sound plays together with the panel, so the win sound waits for the win panel's `_showDelay`. In `GameScene` the win panel uses `Assets/Resources/Sounds/Endgame/Level_Win.asset` and the lose panel uses `Level_Lose.asset`. Surrendering also shows the lose panel, so it plays the same sound.

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
