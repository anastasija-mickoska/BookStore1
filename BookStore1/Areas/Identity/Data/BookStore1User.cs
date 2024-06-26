﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore1.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore1.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookStore1User class
public class BookStore1User : IdentityUser
{
    public ICollection<UserBooks>? Books { get; set; } 
}

