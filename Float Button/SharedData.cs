using Xamarin.Essentials;

namespace Float_Button
{
    class SharedData
    {
        public SharedData()
        {
            if(!Preferences.ContainsKey("characters", null))
            {
                Preferences.Set("characters", "-=[  ]=-");
                Preferences.Set("cursor_index", "4");

            }
        }
        public string GetData(string name)
        {
            string data = (Preferences.ContainsKey(name)) ? Preferences.Get(name, null) : null;
            return data;
        }
        public void SetData(string name, string value)
        {
            Preferences.Set(name, value);
        }
    }
}