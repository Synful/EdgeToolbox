using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeToolbox.Classes {
    class Settings {
        string appDir = "";
        public string email = "";
        public string pass = "";
        public bool devmode = false;

        static Settings _instance;
        public static Settings Instance() {
            return _instance;
        }

        public Settings() {}

        public Settings(string appDir) {
            this.appDir = appDir;
            this.Load();
            _instance = this;
        }

        public void Save() {
            string t = this.pass;
            this.pass = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.pass));
            File.WriteAllText($"{this.appDir}Settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            this.pass = t;
        }
        public void Load() {
            Settings temp = JsonConvert.DeserializeObject<Settings>(File.ReadAllText($"{this.appDir}Settings.json"));
            if (temp != null) {
                this.email = temp.email;
                this.pass = Encoding.UTF8.GetString(Convert.FromBase64String(temp.pass));
                this.devmode = temp.devmode;
            }
        }
    }
}
