﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DnevnikTroskova
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DnevnikTroskovaEntities : DbContext
    {
        public DnevnikTroskovaEntities()
            : base("name=DnevnikTroskovaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Kategorija> Kategorija { get; set; }
        public virtual DbSet<Potkategorija> Potkategorija { get; set; }
        public virtual DbSet<Unos> Unos { get; set; }
    }
}
