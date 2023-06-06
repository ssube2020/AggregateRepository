using System;
namespace Core.Entities
{
    public class ElectricityInfo
    {
        public int Id { get; set; }
        public string Network { get; set; }
        public double? P_Plus { get; set; }
        public double? P_Minus { get; set; }
    }
}

