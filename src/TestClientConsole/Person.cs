using System;
using Cassandra;
using Cassandra.Mapping.Attributes;

namespace TestClientConsole
 {
     [Table("Persons", Keyspace = "driver_test", CaseSensitive = false)]
     public class Person
     {
         [PartitionKey, Column("user_id")]
 
         public Guid UserId { get; set; }
 
         [Column("name")]
         public string Name { get; set; }
 
         [Column("age")]
         public int Age { get; set; }
     }
 }