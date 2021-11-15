using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class CompanyForCreationDto
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required field")]
        public string Address { get; set; }

        public string Country { get; set; }

        public IEnumerable<EmployeeForCreationDto> Employees { get; set; }
    }
}