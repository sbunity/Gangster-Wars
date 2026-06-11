# Gangster Wars — Refactoring & Bug Fix

## Project overview
2D Unity game (Plants vs. Zombies style) at d:\UnityWork\Gangster-Wars.
The project is mid-refactoring to a clean Zenject DI + service-based architecture.
You have Unity MCP tools available — use them actively (script-read, script-update-or-create, 
assets-get-data, assets-modify, console-get-logs, screenshot-game-view, scene-get-data, etc.).

Tech stack: Unity 2D, Zenject (DI + SignalBus), UniTask (async), DOTween (tweens), Spine (character anims).
Root namespace: SBabchuk / SBabchuk.Runtime.*

---

## PHASE 1 — Fix current bugs (do these first)

### Bug 1 — ZenjectException: IAimService (runtime crash when shooting)
Error: "Unable to resolve 'IAimService' while building object with type 'BulletController'"

Root cause: BaseBulletController (Assets/Project/Scripts/Bullets/BaseBulletController.cs)
has a mandatory [Inject] requiring IAimService. In GameSceneInstaller.cs IAimService is bound
only when _sightController != null in the Inspector. When it is null the binding is skipped
but the injection is still mandatory, causing a crash.

### Bug 2 — Background and car not loading on GameScene
Investigate: LevelController.Start() calls Init(pPrefs.PlayerPrefs.levelID) which does
background.sprite = properties.ico. One or more of these is null or unassigned:
- background field not assigned in the Inspector on the LevelController scene object
- AssetProvider not correctly loading LevelDatabase (check AssetProvider.cs load path)
- The car object (likely barricade or a character prefab) not instantiating because
  its prefab/pool entry is broken after refactoring

Use assets-get-data and scene-get-data to inspect the GameScene and LevelController object.
Check console-get-logs for null reference exceptions at scene start.
Fix whatever is missing (re-assign references, fix asset loading paths).

### Bug 3 — IDE errors: DiContainer / Container not found (CS0246, CS0103)
Unity compiles fine but VS/Rider shows errors in:
- GameFactory.cs — DiContainer not found
- ProjectInstaller.cs — Container does not exist

Root cause: Assembly definition (.asmdef) files under Assets/Project/Scripts/Runtime/
are missing a reference to the Zenject assembly.

Fix: Find all .asmdef files under Assets/Project/Scripts/Runtime/ using assets-find
(filter: t:AssemblyDefinitionAsset). For each one that contains runtime scripts using Zenject,
add the Zenject assembly name to the references array.
Alternatively in GameFactory.cs replace DiContainer with IInstantiator (which is injectable
and avoids the assembly reference issue entirely).

---

## PHASE 2 — Complete the refactoring

### Architecture rules (non-negotiable)
- No static fields or static classes anywhere
- No magic numbers — named constants or ScriptableObject values
- No business logic in MonoBehaviours — MonoBehaviours are Views only
- All code in English (names, constants, headers, any comments)
- Minimal comments — only for non-obvious WHY, never WHAT
- ScriptableObjects: do NOT change structure or content
- Async: UniTask only (Cysharp.Threading.Tasks)
- Tweens/animations: DOTween only (Spine skeleton animations stay as-is)

### Code style
- var everywhere possible
- Private fields: _camelCase (e.g. _currentHealth)
- Properties: PascalCase (e.g. CurrentHealth)
- Local variables and method parameters: camelCase (e.g. int amount)
- [SerializeField] for inspector-assigned fields instead of public
- Encapsulate: expose only what is needed

### Service layer — verify all exist, fix what is missing or broken

All interfaces in: Assets/Project/Scripts/Runtime/Services/Contracts/
All implementations in: Assets/Project/Scripts/Runtime/Services/

Required services:

IAssetProvider / AssetProvider — Project context
ISaveService / SaveService — Project context
IPlayerProgressService / PlayerProgressService — Project context
ISceneLoaderService / SceneLoaderService — Project context
ILevelService / LevelService — Project context
IDamageService / DamageService — Project context
ICombatService / CombatService — Project context
IAudioSettingsService / AudioSettingsService — Project context
IInputService / InputService — Project context
IPoolService / PoolService — Scene context
IGameFactory / GameFactory — Scene context
IWaveSpawnerService / WaveSpawnerService — Scene context
IAimService / SightController or NullAimService fallback — Scene context
ILevelRuntimeService / LevelController — Scene context
ILeaderWeaponController / LeaderGangsterController — Scene context
IHandService / HandController — Scene context

### Zenject context bindings

