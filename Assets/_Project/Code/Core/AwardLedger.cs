using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace BB.Core
{
    /// <summary>Kinds of reward events. String-stable: these go in save files.</summary>
    public static class LedgerEventTypes
    {
        public const string CapsEarned = "CapsEarned";
        public const string CapsSpent = "CapsSpent";
        public const string ShinyEarned = "ShinyEarned";
        public const string BragUnlocked = "BragUnlocked";
        public const string CosmeticPurchased = "CosmeticPurchased";
        public const string XpGained = "XpGained";
        public const string StatIncremented = "Stat";
    }

    [Serializable]
    public struct LedgerEntry
    {
        public string type;
        /// <summary>Brag id, shiny id, cosmetic id, or stat key — depending on type.</summary>
        public string id;
        public long amount;
        public long timestampUnix;
        public string matchId;
        /// <summary>"brawl", "rumble", or "recess".</summary>
        public string mode;
    }

    /// <summary>Current profile totals — always DERIVED by folding the ledger, never stored.</summary>
    public sealed class Wallet
    {
        public long Caps;
        public long Xp;
        public readonly List<string> Shinies = new();
        public readonly HashSet<string> Brags = new();
        public readonly HashSet<string> Cosmetics = new();
        public readonly Dictionary<string, long> Stats = new();
    }

    /// <summary>
    /// The Shoebox's spine: an append-only event ledger. Every mode earns
    /// through events; wallet/rank are folds over the ledger. Server sync
    /// later = replay + validate, no save migration. Timestamps are always
    /// injected by callers — nothing in here reads the clock.
    /// </summary>
    [Serializable]
    public class AwardLedger
    {
        public int version = 1;
        public string profileId;
        public List<LedgerEntry> entries = new();

        public void Append(in LedgerEntry entry) => entries.Add(entry);

        public Wallet Fold()
        {
            var w = new Wallet();
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                switch (e.type)
                {
                    case LedgerEventTypes.CapsEarned: w.Caps += e.amount; break;
                    case LedgerEventTypes.CapsSpent: w.Caps -= e.amount; break;
                    case LedgerEventTypes.XpGained: w.Xp += e.amount; break;
                    case LedgerEventTypes.ShinyEarned: w.Shinies.Add(e.id); break;
                    case LedgerEventTypes.BragUnlocked: w.Brags.Add(e.id); break;
                    case LedgerEventTypes.CosmeticPurchased:
                        w.Cosmetics.Add(e.id);
                        w.Caps -= e.amount;
                        break;
                    case LedgerEventTypes.StatIncremented:
                        w.Stats.TryGetValue(e.id, out long v);
                        w.Stats[e.id] = v + e.amount;
                        break;
                }
            }
            return w;
        }

        public string ComputeChecksum()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                sb.Append(e.type).Append('|').Append(e.id).Append('|').Append(e.amount)
                  .Append('|').Append(e.timestampUnix).Append('|').Append(e.matchId)
                  .Append('|').Append(e.mode).Append('\n');
            }
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>Versioned JSON persistence for the profile at persistentDataPath.</summary>
    public static class SaveSystem
    {
        [Serializable]
        class Envelope
        {
            public int saveVersion = 1;
            public string checksum;
            public AwardLedger ledger;
        }

        public static string DefaultPath =>
            System.IO.Path.Combine(Application.persistentDataPath, "shoebox.json");

        public static void Save(AwardLedger ledger, string path = null)
        {
            var env = new Envelope { ledger = ledger, checksum = ledger.ComputeChecksum() };
            System.IO.File.WriteAllText(path ?? DefaultPath, JsonUtility.ToJson(env, prettyPrint: true));
        }

        /// <summary>Loads the profile; returns a fresh ledger if absent or corrupt (never throws at boot).</summary>
        public static AwardLedger LoadOrCreate(string profileId, string path = null)
        {
            string p = path ?? DefaultPath;
            try
            {
                if (System.IO.File.Exists(p))
                {
                    var env = JsonUtility.FromJson<Envelope>(System.IO.File.ReadAllText(p));
                    if (env?.ledger != null)
                    {
                        if (env.checksum != env.ledger.ComputeChecksum())
                            Debug.LogWarning("Shoebox checksum mismatch — save was edited or corrupted. Loading anyway.");
                        return env.ledger;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load Shoebox save, starting fresh: {ex.Message}");
            }
            return new AwardLedger { profileId = profileId };
        }
    }
}
