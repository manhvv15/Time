﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChamCong365.OOP.NhanVien.DonDeXuat
{
    public class CaLamViecById
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class DataAPI_CaLamViecById
        {
            public bool result { get; set; }
            public string message { get; set; }
            public Shift shift { get; set; }
        }

        public class API_CaLamViecById
        {
            public DataAPI_CaLamViecById data { get; set; }
            public object error { get; set; }
        }

        public class Shift
        {
            public string _id { get; set; }
            public int shift_id { get; set; }
            public int com_id { get; set; }
            public string shift_name { get; set; }
            public string start_time { get; set; }
            public object start_time_latest { get; set; }
            public string end_time { get; set; }
            public object end_time_earliest { get; set; }
            public DateTime create_time { get; set; }
            public int over_night { get; set; }
            public int shift_type { get; set; }
            public int num_to_calculate { get; set; }
            public int num_to_money { get; set; }
            public int is_overtime { get; set; }
            public int status { get; set; }
        }


    }
}
