using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KeepToEvernote
{
    internal class KeepNote
    {
        public string? color { get; set; }
        public bool isTrashed { get; set; }
        public bool isPinned { get; set; }
        public bool isArchived { get; set; }
        public string? textContent { get; set; }
        public List<Dictionary<string, object>> listContent { get; set; }
        public string? title { get; set; }
        public long userEditedTimestampUsec { get; set; }
        public long createdTimestampUsec { get; set; }
        public List<Dictionary<string, string>>? labels { get; set; }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                          DateTimeKind.Utc);

        // Keep uses Usec since Unix Epoch. We convert to a .net DateTime here.
        public static DateTime FromUsecSinceUnixEpoch(long Usec)
        {
            var milliseconds = Usec / 1000;
            return UnixEpoch + TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
