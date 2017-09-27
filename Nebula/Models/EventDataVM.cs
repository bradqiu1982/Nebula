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

        public EventDataVM(string _id,string _title,string _className,string _desc,string _start,string _end)
        {
            id = _id;
            title = _title;
            className = _className;
            desc = _desc;
            start = _start;
            end = _end;
        }

        public string id { set; get; }
        public string title { set; get; }
        public string className { set; get; }
        public string desc { set; get; }
        public string start { set; get; }
        public string end { set; get; }
    }
}