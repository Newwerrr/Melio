using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melio.Classes
{
    public class Debug
    {
        public static void DumpTypes()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name.ToLower().Contains("rope"))
                    {
                        MelonLogger.Msg($"[DEBUG] {type.FullName}");
                    }
                }
            }
        }
    }
}