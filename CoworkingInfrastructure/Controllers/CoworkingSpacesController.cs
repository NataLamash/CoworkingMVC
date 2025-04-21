using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CoworkingDomain.Model;
using CoworkingInfrastructure;
using CoworkingInfrastructure.ViewModels.CoworkingFacilities;
using CoworkingInfrastructure.ViewModels.CoworkingSpaces;
using CoworkingInfrastructure.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingInfrastructure.Controllers
{
    public class CoworkingSpacesController : Controller
    {
        private readonly CoworkingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CoworkingSpacesController(
            CoworkingDbContext context,
            IWebHostEnvironment env
        )
        {
            _context = context;
            _env = env;
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var space = await _context.CoworkingSpaces
                .Include(cs => cs.CoworkingSpaceImages)
                .Include(cs => cs.CoworkingFacilityPrices)
                    .ThenInclude(cfp => cfp.Facility)
                .FirstOrDefaultAsync(cs => cs.Id == id.Value);

            if (space == null) return NotFound();
            return View(space);
        }

        // GET: Index
        public async Task<IActionResult> Index()
            => View(await _context.CoworkingSpaces.ToListAsync());

        // GET: Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            var vm = new CreateCoworkingSpaceViewModel
            {
                Facilities = _context.Facilities
                    .Select(f => new FacilitySelection
                    {
                        FacilityId = f.Id,
                        FacilityName = f.Name,
                        IsSelected = false,
                        Price = null
                    })
                    .ToList()
            };
            return View(vm);
        }

        // POST: Create
        [HttpPost, Authorize(Roles = "admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCoworkingSpaceViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 1) Додаємо CoworkingSpace
            var space = new CoworkingSpace
            {
                Name = vm.Name,
                Location = vm.Location,
                Description = vm.Description,
                Capacity = vm.Capacity,
                HourlyRate = vm.HourlyRate
            };
            _context.CoworkingSpaces.Add(space);
            await _context.SaveChangesAsync(); // space.Id тепер відомий

            // 2) Додаємо фасіліті
            foreach (var sel in vm.Facilities.Where(f => f.IsSelected))
            {
                _context.CoworkingFacilityPrices.Add(new CoworkingFacilityPrice
                {
                    CoworkingSpaceId = space.Id,
                    FacilityId = sel.FacilityId,
                    Price = sel.Price ?? 0m
                });
            }
            await _context.SaveChangesAsync();

            // 3) Додаємо фото
            if (vm.Photos?.Any() == true)
            {
                var uploadFolder = Path.Combine(
                    _env.WebRootPath, "uploads", "spaces", space.Id.ToString()
                );
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in vm.Photos)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var fullPath = Path.Combine(uploadFolder, fileName);

                        using var stream = System.IO.File.Create(fullPath);
                        await file.CopyToAsync(stream);

                        _context.CoworkingSpaceImages.Add(new CoworkingSpaceImage
                        {
                            CoworkingSpaceId = space.Id,
                            FilePath = $"/uploads/spaces/{space.Id}/{fileName}"
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = space.Id });
        }

        // GET: Edit
        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var space = await _context.CoworkingSpaces
                .Include(s => s.CoworkingSpaceImages)
                .Include(s => s.CoworkingFacilityPrices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (space == null) return NotFound();

            var allFacilities = await _context.Facilities.ToListAsync();
            var existingCFPs = space.CoworkingFacilityPrices;

            var vm = new EditCoworkingSpaceViewModel
            {
                Id = space.Id,
                Name = space.Name,
                Location = space.Location,
                Description = space.Description,
                Capacity = space.Capacity,
                HourlyRate = space.HourlyRate,
                ExistingPhotos = space.CoworkingSpaceImages
                                     .Select(img => new PhotoItem
                                     {
                                         Id = img.Id,
                                         FilePath = img.FilePath
                                     })
                                     .ToList(),
                Facilities = allFacilities.Select(f =>
                {
                    var ex = existingCFPs.FirstOrDefault(x => x.FacilityId == f.Id);
                    return new FacilitySelection
                    {
                        FacilityId = f.Id,
                        FacilityName = f.Name,
                        IsSelected = ex != null,
                        Price = ex?.Price
                    };
                }).ToList()
            };
            return View(vm);
        }

        // POST: Edit
        [HttpPost, Authorize(Roles = "admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCoworkingSpaceViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 1) Оновлюємо простір
            var space = await _context.CoworkingSpaces
                .Include(s => s.CoworkingSpaceImages)
                .Include(s => s.CoworkingFacilityPrices)
                .FirstOrDefaultAsync(s => s.Id == vm.Id);

            if (space == null) return NotFound();

            space.Name = vm.Name;
            space.Location = vm.Location;
            space.Description = vm.Description;
            space.Capacity = vm.Capacity;
            space.HourlyRate = vm.HourlyRate;
            _context.Update(space);
            await _context.SaveChangesAsync();

            // 2) Оновлюємо фасіліті
            var existingCFPs = space.CoworkingFacilityPrices.ToList();
            foreach (var sel in vm.Facilities)
            {
                var existing = existingCFPs.FirstOrDefault(x => x.FacilityId == sel.FacilityId);
                if (sel.IsSelected)
                {
                    if (existing == null)
                    {
                        _context.CoworkingFacilityPrices.Add(new CoworkingFacilityPrice
                        {
                            CoworkingSpaceId = space.Id,
                            FacilityId = sel.FacilityId,
                            Price = sel.Price ?? 0m
                        });
                    }
                    else
                    {
                        existing.Price = sel.Price ?? 0m;
                    }
                }
                else if (existing != null)
                {
                    _context.CoworkingFacilityPrices.Remove(existing);
                }
            }
            await _context.SaveChangesAsync();

            // 3) Видаляємо позначені фото
            foreach (var pic in vm.ExistingPhotos.Where(p => p.Remove))
            {
                var img = await _context.CoworkingSpaceImages.FindAsync(pic.Id);
                if (img != null)
                {
                    var physicalPath = Path.Combine(
                        _env.WebRootPath,
                        img.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                    );
                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);

                    _context.CoworkingSpaceImages.Remove(img);
                }
            }
            await _context.SaveChangesAsync();

            // 4) Додаємо нові фото
            if (vm.NewPhotos?.Any() == true)
            {
                var uploadFolder = Path.Combine(
                    _env.WebRootPath, "uploads", "spaces", space.Id.ToString()
                );
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in vm.NewPhotos.Where(f => f.Length > 0))
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var fullPath = Path.Combine(uploadFolder, fileName);

                    using var stream = System.IO.File.Create(fullPath);
                    await file.CopyToAsync(stream);

                    _context.CoworkingSpaceImages.Add(new CoworkingSpaceImage
                    {
                        CoworkingSpaceId = space.Id,
                        FilePath = $"/uploads/spaces/{space.Id}/{fileName}"
                    });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = space.Id });
        }

        // GET: Delete
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var space = await _context.CoworkingSpaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null) return NotFound();

            return View(space);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete"), Authorize(Roles = "admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var space = await _context.CoworkingSpaces.FindAsync(id);
            if (space != null)
                _context.CoworkingSpaces.Remove(space);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoworkingSpaceExists(int id)
            => _context.CoworkingSpaces.Any(e => e.Id == id);

        // GET: Import form
        [HttpGet, Authorize(Roles = "admin")]
        public IActionResult Import()
            => View();

        // POST: Import Excel
        [HttpPost, Authorize(Roles = "admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Будь ласка, оберіть Excel‑файл.");
                return View();
            }

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet(1);
            var rows = sheet.RangeUsed().RowsUsed().Skip(1);

            var errors = new List<string>();
            int importedCount = 0;

            foreach (var row in rows)
            {
                try
                {
                    // 1) Основні поля
                    var space = new CoworkingSpace
                    {
                        Name = row.Cell(1).GetString(),
                        Location = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        Capacity = row.Cell(4).GetValue<int>(),
                        HourlyRate = row.Cell(5).GetValue<decimal>()
                    };
                    _context.CoworkingSpaces.Add(space);
                    await _context.SaveChangesAsync();

                    // 2) Facilities
                    var facs = row.Cell(6).GetString()
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var token in facs)
                    {
                        var parts = token.Split(':', 2);
                        var fname = parts[0].Trim();
                        var price = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
                        var facility = await _context.Facilities
                            .FirstOrDefaultAsync(f => f.Name == fname);

                        if (facility != null)
                        {
                            _context.CoworkingFacilityPrices.Add(new CoworkingFacilityPrice
                            {
                                CoworkingSpaceId = space.Id,
                                FacilityId = facility.Id,
                                Price = price
                            });
                        }
                    }
                    await _context.SaveChangesAsync();

                    // 3) Photos
                    var photoNames = row.Cell(7).GetString()
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var importFolder = Path.Combine(_env.WebRootPath, "photos");
                    var targetFolder = Path.Combine(
                        _env.WebRootPath, "uploads", "spaces", space.Id.ToString()
                    );
                    Directory.CreateDirectory(targetFolder);

                    foreach (var fn in photoNames)
                    {
                        var src = Path.Combine(importFolder, fn.Trim());
                        if (System.IO.File.Exists(src))
                        {
                            var destFile = $"{Guid.NewGuid()}{Path.GetExtension(fn)}";
                            var dest = Path.Combine(targetFolder, destFile);
                            System.IO.File.Copy(src, dest);

                            _context.CoworkingSpaceImages.Add(new CoworkingSpaceImage
                            {
                                CoworkingSpaceId = space.Id,
                                FilePath = $"/uploads/spaces/{space.Id}/{destFile}"
                            });
                        }
                    }
                    await _context.SaveChangesAsync();

                    importedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                }
            }

            ViewBag.ImportedCount = importedCount;
            ViewBag.Errors = errors;
            return View("ImportResult");
        }
    }
}
