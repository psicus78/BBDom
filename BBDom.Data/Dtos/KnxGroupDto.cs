﻿using BBDom.Data.Models;

namespace BBDom.Data.Dtos
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class KnxGroupWithStateDto
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public KnxDPTEnum? DPT { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public double? State { get; set; }
    }
    
}
