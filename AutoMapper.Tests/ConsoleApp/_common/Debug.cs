using System;


namespace ConsoleApp._common
{
    public class Debug
    {
        public static void Show(object pObject)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(pObject) + "\n\n");

        }
    }
}
