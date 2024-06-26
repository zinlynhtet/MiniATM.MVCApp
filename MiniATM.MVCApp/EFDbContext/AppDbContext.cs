﻿using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.Models;

namespace MiniATM.MVCApp.EFDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<UserDataModel> UserData { get; set; }
        public DbSet<AdminDataModel> AdminData { get; set; }
        public DbSet<TransactionHistoryModel> TransactionHistory { get; set; }
    }

}
