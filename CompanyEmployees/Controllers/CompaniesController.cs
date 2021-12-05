using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace CompanyEmployees
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {

        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies() 
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);

                var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies); // use automap to convert company to company DTO

            
                return Ok(companiesDTO); // return status 200
        }

        [HttpGet("{id}", Name = "CompanyByID")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id : {id} does not exist in the database");
                return NotFound();
            }
            else
            {
                var companyDTO = _mapper.Map<CompanyDTO>(company); 
                return Ok(companyDTO);
            }
        }

        [HttpPost]
        [ServiceFilter((typeof(ValidationFilterAttribute)))]
        public IActionResult CreateCompany([FromBody]CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError("Company for creation Dto object sent from client is null");
                return BadRequest("CompanyForCreationDto object is null");
            }

            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            _repository.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDTO>(companyEntity);

            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var companyEntities = await  _repository.Company.GetByIdsAsync(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var companyToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            return Ok(companyToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null");
                return BadRequest("Company collection is null");
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company;
            
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter((typeof(ValidationFilterAttribute)))]
        [ServiceFilter(typeof(ValidateCompanyExistAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            var companyEntity = HttpContext.Items["company"] as Company;

            _mapper.Map(company, companyEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

    }
}
