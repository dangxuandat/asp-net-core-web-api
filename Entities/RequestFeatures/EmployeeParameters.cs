namespace Entities.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {

        public EmployeeParameters()
        {
            OrderBy = "name";
        }
        public int MinAge { get; set; }
        public int MaxAge { get; set; } = int.MaxValue;

        public bool ValidAgeRange => MaxAge > MinAge;

        public string SearchTerm { get; set; }
    }
}