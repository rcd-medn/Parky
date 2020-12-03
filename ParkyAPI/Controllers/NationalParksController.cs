



using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private INationalParkRepository _nationalParkRepository;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            _nationalParkRepository = nationalParkRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _nationalParkRepository.GetNationalParks();
            var objDto = new List<NationalParkDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDTO>(obj));
            }

            return Ok(objDto);
        }

        [HttpGet("{id:int}", Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int id)
        {
            var obj = _nationalParkRepository.GetNationalPark(id);

            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<NationalParkDTO>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody]NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_nationalParkRepository.NationalParkExist(nationalParkDTO.Name))
            {
                ModelState.AddModelError("", "Esse parque já existe");
                return StatusCode(404, ModelState);
            }

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);

            if (!_nationalParkRepository.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao salvar o registro {nationalParkDTO.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { id = nationalParkObj.Id }, nationalParkObj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkId != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);
            if (!_nationalParkRepository.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao tentar atualizar as informações do registro {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNtionalPark(int nationalParkId)
        {
            if (!_nationalParkRepository.NationalParkExist(nationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _nationalParkRepository.GetNationalPark(nationalParkId);
            if (!_nationalParkRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao deletar o registro {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
