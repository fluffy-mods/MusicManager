// MusicDatabase.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using Verse;

namespace MusicManager {
    public class SongDatabase: IExposable {
        private Dictionary<SongDef, SongMetaData> _database = new Dictionary<SongDef, SongMetaData>();
        private List<SongMetaData> _databaseWorkingList = new List<SongMetaData>();

        public SongMetaData this[SongDef song] {
            get {
                if (_database.TryGetValue(song, out SongMetaData meta)) {
                    return meta;
                }

                _database.Add(song, meta = new SongMetaData(song));
                return meta;
            }
        }

        public static string SongDatabasePath => Path.Combine(GenFilePaths.ConfigFolderPath, "SongDatabase.xml");
        public const string RootElement = "SongDatabase";

        public void Initialize() {
            // load stored data
            if (File.Exists(SongDatabasePath)) {
                try {
                    Scribe.loader.InitLoading(SongDatabasePath);
                    ExposeData();
                } catch (Exception ex) {
                    Verse.Log.Error($"Exception loading song metadata database:\n{ex}");
                } finally {
                    Scribe.loader.FinalizeLoading();
                }

                try {
                    Scribe.loader.crossRefs.ResolveAllCrossReferences();
                } catch (Exception ex) {
                    Verse.Log.Error($"Exception resolving references for song metadata database:\n{ex}");
                }

                try {
                    Scribe.loader.initer.DoAllPostLoadInits();
                } catch (Exception ex) {
                    Verse.Log.Error($"Exception initializing song metadata database:\n{ex}");
                }
            }

            // create containers for all other songs
            foreach (SongDef song in DefDatabase<SongDef>.AllDefsListForReading) {
                if (_database.ContainsKey(song)) {
                    continue;
                }

                _database.Add(song, new SongMetaData(song));
            }
        }

        public void WriteMetaData() {
            try {
                Log.Debug("writing song database");
                Scribe.saver.InitSaving(SongDatabasePath, RootElement);
                ExposeData();
            } catch (Exception ex) {
                Verse.Log.Error($"Exception writing song metadata database:\n{ex}");
            } finally {
                Scribe.saver.FinalizeSaving();
            }
        }

        public List<SongMetaData> PrunedDatabase() {
            int pre = _database.Count;
            List<SongMetaData> _custom = _database.Where( ( p ) => p.Value.HasCustomMetaData ).Select( p => p.Value ).ToList();
            Log.Debug($"pruning song database, pre: {pre}, post: {_database.Count}");
            return _custom;
        }

        public void ResetCustomMetaData() {
            foreach (KeyValuePair<SongDef, SongMetaData> meta in _database) {
                meta.Value.ApplyOriginalMetaData();
            }

            WriteMetaData();
        }

        public void ExposeData() {
            if (Scribe.mode == LoadSaveMode.Saving) {
                _databaseWorkingList = PrunedDatabase();
            }

            Scribe_Collections.Look(ref _databaseWorkingList, "Database", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                _database = _databaseWorkingList.ToDictionary(m => m.song, m => m);
            }
        }
    }

    public static class SongDef_Extensions {
        public static bool Enabled(this SongDef song) {
            return !MusicManager.SongDatabase[song].disabled;
        }
    }

    public class SongMetaData: IExposable {
        public SongDef   song;
        public List<Season>    seasons;
        public bool      tense;
        public TimeOfDay time;
        public bool      disabled;

        private SongMetaData original;

        public SongMetaData(SongDef song) {
            this.song = song;
            NoteOriginal(song);
        }

        public SongMetaData() {
            // scribe;
        }

        public void NoteOriginal(SongDef song) {
            original = new SongMetaData {
                seasons = song.allowedSeasons == null ? null : new List<Season>(song.allowedSeasons),
                time = song.allowedTimeOfDay,
                tense = song.tense
            };
        }

        public void ApplyCustomMetaData() {
            song.allowedSeasons = seasons == null ? null : new List<Season>(seasons);
            song.allowedTimeOfDay = time;
            song.tense = tense;
        }

        public void ApplyOriginalMetaData() {
            song.allowedSeasons = original.seasons == null ? null : new List<Season>(original.seasons);
            song.allowedTimeOfDay = original.time;
            song.tense = original.tense;
            disabled = false;
        }

        public bool HasCustomMetaData {
            get {
                GetCurrentMetaData();
                if (disabled) {
                    return true;
                }

                if (tense != original.tense) {
                    return true;
                }

                return time != original.time
                    ? true
                    : seasons?.Count != original.seasons?.Count || ((seasons != null || original.seasons != null) && (seasons.Except(original.seasons).Any() || original.seasons.Except(seasons).Any()));
            }
        }

        public void GetCurrentMetaData() {
            seasons = song.allowedSeasons == null ? null : new List<Season>(song.allowedSeasons);
            time = song.allowedTimeOfDay;
            tense = song.tense;
        }

        public void ExposeData() {
            if (Scribe.mode == LoadSaveMode.Saving) {
                GetCurrentMetaData();
            }

            Scribe_Defs.Look(ref song, "SongDef");
            Scribe_Collections.Look(ref seasons, "Seasons");
            Scribe_Values.Look(ref tense, "Tense");
            Scribe_Values.Look(ref time, "TimeOfDay");
            Scribe_Values.Look(ref disabled, "Disabled");

            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                NoteOriginal(song);
                ApplyCustomMetaData();
            }
        }
    }
}
