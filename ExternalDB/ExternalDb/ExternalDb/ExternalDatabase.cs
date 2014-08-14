namespace ExternalDb {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public partial class ExternalDatabase : DbContext {
        public ExternalDatabase()
        : base("name=ExternalDatabase") {
        }

        public virtual DbSet<Customer> Customers {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {


        }
    }
}
