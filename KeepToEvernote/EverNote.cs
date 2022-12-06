using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepToEvernote
{
    internal class EverNote
    {
        public string? title { get; set; }
        public string? created { get; set; }
        public string updated { get; set; }
        public List<string?>? tags { get; set; }
        public string? author { get; set; }
        public string? content { get; set; }
    }
}
