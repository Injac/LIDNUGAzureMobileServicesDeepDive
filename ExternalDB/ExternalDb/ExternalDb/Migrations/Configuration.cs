namespace ExternalDb.Migrations {
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ExternalDb.ExternalDatabase> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ExternalDb.ExternalDatabase context) {
            //Add some default data
            IList<Customer> customers = new List<Customer>();

            customers.Add(new Customer() {
                FirstName = "Mike", LastName = "Meyers"
            });
            customers.Add(new Customer() {
                FirstName = "John", LastName = "Doe"
            });
            customers.Add(new Customer() {
                FirstName = "Jack", LastName = "Black"
            });
            customers.Add(new Customer() {
                FirstName = "Marlyn", LastName = "Manson"
            });

            foreach (var cst in customers) {
                context.Customers.AddOrUpdate(cst);

            }
        }
    }
}
