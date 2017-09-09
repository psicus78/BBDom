using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BBDom.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class KnxGroup
    {
        [Key]
        public string Address { get; set; }
        public string Name { get; set; }
        public KnxDPTEnum? DPT { get; set; }
        public KnxGroupDirection Direction { get; set; }
    }

    public enum KnxGroupDirection
    {
        /// <summary>
        /// Read only variable
        /// </summary>
        OUTPUT = 0,

        /// <summary>
        /// Writable variable
        /// </summary>
        INPUT = 1,

        /// <summary>
        /// Input-output variable
        /// </summary>
        INPUT_OUTPUT = 2,
    }

    public enum KnxDPTEnum
    {
        PERCENTAGE = 0,
        TEMPERATURE = 1,
        SWITCH = 2, //true=On false=Off
        UP_DOWN = 3, //true=Up false=Down
        OPEN_CLOSE = 4, //true=Open false=Close
        COUNTER_PULSES = 5, //0..255
        DIMMING_CONTROL = 6, //-7..7
    }

    public static class KnxDPT
    {
        private static List<KeyValuePair<KnxDPTEnum, string>> TypesList = new List<KeyValuePair<KnxDPTEnum, string>>
        {
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.PERCENTAGE, "5.001"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.TEMPERATURE, "9.001"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.SWITCH, "1.001"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.UP_DOWN, "1.008"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.OPEN_CLOSE, "1.009"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.COUNTER_PULSES, "5.010"),
            new KeyValuePair<KnxDPTEnum, string>(KnxDPTEnum.DIMMING_CONTROL, "3.007"),
        };

        public static Dictionary<KnxDPTEnum, string> KnxDPTs = new Dictionary<KnxDPTEnum, string>(TypesList);
        
    }

}
