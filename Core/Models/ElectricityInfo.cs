using System;
namespace Core.Models
{
    public class ElectricityInfoModel
    {
        public int Id { get; set; }
        public string Network { get; set; }
        public string Obt_Name { get; set; }
        public string Obj_Gv_Type { get; set; }
        public int? Obj_Number { get; set; }
        public double? P_Plus { get; set; }
        public DateTime Pl_T { get; set; }
        public double? P_Minus { get; set; }
    }
}

