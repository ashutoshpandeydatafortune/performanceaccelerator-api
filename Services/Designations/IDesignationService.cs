using DF_EvolutionAPI.Models;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services.Designations
{
    public interface IDesignationService
    {
        public List<Resource> GetResourcesByDesignationName(string designation);
        public Designation GetDesignationDetailsByDesignationName(string designation);
    }
}
