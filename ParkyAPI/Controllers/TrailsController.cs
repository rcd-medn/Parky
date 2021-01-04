



using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private ITrailRepository _trailRepository;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepository, IMapper mapper)
        {
            _trailRepository = trailRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Recupera todos os registros de trilhas cadastradas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetTrails()
        {
            var objList = _trailRepository.GetTrails();
            var objDto = new List<TrailDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDTO>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Recupera o registro de uma trilha indicado pelo seu ID
        /// </summary>
        /// <param name="id">ID do registro da trilha</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int id)
        {
            var obj = _trailRepository.GetTrail(id);

            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<TrailDTO>(obj);
            return Ok(objDto);
        }

        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _trailRepository.GetTrailsInNationalPark(nationalParkId);

            if (objList == null)
            {
                return NotFound();
            }

            var objDto = new List<TrailDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDTO>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Registra uma nova trilha no banco de dados.
        /// </summary>
        /// <param name="trailDTO">Informações do novo registro</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDTO trailDTO)
        {
            if (trailDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepository.TrailExist(trailDTO.Name))
            {
                ModelState.AddModelError("", "Essa trilha já existe");
                return StatusCode(404, ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDTO);

            if (!_trailRepository.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao salvar o registro {trailDTO.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { version = HttpContext.GetRequestedApiVersion().ToString(), id = trailObj.Id }, trailObj);
        }

        /// <summary>
        /// Atualiza o registro de uma trilha
        /// </summary>
        /// <param name="trailId">ID do registro a ser atualizado</param>
        /// <param name="trailDTO">Informações da trilha que devem ser atualizadas</param>
        /// <returns></returns>
        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdatetDTO trailDTO)
        {
            if (trailDTO == null || trailId != trailDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDTO);
            if (!_trailRepository.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao tentar atualizar as informações do registro {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Exclusão de registro de uma trilha
        /// </summary>
        /// <param name="trailId">ID da trilha a ser excluído</param>
        /// <returns></returns>
        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExist(trailId))
            {
                return NotFound();
            }

            var trailObj = _trailRepository.GetTrail(trailId);
            if (!_trailRepository.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao deletar o registro {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
