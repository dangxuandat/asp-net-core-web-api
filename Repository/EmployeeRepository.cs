using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployees.Extensions;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository;


namespace Contracts
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {


        public EmployeeRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            
        }


        public async Task<PageList<Employee>> GetEmployeesAsync(Guid companyId,
            EmployeeParameters employeeParameters, bool trackChanges)
        {
            
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId) 
                                                       && (e.Age >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge)
                    , trackChanges)
                .FilterEmployees(employeeParameters.MinAge,employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .OrderBy(e => e.Name)
                .ToListAsync();
            return PageList<Employee>.ToPagedList(employees, employeeParameters.PageNumber,
                employeeParameters.PageSize);
        }


        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) => Delete(employee);
    }
}