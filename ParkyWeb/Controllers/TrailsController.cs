



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly ITrailRepository _trailRepository;

        public TrailsController(ITrailRepository trailRepository, INationalParkRepository nationalParkRepository)
        {
            _nationalParkRepository = nationalParkRepository;
            _trailRepository = trailRepository;
        }

        [HttpGet]
        public IActionResult Index() => View(new Trail() { });

        [HttpGet]
        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _trailRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWToken")) });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));

            TrailsVM trailsVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                Trail = new Trail()
            };

            if (id == null)
            {
                return View(trailsVM);
            }

            trailsVM.Trail = await _trailRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (trailsVM.Trail == null)
            {
                return NotFound();
            }

            return View(trailsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM trailsVM)
        {
            if (ModelState.IsValid)
            {
                if (trailsVM.Trail.Id == 0)
                {
                    await _trailRepository.CreateAsync(SD.TrailAPIPath, trailsVM.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailRepository.UpdateAsync(SD.TrailAPIPath + trailsVM.Trail.Id, trailsVM.Trail, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));
                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                    Trail = trailsVM.Trail
                };

                return View(objVM);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepository.DeleteAsync(SD.TrailAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new { success = true, message = "Registro de trilha removido do banco de dados com sucesso!" });
            }

            return Json(new { success = false, message = "Não foi possível remover o registro do banco de dados." });
        }
    }
}
