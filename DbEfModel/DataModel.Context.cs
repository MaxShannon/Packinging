﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DbEfModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class jhglEntities : DbContext
    {
        public jhglEntities()
            : base("name=jhglEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CargoInInfoes> CargoInInfoes { get; set; }
        public virtual DbSet<CargoOutOrderInfoes> CargoOutOrderInfoes { get; set; }
        public virtual DbSet<cheh> cheh { get; set; }
        public virtual DbSet<dw> dw { get; set; }
        public virtual DbSet<huom> huom { get; set; }
        public virtual DbSet<huom2> huom2 { get; set; }
        public virtual DbSet<huot> huot { get; set; }
        public virtual DbSet<HuotHuomInfoes> HuotHuomInfoes { get; set; }
        public virtual DbSet<lx> lx { get; set; }
        public virtual DbSet<sys_ry> sys_ry { get; set; }
        public virtual DbSet<CargoView> CargoView { get; set; }
        public virtual DbSet<HuotView> HuotView { get; set; }
        public virtual DbSet<ShipmentView> ShipmentView { get; set; }
        public virtual DbSet<CargoInfoes> CargoInfoes { get; set; }
        public virtual DbSet<CargoHuotInfoes> CargoHuotInfoes { get; set; }
        public virtual DbSet<ShipmentInfoes> ShipmentInfoes { get; set; }
        public virtual DbSet<webdddl> webdddl { get; set; }
        public virtual DbSet<ShipmentHuotInfoes> ShipmentHuotInfoes { get; set; }
        public virtual DbSet<CargoLogInfoes> CargoLogInfoes { get; set; }
    }
}
