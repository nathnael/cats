﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cats.EasyAccess;

namespace EasyCats.ViewModels
{
    public class UserProfile : DynamicModel
    {
        //you don't have to specify the connection - Massive will use the first one it finds in your config
        public UserProfile() : base("CatsContext", "UserProfile", "UserProfileID") { }
    }
}