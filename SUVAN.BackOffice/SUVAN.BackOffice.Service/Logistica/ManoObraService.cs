using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SUVAN.BackOffice.Service.Logistica
{
    public class ManoObraService : IManoObraService
    {
        private readonly SuvanDbContext _context;
        public ManoObraService(SuvanDbContext context)
        {
            _context = context;
        }
        public async Task<List<ManoObraViewModel>> GetManoObras()
        {
            return await _context.ManoObras
                .AsNoTracking()
                .Select(mo => new ManoObraViewModel
                {
                    IdManoObra = mo.IdManoObra,
                    DescripcionManoobra = mo.DescripcionManoobra,
                    CostoUnitario = mo.CostoUnitario
                })
                .ToListAsync();
        }
        public async Task<ManoObraViewModel> GetManoObra(int idManoObra)
        {
            var entity = await _context.ManoObras
                .Include(mo => mo.ManoObraDetalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(mo => mo.IdManoObra == idManoObra);
            if (entity == null) return new ManoObraViewModel();
            return new ManoObraViewModel
            {
                IdManoObra = entity.IdManoObra,
                DescripcionManoobra = entity.DescripcionManoobra,
                CostoUnitario = entity.CostoUnitario,
                Detalles = entity.ManoObraDetalles.Select(d => new ManoObraDetalleViewModel
                {
                    IdMoDetalle = d.IdMoDetalle,
                    IdManoObra = d.IdManoObra,
                    DescripcionActividad = d.DescripcionActividad,
                    EsObligatorio = d.EsObligatorio == 1
                }).ToList()
            };
        }
        public async Task<int> GuardarManoObraAsync(ManoObraViewModel model, int idUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ManoObra entity;
                if (model.IdManoObra == 0)
                {
                    // Create
                    entity = new ManoObra
                    {
                        DescripcionManoobra = model.DescripcionManoobra,
                        CostoUnitario = model.CostoUnitario,
                        Idusuario = idUsuario,
                        Fecharegistro = DateTime.Now
                    };
                    if (model.Detalles != null && model.Detalles.Any())
                    {
                        foreach (var det in model.Detalles)
                        {
                            entity.ManoObraDetalles.Add(new ManoObraDetalle
                            {
                                DescripcionActividad = det.DescripcionActividad,
                                EsObligatorio = (sbyte)(det.EsObligatorio ? 1 : 0),
                                Idusuario = idUsuario,
                                Fecharegistro = DateTime.Now
                            });
                        }
                    }
                    _context.ManoObras.Add(entity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update
                    entity = await _context.ManoObras
                        .Include(mo => mo.ManoObraDetalles)
                        .FirstOrDefaultAsync(mo => mo.IdManoObra == model.IdManoObra);
                    if (entity == null) throw new Exception("Registro no encontrado.");
                    entity.DescripcionManoobra = model.DescripcionManoobra;
                    entity.CostoUnitario = model.CostoUnitario;
                    entity.Idusuario = idUsuario;
                    entity.Fecharegistro = DateTime.Now;
                    if (model.Detalles == null)
                    {
                        model.Detalles = new List<ManoObraDetalleViewModel>();
                    }
                    // Merge details
                    var detailsToKeep = model.Detalles.Where(d => d.IdMoDetalle > 0).Select(d => d.IdMoDetalle).ToList();

                    // Remove deleted
                    var detailsToRemove = entity.ManoObraDetalles.Where(d => !detailsToKeep.Contains(d.IdMoDetalle)).ToList();
                    _context.ManoObraDetalles.RemoveRange(detailsToRemove);
                    // Update existing and Add new
                    foreach (var det in model.Detalles)
                    {
                        if (det.IdMoDetalle > 0)
                        {
                            var existing = entity.ManoObraDetalles.FirstOrDefault(d => d.IdMoDetalle == det.IdMoDetalle);
                            if (existing != null)
                            {
                                existing.DescripcionActividad = det.DescripcionActividad;
                                existing.EsObligatorio = (sbyte)(det.EsObligatorio ? 1 : 0);
                                existing.Idusuario = idUsuario;
                                existing.Fecharegistro = DateTime.Now;
                            }
                        }
                        else
                        {
                            entity.ManoObraDetalles.Add(new ManoObraDetalle
                            {
                                DescripcionActividad = det.DescripcionActividad,
                                EsObligatorio = (sbyte)(det.EsObligatorio ? 1 : 0),
                                Idusuario = idUsuario,
                                Fecharegistro = DateTime.Now
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                return entity.IdManoObra;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<ManoObraDetalleViewModel>> GetTodasActividades()
        {
            return await _context.ManoObraDetalles
                .Include(d => d.IdManoObraNavigation)
                .AsNoTracking()
                .Select(d => new ManoObraDetalleViewModel
                {
                    IdMoDetalle = d.IdMoDetalle,
                    IdManoObra = d.IdManoObra,
                    DescripcionActividad = d.DescripcionActividad,
                    EsObligatorio = d.EsObligatorio == 1,
                    ServicioPadre = d.IdManoObraNavigation.DescripcionManoobra
                })
                .ToListAsync();
        }
        public async Task<List<ManoObraDetalleViewModel>> GetActividadesPorManoObra(int idManoObra)
        {
            return await _context.ManoObraDetalles
                .AsNoTracking()
                .Where(d => d.IdManoObra == idManoObra)
                .Select(d => new ManoObraDetalleViewModel
                {
                    IdMoDetalle = d.IdMoDetalle,
                    IdManoObra = d.IdManoObra,
                    DescripcionActividad = d.DescripcionActividad,
                    EsObligatorio = d.EsObligatorio == 1
                })
                .ToListAsync();
        }
    }
}
