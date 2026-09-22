using Microsoft.EntityFrameworkCore;
using BancoCore.Models;

namespace BancoCore.Data;

public class BancoDbContext : DbContext
{
    public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options){}
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();

    public DbSet<Transaccion> Transaccion => Set<Transaccion>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cuenta>().HasData(
            new Cuenta
            {
                Id = 1,
                NumeroCuenta = "PE-001-987654",
                Saldo = 2500.00m,
                Titular = "Rodrigo",
                Moneda = "PEN"
            },
            new Cuenta
            {
                Id = 2,
                NumeroCuenta = "PE-002-123456",
                Saldo = 1500.00m,
                Titular = "Maria",
                Moneda = "PEN"
            }
        );

    }



}
