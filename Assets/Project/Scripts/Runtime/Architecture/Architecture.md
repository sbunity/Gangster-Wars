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