ProjectInstaller (global): IAssetProvider, ISaveService, IPlayerProgressService,
ISceneLoaderService, ILevelService, IDamageService, ICombatService,
IAudioSettingsService, IInputService + all signal declarations.

GameSceneInstaller (per scene): IPoolService, IGameFactory, IWaveSpawnerService,
all scene MonoBehaviour instances (LevelController, BarricadeController,
LeaderGangsterController, HandController, SightController) + IAimService (ALWAYS bound,
never conditionally skipped).

Injection rules:
- Если инжектим в MonoBehaviour, обязательно использовать метод:
  `[Inject] private void Construct(...)`
- Не использовать field/property injection в MonoBehaviour.
- Если класс не MonoBehaviour, использовать constructor injection.
- `[InjectOptional]` использовать только для осознанной совместимости на коротком migration step; к финалу убрать там, где dependency обязательна.
- Не маскировать ошибки отсутствующих bindings через optional injection.

### GameFactory
Must be a plain C# class (not MonoBehaviour) implementing IGameFactory.
Use IInstantiator (injectable Zenject interface, preferred over raw DiContainer)
to call InjectGameObject() on pooled objects after retrieval.
All object creation goes through GameFactory: enemies, bullets, grenades,
collisions, bonuses.

### Composition-based GameObjects

Enemy prefabs must be composed of these MonoBehaviour components
(already partially exists in Assets/Project/Scripts/Runtime/Gameplay/Enemies/):
- EnemyView — Spine animation, sprite sorting, visual state only
- EnemyHealth — HP tracking, exposes C# events OnChanged(float normalized) and OnDied()
- EnemyMovement — DOTween-based movement toward target, range checking
- EnemyAttack — attack timing and damage delegation to IDamageService
- EnemyDeath — death animation trigger, pool return after animation completes
- EnemyReward — coin/bonus granting via ICombatService on death

Configuration of all components happens in GameFactory.CreateEnemy() after
InjectGameObject() is called.

Character prefabs composed of (already partially exists in Runtime/Gameplay/Characters/):
- CharacterView — Spine animation, visual state
- CharacterAttack — attack state machine
- CharacterAmmo — magazine/ammo tracking
- CharacterWeapon — bullet spawning via IGameFactory

Legacy monolithic controllers in Assets/Project/Scripts/Enemies/ and
Assets/Project/Scripts/Main Player/ should be migrated to this composition pattern.
The old EnemyControllerBase can remain as a bridge adapter until full migration,
but all new logic goes into the component scripts.

### UI
UI MonoBehaviours should:
1. Inject services and SignalBus via [Inject]
2. Subscribe to signals for data updates (never poll, never FindObjectOfType)
3. Call service methods for user actions
4. Contain zero business logic

### Pool service
IPoolService / PoolService — keep existing implementation, minimal changes.
Only modify if broken after other refactoring.

### Save service
ISaveService / SaveService — keep existing save format, minimal changes.
Interface must exist so the implementation can be swapped in the future.

---

## PHASE 3 — Cleanup

After all functionality works and is verified:
1. Delete any fully-migrated legacy scripts that are no longer referenced
2. Organize folder structure:

Assets/Project/Scripts/
  Runtime/
    Architecture/         GameSignals.cs
    Factories/            GameFactory.cs
    Gameplay/
      Enemies/            composition components
      Characters/         composition components
      Projectiles/
      Grenades/
      Bonuses/
      Collisions/
    Installers/           ProjectInstaller.cs, GameSceneInstaller.cs
    Services/
      Contracts/          all interfaces
    UI/                   all UI scripts
  Databases/              ScriptableObjects, untouched

3. Remove any remaining Ukrainian-language comments; rename Ukrainian identifiers to English
4. Remove commented-out code blocks

---

## Workflow instructions

- After fixing each bug: use console-get-logs to verify no new errors
- After significant script changes: use screenshot-game-view to visually verify
- Do NOT break existing game mechanics — all gameplay must work as before
- Use script-read to inspect any file before modifying
- Use assets-get-data to inspect prefabs and scenes before modifying
- Use console-clear-logs before testing to get clean output
- If Unity shows compile errors after a change, fix them immediately before continuing

Order of work:
1. Bug 1 (IAimService crash) — blocks gameplay testing, fix first
2. Bug 2 (background and car not loading) — fix and verify with screenshot
3. Bug 3 (IDE assembly definition errors) — fix .asmdef references
4. Phase 2 composition migration — enemy and character components
5. Phase 2 service layer completion — verify all bindings
6. Phase 2 UI refactoring
7. Phase 3 cleanup and folder reorganization
