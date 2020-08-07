using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitchChatBot
{
    class CowsStatistics
    {
        private class StatisticsEntry
        {
            public string userName;
            public List<int> attemptsCounts = new List<int>();

            public override string ToString()
            {
                return $"username: {userName}, attempts count: {attemptsCounts.Count}, average count: {Average} \r";
            }

            public float Average => (float)attemptsCounts.Sum() / attemptsCounts.Count();
        }

        private Dictionary<string, StatisticsEntry> statistics = new Dictionary<string, StatisticsEntry>();

        public void UpdateStatistics(string userName, int attemptCout)
        {
            var entry = GetAddStatisticsEntry(userName);
            entry.attemptsCounts.Add(attemptCout);
        }

        public string PrintStatistics()
        {
            var list = statistics.Values.ToList().OrderBy(e => e.Average);
            StringBuilder builder = new StringBuilder();
            foreach (var entry in list)
            {
                builder.Append(entry.ToString());
            }
            return builder.ToString();
        }

        private StatisticsEntry GetAddStatisticsEntry(string userName)
        {
            if (!statistics.ContainsKey(userName))
            {
                StatisticsEntry entry = new StatisticsEntry();
                entry.userName = userName;
                statistics.Add(userName, entry);
                return entry;
            }
            else
            {
                return statistics[userName];
            }
        }


    }
}
