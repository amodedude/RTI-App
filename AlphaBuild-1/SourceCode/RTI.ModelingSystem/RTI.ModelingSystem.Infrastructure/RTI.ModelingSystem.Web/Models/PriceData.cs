using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RTI.ModelingSystem.Web.Models
{
    public class PriceData
    {
        public Dictionary<DateTime, Tuple<int, double, string>> cleanThroughput { get; set; }
        public Dictionary<DateTime, Tuple<int, double, string>> normOpsThroughput { get; set; }
        public List<Tuple<int, double>> regensPerWeekClean { get; set; }
        public List<Tuple<int, double>> regensPerWeekNormOps { get; set; }
        public DataTable trainList { get; set; }
        public int number_trains { get; set; }
        public double number_regens { get; set; }
        public int acid_price { get; set; }
        public int caustic_price { get; set; }
        public Dictionary<int, double> grain_Forcast { get; set; }
        public Dictionary<int, Tuple<double?, double?>> saltSplit { get; set; }
        public double amountCation { get; set; }
        public double aomuntAnion { get; set; }
        public double lbsAcid { get; set; }
        public double lbsCaustic { get; set; }
    }
}