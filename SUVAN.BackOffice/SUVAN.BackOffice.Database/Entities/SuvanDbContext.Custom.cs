using Microsoft.EntityFrameworkCore;
using static SUVAN.BackOffice.Models.StoredsProcedures.ModelsStoredsProcedures;

namespace SUVAN.BackOffice.Database.Entities;

public partial class SuvanDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModelRutaConfiguracion>(entity =>
        {
            entity.HasNoKey();
        });
    }
}