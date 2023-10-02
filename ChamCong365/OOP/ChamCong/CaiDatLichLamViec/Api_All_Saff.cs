﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChamCong365.OOP.ChamCong.CaiDatLichLamViec
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DataAllSaff
    {
        public bool result { get; set; }
        public string message { get; set; }
        public int totalItems { get; set; }
        public List<ItemAllSaff> items { get; set; }
    }

    public class ItemAllSaff
    {
        public int _id { get; set; }
        public int ep_id { get; set; }
        public string ep_email { get; set; }
        public string ep_email_lh { get; set; }
        public string ep_phone_tk { get; set; }
        public string ep_name { get; set; }
        public int? ep_education { get; set; }
        public int? ep_exp { get; set; }
        public string ep_phone { get; set; }
        public string ep_image { get; set; }
        public string ep_address { get; set; }
        public int ep_gender { get; set; }
        public int ep_married { get; set; }
        public int? ep_birth_day { get; set; }
        public string ep_description { get; set; }
        public int create_time { get; set; }
        public int role_id { get; set; }
        public int? group_id { get; set; }
        public string start_working_time { get; set; }
        public int position_id { get; set; }
        public string ep_status { get; set; }
        public int allow_update_face { get; set; }
        public int com_id { get; set; }
        public int dep_id { get; set; }
        public string nameDeparment { get; set; }
        public string gr_name { get; set; }
    }

    public class RootAllSaff
    {
        public DataAllSaff data { get; set; }
        public object error { get; set; }
    }


}
