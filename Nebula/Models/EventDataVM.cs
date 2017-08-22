using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class EventDataVM
    {
        public EventDataVM()
        {
            id = string.Empty;
            title = string.Empty;
            className = string.Empty;
            desc = string.Empty;
            start = string.Empty;
            end = string.Empty;
        }

        public string id { set; get; }
        public string title { set; get; }
        public string className { set; get; }
        public string desc { set; get; }
        public string start { set; get; }
        public string end { set; get; }
    }
}