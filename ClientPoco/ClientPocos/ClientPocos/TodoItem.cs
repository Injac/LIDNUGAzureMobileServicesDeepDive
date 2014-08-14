using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ClientPocos {
    public class TodoItem {
        public string Id {
            get;
            set;
        }

        [JsonProperty(PropertyName = "text")]
        public string Text {
            get;
            set;
        }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete {
            get;
            set;
        }
    }
}
