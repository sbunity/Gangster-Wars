using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressNormalizer : IPlayerProgressNormalizer
    {
        private readonly IAssetProvider _assetProvider;

        public PlayerProgressNormalizer(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public bool Normalize(PlayerProgress progress)
        {
            if (progress == null)
                return false;

            var defaults = _assetProvider.PlayerProgressDefaults;
            var changed = false;
            changed |= EnsureLists(progress);
            changed |= EnsureWeapons(progress, defaults);
            changed |= EnsureGrenades(progress);
            changed |= EnsureDefences(progress, defaults);
            changed |= EnsurePersonages(progress);
            changed |= EnsureLevels(progress, defaults);
            changed |= ClampGrenadeCounts(progress);
            return changed;
        }

        private static bool EnsureLists(PlayerProgress progress)
        {
            var changed = false;
            progress.Weapons = EnsureList(progress.Weapons, ref changed);
            progress.Grenades = EnsureList(progress.Grenades, ref changed);
            progress.Defences = EnsureList(progress.Defences, ref changed);
            progress.Personages = EnsureList(progress.Personages, ref changed);
            progress.Levels = EnsureList(progress.Levels, ref changed);
            progress.Chapters = EnsureList(progress.Chapters, ref changed);

            if (progress.SeenEnemyIds == null)
            {
                progress.SeenEnemyIds = new List<int>();
                changed = true;
            }

            return changed;
        }

        private static List<T> EnsureList<T>(List<T> entries, ref bool changed) where T : class
        {
            if (entries == null)
            {
                changed = true;
                return new List<T>();
            }

            if (entries.RemoveAll(entry => entry == null) > 0)
                changed = true;

            return entries;
        }

        private bool EnsureWeapons(PlayerProgress progress, PlayerProgressDefaults defaults)
        {
            var weapons = _assetProvider.WeaponStoreDatabase.Weapons;
            if (weapons == null)
                return false;

            var changed = false;
            foreach (var weapon in weapons)
            {
                if (weapon == null || progress.GetWeaponShortInfo(weapon.Id) != null)
                    continue;

                var isStarter = weapon.Id == defaults.StarterWeaponId;
                var ammo = isStarter ? defaults.StarterWeaponAmmo : 0;
                progress.Weapons.Add(new WeaponShortInfo(weapon.Id, weapon.Name, isStarter, ammo));
                changed = true;
            }

            return changed;
        }

        private bool EnsureGrenades(PlayerProgress progress)
        {
            var grenades = _assetProvider.BombStoreDatabase.Grenades;
            if (grenades == null)
                return false;

            var changed = false;
            foreach (var grenade in grenades)
            {
                if (grenade == null || progress.GetGrenadeShortInfo(grenade.Id) != null)
                    continue;

                progress.Grenades.Add(new GrenadeShortInfo(grenade.Id));
                changed = true;
            }

            return changed;
        }

        private bool EnsureDefences(PlayerProgress progress, PlayerProgressDefaults defaults)
        {
            var defences = _assetProvider.DefenseStoreDatabase.Defenses;
            if (defences == null)
                return false;

            var changed = false;
            foreach (var defence in defences)
            {
                if (defence == null || progress.GetDefenceShortInfo(defence.Id) != null)
                    continue;

                progress.Defences.Add(new DefenceShortInfo(defence.Id, defence.Id == defaults.StarterDefenceId));
                changed = true;
            }

            return changed;
        }

        private bool EnsurePersonages(PlayerProgress progress)
        {
            var personages = _assetProvider.MainPlayerDatabase.Personages;
            if (personages == null)
                return false;

            var changed = false;
            foreach (var personage in personages)
            {
                if (personage == null || progress.GetPersonageShortInfo(personage.Id) != null)
                    continue;

                progress.Personages.Add(new PersonageShortInfo(personage.Id));
                changed = true;
            }

            return changed;
        }

        private bool EnsureLevels(PlayerProgress progress, PlayerProgressDefaults defaults)
        {
            var levelDatabase = _assetProvider.LevelDatabase;
            if (levelDatabase == null)
                return false;

            var changed = false;
            if (levelDatabase.Chapters != null)
            {
                foreach (var chapter in levelDatabase.Chapters)
                {
                    if (chapter == null)
                        continue;

                    if (chapter.Levels != null)
                    {
                        foreach (var level in chapter.Levels)
                            changed |= EnsureLevel(progress, level, chapter.Id);
                    }

                    changed |= EnsureChapter(progress, chapter, defaults);
                }
            }

            foreach (var level in levelDatabase.Levels)
                changed |= EnsureLevel(progress, level, levelDatabase.GetChapterIdByLevelId(level.Id));

            var chapterId = levelDatabase.GetChapterIdByLevelId(progress.LevelId);
            if (progress.ChapterId != chapterId)
            {
                progress.ChapterId = chapterId;
                changed = true;
            }

            return changed;
        }

        private static bool EnsureChapter(PlayerProgress progress, ChapterDatabase chapter, PlayerProgressDefaults defaults)
        {
            var changed = false;
            var chapterShortInfo = progress.GetChapterShortInfo(chapter.Id);
            if (chapterShortInfo == null)
            {
                progress.Chapters.Add(new ChapterShortInfo(chapter.Id, chapter.Name, chapter.Id == defaults.UnlockedChapterId));
                changed = true;
            }
            else if (chapterShortInfo.Name != chapter.Name)
            {
                chapterShortInfo.Name = chapter.Name;
                changed = true;
            }

            return changed | progress.RefreshChapterCompletion(chapter.Id, chapter.GetLevelIds());
        }

        private static bool EnsureLevel(PlayerProgress progress, Level level, int chapterId)
        {
            if (level == null)
                return false;

            var levelShortInfo = progress.GetLevelShortInfo(level.Id);
            if (levelShortInfo == null)
            {
                progress.Levels.Add(new LevelShortInfo(level.Id, chapterId));
                return true;
            }

            if (levelShortInfo.ChapterId == chapterId)
                return false;

            levelShortInfo.ChapterId = chapterId;
            return true;
        }

        private static bool ClampGrenadeCounts(PlayerProgress progress)
        {
            var changed = false;
            foreach (var grenade in progress.Grenades)
            {
                if (grenade.Count >= 0)
                    continue;

                grenade.Count = 0;
                changed = true;
            }

            return changed;
        }
    }
}
