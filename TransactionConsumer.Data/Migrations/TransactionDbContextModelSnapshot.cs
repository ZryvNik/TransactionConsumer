using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TransactionConsumer.Data;

#nullable disable

namespace TransactionConsumer.Data.Migrations
{
    [DbContext(typeof(TransactionDbContext))]
    partial class TransactionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TransactionConsumer.Data.Models.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime>("InsertDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
} 