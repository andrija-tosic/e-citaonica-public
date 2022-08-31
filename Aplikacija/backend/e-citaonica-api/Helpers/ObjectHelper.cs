using Newtonsoft.Json;

namespace e_citaonica_api.Helpers
{
    static class ObjectHelper
    {
        public static void Dump<T>(this T x)
        {
            string json = JsonConvert.SerializeObject(x, Formatting.Indented);
            Console.WriteLine(x?.GetType().Name + json);
        }
    }
}
