using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusManagementAPI.Data;
using BusManagementAPI.DTOs;
using BusManagementAPI.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BusManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusesController : ControllerBase
    {
        private readonly BusDbContext _context;

        public BusesController(BusDbContext context)
        {
            _context = context;
        }

        [HttpPost("/CreateRoute")]
        public async Task<IActionResult> SaveBusRoute(BusRouteDTO busIP)
        {
            var existingRoute = _context.BusRoutes.Any(x => x.RouteCode.ToLower() == busIP.RouteCode.ToLower() && x.IsActive == true);

            if (existingRoute)
            {
                return BadRequest("Route already exists - " + busIP.RouteCode);
            }

            var busRoute = new Busroute
            {
                RouteID = Guid.NewGuid(),
                RouteCode = busIP.RouteCode,
                StartingPoint = busIP.StartPoint,
                EndingPoint = busIP.EndPoint,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            _context.BusRoutes.Add(busRoute);

            var busRouteId = busRoute.RouteID;

            var busStages = busIP.BusStages;

            if (busStages.Count > 0)
            {
                foreach (var stage in busStages)
                {
                    var busStage = new BusRouteStage
                    {
                        RouteID = busRouteId,
                        StageID = Guid.NewGuid(),
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.Longitude,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.BusRouteStages.Add(busStage);
                }
            }

            var sucess = await _context.SaveChangesAsync() > 0;

            if (sucess)
            {
                return Ok("Route created successfully - " + busIP.RouteCode);
            }

            return BadRequest("Error saving the route, try again later");
        }

        [HttpGet("/GetAllRoutes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            var routes = await _context.BusRoutes
                .Where(x => x.IsActive == true)
                .Include(r => r.BusRouteStages) // Include stages
                .ToListAsync();

            var res = routes.OrderBy(x => x.RouteCode).Select(route => new BusRouteStagesDisplayDTO
            {
                ID = route.RouteID,
                Code = route.RouteCode,
                From = route.StartingPoint,
                To = route.EndingPoint,
                Stages = route.BusRouteStages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StageOrder)
                    .Select(stage => new BusRouteStagesDTO
                    {
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = (int)stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.Longitude
                    }).ToList()
            }).ToList();

            return Ok(res);

        }

        [HttpGet("/GetRoutes")]
        public IActionResult GetRoutes(int page = 1, int pageSize = 10)
        {
            var routes = _context.BusRoutes.OrderBy(x => x.RouteCode).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalRoutes = _context.BusRoutes.Count();
            var totalPages = (int)Math.Ceiling((double)totalRoutes / pageSize);

            var result = new
            {
                routes,
                totalPages
            };

            return Ok(result);
        }

        [HttpGet("/GetRouteByCode/{RouteCode}")]
        public async Task<IActionResult> GetRouteByCode(string RouteCode)
        {
            var route = await _context.BusRoutes
                .Where(x => x.IsActive == true && x.RouteCode.ToLower() == RouteCode.ToLower())
                .Include(r => r.BusRouteStages)
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return Ok("No Active Routes found with code - " + RouteCode);
            }

            var res = new BusRouteStagesDisplayDTO
            {
                ID = route.RouteID,
                Code = route.RouteCode,
                From = route.StartingPoint,
                To = route.EndingPoint,
                Stages = route.BusRouteStages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StageOrder)
                    .Select(stage => new BusRouteStagesDTO
                    {
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = (int)stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.L
                    }).ToList()
            };

            return Ok(res);
        }

        [HttpGet("/GetRouteStagesByCode/{RouteCode}")]
        public async Task<IActionResult> GetRouteStagesByCode(string RouteCode)
        {
            var route = await _context.BusRoutes
                .Where(x => x.IsActive == true && x.RouteCode.ToLower() == RouteCode.ToLower())
                .Include(r => r.BusRouteStages)
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return Ok("No Active Routes found with code - " + RouteCode);
            }

            var res = new BusRouteStagesDisplayDTO
            {
                Stages = route.BusRouteStages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StageOrder)
                    .Select(stage => new BusRouteStagesDTO
                    {
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = (int)stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.Longitude
                    }).ToList()
            };

            return Ok(res);
        }

        [HttpGet("/GetRouteDtlsforBot/{RouteCode}")]
        public async Task<IActionResult> GetRouteDtlsforBot(string RouteCode)
        {
            // Step 1: Fetch required routes
            var routes = _context.BusRoutes
                .Where(a => a.IsActive && a.RouteCode == RouteCode)
                .ToList();

            var routeIds = routes.Select(r => r.RouteID).ToList();

            // Step 2: Fetch stages for those routes
            var stages = _context.BusRouteStages
                .Where(s => routeIds.Contains(s.RouteID))
                .OrderBy(s => s.StageOrder)
                .ToList();

            var stageIds = stages.Select(s => s.StageName).ToList();

            // Step 3: Fetch translations (English only)
            var stageTranslations = _context.StageTranslations
                .Where(t => stageIds.Contains(t.EnglishName) && t.TranslatedLanguage == "Tamil")
                .ToList();

            // Step 4: Build final result with translated names
            var routeDtl = routes.Select(route =>
            {
                var stageList = stages
                    .Where(s => s.RouteID == route.RouteID)
                    .OrderBy(s => s.StageOrder)
                    .ToList();

                // Get intermediate stage IDs
                var intermediateStages = stageList.Skip(1).Take(stageList.Count - 2);

                // Map to translated names
                var viaNames = intermediateStages
                    .Select(stage =>
                        stageTranslations.FirstOrDefault(t => t.EnglishName == stage.StageName)?.TranslatedName ?? stage.StageName
                    );

                return new
                {
                    Route = route.RouteCode,
                    From = stageTranslations.Where(x => x.EnglishName == route.StartingPoint).Select(x => x.TranslatedName).FirstOrDefault(),
                    To = stageTranslations.Where(x => x.EnglishName == route.EndingPoint).Select(x => x.TranslatedName).FirstOrDefault(),
                    Via = string.Join(", ", viaNames)
                };
            }).FirstOrDefault();



            return Ok(routeDtl);
        }

        [HttpGet("/GetRouteByID/{Id}")]
        public async Task<IActionResult> GetRouteByID(Guid Id)
        {
            var route = await _context.BusRoutes
                .Where(x => x.IsActive == true && x.RouteID == Id)
                .Include(r => r.BusRouteStages)
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return Ok("No Active Routes found with id - " + Id);
            }

            var res = new BusRouteStagesDisplayDTO
            {
                ID = route.RouteID,
                Code = route.RouteCode,
                From = route.StartingPoint,
                To = route.EndingPoint,
                Stages = route.BusRouteStages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StageOrder)
                    .Select(stage => new BusRouteStagesDTO
                    {
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = (int)stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.Longitude
                    }).ToList()
            };

            return Ok(res);
        }

        [HttpPut("/UpdateRoute/{id}")]
        public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] BusRouteDTO updatedRoute)
        {
            var existingRoute = await _context.BusRoutes
                .Include(r => r.BusRouteStages)
                .FirstOrDefaultAsync(x => x.RouteID == id && x.IsActive == true);

            if (existingRoute == null)
            {
                return NotFound("No active route found with ID - " + id);
            }

            // Update route details
            existingRoute.RouteCode = updatedRoute.RouteCode;
            existingRoute.StartingPoint = updatedRoute.StartPoint;
            existingRoute.EndingPoint = updatedRoute.EndPoint;
            existingRoute.ModifiedDate = DateTime.Now;

            // Deactivate existing stages
            var existingStages = existingRoute.BusRouteStages.Where(s => s.IsActive).ToList();
            foreach (var stage in existingStages)
            {
                stage.IsActive = false;
                stage.DeletedDate = DateTime.Now;
            }

            // Add new stages
            if (updatedRoute.BusStages != null && updatedRoute.BusStages.Count > 0)
            {
                foreach (var stage in updatedRoute.BusStages)
                {
                    var newStage = new BusRouteStage
                    {
                        StageID = Guid.NewGuid(),
                        RouteID = id,
                        StageName = stage.StageName,
                        StageOrder = stage.StageOrder,
                        DistanceFromStart = stage.DistanceFromStart,
                        Latitude = stage.Latitude,
                        Longitude = stage.Longitude,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    _context.BusRouteStages.Add(newStage);
                }
            }

            var success = await _context.SaveChangesAsync() > 0;

            if (success)
            {
                return Ok("Route and stages updated successfully - " + updatedRoute.RouteCode);
            }

            return BadRequest("Update failed. Try again later.");
        }

        [HttpDelete("/DeleteRouteByID/{id}")]
        public async Task<IActionResult> DeleteRouteByID(Guid id)
        {
            var route = await _context.BusRoutes.Where(x => x.RouteID == id && x.IsActive == true).FirstOrDefaultAsync();

            if (route == null)
            {
                return Ok("Route already been deleted or not found");
            }

            route.IsActive = false;
            route.DeletedDate = DateTime.Now;

            _context.BusRoutes.Update(route);

            var routeStages = await _context.BusRouteStages.Where(x => x.RouteID == id && x.IsActive == true).ToListAsync();

            foreach (var stage in routeStages)
            {
                stage.IsActive = false;
                stage.DeletedDate = DateTime.Now;
            }

            var sucess = await _context.SaveChangesAsync() > 0;
            if (sucess)
            {
                return Ok("Route deleted successfully - " + route.RouteCode);
            }

            return BadRequest("Route cannot be deleted. Try again later");
        }

        [HttpGet("/GetRouteCodes")]
        public async Task<IActionResult> GetRouteCodes()
        {
            return Ok(await _context.BusRoutes.Where(x => x.IsActive == true).OrderBy(x => x.RouteCode).Select(x => x.RouteCode).ToListAsync());
        }

        [HttpGet("/GetRouteStageInfo/{id}")]
        public async Task<IActionResult> GetRouteStageInfo(string id)
        {
            var route = await _context.BusRoutes.Where(x => x.RouteCode == id && x.IsActive == true).Include(x => x.BusRouteStages).FirstOrDefaultAsync();

            var stageCount = route.BusRouteStages.Count;

            return Ok(new { RouteCode = route.RouteCode, StageCount = stageCount });
        }

        // [HttpPost("/AddFareMaster")]
        // public async Task<IActionResult> AddFareMaster(FareMasterDTO fareIP)
        // {
        //     var existingFare = await _context.FareMasters.Where(x => x.BusType.ToLower() == fareIP.BusType.ToLower() && x.IsActive == true).FirstOrDefaultAsync();

        //     if (existingFare != null)
        //     {
        //         existingFare.BaseFare = fareIP.BaseFare;
        //         existingFare.FarePerStage = fareIP.FarePerStage;
        //         existingFare.ModifiedDate = DateTime.Now;

        //         _context.FareMasters.Update(existingFare);
        //         await _context.SaveChangesAsync();
        //     }
        //     else
        //     {
        //         var newFare = new FareMaster
        //         {
        //             FareID = Guid.NewGuid(),
        //             BusType = fareIP.BusType,
        //             BaseFare = fareIP.BaseFare,
        //             FarePerStage = fareIP.FarePerStage,
        //             IsActive = true,
        //             CreatedDate = DateTime.Now
        //         };

        //         _context.FareMasters.Add(newFare);
        //         await _context.SaveChangesAsync();
        //     }

        //     return Ok("Fare Added Successfully");
        // }

        [HttpGet("/CalculateFare/{routeCode}/{busType}/{startStage}/{endStage}")]
        public async Task<IActionResult> CalculateFare(string routeCode, string busType, string startStage, string endStage)
        {
            var route = await _context.BusRoutes
        .Include(r => r.BusRouteStages)
        .FirstOrDefaultAsync(r => r.RouteCode.ToLower() == routeCode.ToLower() && r.IsActive == true);

            if (route == null)
                return NotFound("Route not found or inactive");

            var stages = route.BusRouteStages.Where(s => s.IsActive).ToList();

            var start = stages.Where(s => s.StageName.ToLower() == startStage.ToLower()).FirstOrDefault();
            var end = stages.Where(s => s.StageName.ToLower() == endStage.ToLower()).FirstOrDefault();

            if (start == null || end == null)
                return BadRequest("Invalid stage names");

            var startTranslated = _context.StageTranslations.Where(x => x.EnglishName == start.StageName && x.IsActive == true).Select(x => x.TranslatedName).FirstOrDefault();
            var endTranslated = _context.StageTranslations.Where(x => x.EnglishName == end.StageName && x.IsActive == true).Select(x => x.TranslatedName).FirstOrDefault();

            int stageDifference = Math.Abs(end.StageOrder - start.StageOrder);
            decimal fare = _context.FareMasters_New.Where(x => x.BusType == busType && x.Stage == stageDifference && x.IsActive == true).Select(x => x.StageFare).FirstOrDefault();

            return Ok(new
            {
                RouteCode = route.RouteCode,
                busType = busType,
                From = start.StageName,
                FromTranslated = startTranslated,
                To = end.StageName,
                ToTranslated = endTranslated,
                StagesTravelled = stageDifference,
                Fare = fare
            });
        }

        [HttpPost("/ImportBusRoutes")]
        public async Task<IActionResult> ImportBusRoutes(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok("No file uploaded.");

            var extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".xlsx")
                return Ok("Please upload a valid Excel (.xlsx) file.");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // First worksheet
                    var busRoutes = new List<Busroute>();
                    var busRouteStages = new List<BusRouteStage>();
                    var routeMap = new Dictionary<string, Guid>(); // Map RouteCode to RouteID

                    // Assuming row 1 is header
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        var routeCode = row.Cell(1).GetValue<string>().Trim();
                        var startingPoint = row.Cell(2).GetValue<string>().Trim();
                        var endingPoint = row.Cell(3).GetValue<string>().Trim();
                        var stageName = row.Cell(4).GetValue<string>().Trim();
                        var stageOrder = row.Cell(5).GetValue<string>().Trim();
                        var latitude = row.Cell(6).GetValue<string>().Trim();
                        var longitude = row.Cell(7).GetValue<string>().Trim();

                        if (string.IsNullOrEmpty(routeCode) || string.IsNullOrEmpty(startingPoint)
                            || string.IsNullOrEmpty(endingPoint) || string.IsNullOrEmpty(stageName) || string.IsNullOrEmpty(stageOrder) 
                            || string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
                            continue;

                        if (!routeMap.ContainsKey(routeCode))
                        {
                            var newRoute = new Busroute
                            {
                                RouteID = Guid.NewGuid(),
                                RouteCode = routeCode,
                                StartingPoint = startingPoint,
                                EndingPoint = endingPoint,
                                IsActive = true,
                                CreatedDate = DateTime.Now
                            };
                            busRoutes.Add(newRoute);
                            routeMap[routeCode] = newRoute.RouteID;
                        }

                        var busStage = new BusRouteStage
                        {
                            StageID = Guid.NewGuid(),
                            RouteID = routeMap[routeCode],
                            StageName = stageName,
                            StageOrder = Convert.ToInt32(stageOrder),
                            Latitude = Convert.ToDouble(latitude),
                            Longitude = Convert.ToDouble(longitude),
                            IsActive = true,
                            CreatedDate = DateTime.Now
                        };
                        busRouteStages.Add(busStage);
                    }

                    _context.BusRoutes.AddRange(busRoutes);
                    _context.BusRouteStages.AddRange(busRouteStages);
                    _context.SaveChanges();

                    //return Ok("Routes and stages imported successfully.");
                    return Ok($"{busRoutes.Count} Routes and {busRouteStages.Count} Stages Imported successfully.");
                }
            }
        }

        [HttpPost("/ImportStageTranslations")]
        public async Task<IActionResult> ImportStageTranslations(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok("No file uploaded.");

            var extension = Path.GetExtension(file.FileName);
            if (extension.ToLower() != ".xlsx")
                return Ok("Please upload a valid Excel (.xlsx) file.");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); 
                    var stageTranslations = new List<StageTranslation>();

                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        var englishName = row.Cell(1).GetValue<string>().Trim();
                        var translatedName = row.Cell(2).GetValue<string>().Trim();
                        var translatedLanguage = row.Cell(3).GetValue<string>().Trim();

                        if (string.IsNullOrEmpty(englishName) && string.IsNullOrEmpty(translatedName) && string.IsNullOrEmpty(translatedLanguage))
                            continue; 

                        var translation = new StageTranslation
                        {
                            TranslationId = Guid.NewGuid(),
                            EnglishName = englishName,
                            TranslatedName = translatedName,
                            TranslatedLanguage = translatedLanguage,
                            IsActive = true
                        };

                        stageTranslations.Add(translation);
                    }

                    _context.StageTranslations.AddRange(stageTranslations);
                    _context.SaveChanges();

                    return Ok($"{stageTranslations.Count} translations imported successfully.");
                }
            }
        }


        [HttpGet("/SearchStages/{searchText}")]
        public async Task<IActionResult> SearchStages(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest("Search text is required.");
            }

            // Fetch distinct stage names containing the search text (case insensitive)
            var matchingStages = await _context.BusRouteStages
                .Where(s => s.IsActive && s.StageName.ToLower().Contains(searchText.ToLower()))
                .Select(s => s.StageName)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            if (!matchingStages.Any())
            {
                return Ok("No stages found matching the search text.");
            }

            return Ok(matchingStages);
        }

        [HttpGet("/FindRoutesBetweenStages/{fromStage}/{toStage}")]
        public async Task<IActionResult> FindRoutesBetweenStages(string fromStage, string toStage)
        {
            if (string.IsNullOrWhiteSpace(fromStage) || string.IsNullOrWhiteSpace(toStage))
            {
                return BadRequest("Both 'fromStage' and 'toStage' are required.");
            }

            var routesWithStages = await _context.BusRoutes
                .Where(r => r.IsActive)
                .Include(r => r.BusRouteStages.Where(s => s.IsActive))
                .ToListAsync();

            var matchingRoutes = new List<BusRouteStagesDisplayDTO>();

            foreach (var route in routesWithStages)
            {
                var stages = route.BusRouteStages.OrderBy(s => s.StageOrder).ToList();

                var from = stages.FirstOrDefault(s => s.StageName.Equals(fromStage, StringComparison.OrdinalIgnoreCase));
                var to = stages.FirstOrDefault(s => s.StageName.Equals(toStage, StringComparison.OrdinalIgnoreCase));

                if (from != null && to != null)
                {
                    // Get stage orders regardless of direction
                    int startOrder = Math.Min(from.StageOrder, to.StageOrder);
                    int endOrder = Math.Max(from.StageOrder, to.StageOrder);

                    var filteredStages = stages
                        .Where(s => s.StageOrder >= startOrder && s.StageOrder <= endOrder)
                        .ToList();

                    // If user gave reverse direction, reverse the stage list for clarity
                    if (from.StageOrder > to.StageOrder)
                    {
                        filteredStages.Reverse();
                    }

                    matchingRoutes.Add(new BusRouteStagesDisplayDTO
                    {
                        ID = route.RouteID,
                        Code = route.RouteCode,
                        From = route.StartingPoint,
                        To = route.EndingPoint,
                        Stages = filteredStages
                            .Select(s => new BusRouteStagesDTO
                            {
                                StageName = s.StageName,
                                StageOrder = s.StageOrder,
                                DistanceFromStart = (int)s.DistanceFromStart
                            }).ToList()
                    });
                }
            }

            if (!matchingRoutes.Any())
            {
                return Ok("No routes found between the given stages.");
            }

            return Ok(matchingRoutes.OrderBy(x => x.Code));
        }
    }
}
