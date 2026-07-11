using BB.Core;
using NUnit.Framework;

namespace BB.Tests
{
    public class AwardLedgerTests
    {
        static LedgerEntry Entry(string type, string id = null, long amount = 0)
            => new() { type = type, id = id, amount = amount, timestampUnix = 1750000000, matchId = "m1", mode = "brawl" };

        [Test]
        public void Fold_SumsCaps()
        {
            var ledger = new AwardLedger();
            ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 50));
            ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 25));
            ledger.Append(Entry(LedgerEventTypes.CapsSpent, amount: 30));
            Assert.AreEqual(45, ledger.Fold().Caps);
        }

        [Test]
        public void Fold_CosmeticPurchaseDeductsAndOwns()
        {
            var ledger = new AwardLedger();
            ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 100));
            ledger.Append(Entry(LedgerEventTypes.CosmeticPurchased, id: "hat.trafficCone", amount: 60));
            var wallet = ledger.Fold();
            Assert.AreEqual(40, wallet.Caps);
            Assert.IsTrue(wallet.Cosmetics.Contains("hat.trafficCone"));
        }

        [Test]
        public void Fold_TracksStatsBragsAndShinies()
        {
            var ledger = new AwardLedger();
            ledger.Append(Entry(LedgerEventTypes.StatIncremented, id: "wins", amount: 1));
            ledger.Append(Entry(LedgerEventTypes.StatIncremented, id: "wins", amount: 1));
            ledger.Append(Entry(LedgerEventTypes.BragUnlocked, id: "brag.firstWin"));
            ledger.Append(Entry(LedgerEventTypes.ShinyEarned, id: "shiny.luckyPenny", amount: 1));

            var wallet = ledger.Fold();
            Assert.AreEqual(2, wallet.Stats["wins"]);
            Assert.IsTrue(wallet.Brags.Contains("brag.firstWin"));
            CollectionAssert.Contains(wallet.Shinies, "shiny.luckyPenny");
        }

        [Test]
        public void Checksum_ChangesWhenLedgerChanges()
        {
            var ledger = new AwardLedger();
            ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 10));
            string before = ledger.ComputeChecksum();
            ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 10));
            Assert.AreNotEqual(before, ledger.ComputeChecksum());
        }

        [Test]
        public void SaveLoad_RoundTrips()
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "bb_test_shoebox.json");
            try
            {
                var ledger = new AwardLedger { profileId = "test" };
                ledger.Append(Entry(LedgerEventTypes.CapsEarned, amount: 77));
                ledger.Append(Entry(LedgerEventTypes.BragUnlocked, id: "brag.firstWin"));
                SaveSystem.Save(ledger, path);

                var loaded = SaveSystem.LoadOrCreate("other", path);
                Assert.AreEqual("test", loaded.profileId);
                Assert.AreEqual(2, loaded.entries.Count);
                Assert.AreEqual(77, loaded.Fold().Caps);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Test]
        public void LoadOrCreate_ReturnsFreshOnMissingFile()
        {
            var ledger = SaveSystem.LoadOrCreate("fresh", "/nonexistent/dir/nope.json");
            Assert.AreEqual("fresh", ledger.profileId);
            Assert.AreEqual(0, ledger.entries.Count);
        }
    }
}
