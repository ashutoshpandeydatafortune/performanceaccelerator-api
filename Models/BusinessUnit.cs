using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class BusinessUnit: BaseEntity_PRMS
    {
        public int BusinessUnitId { get; set; }
        public string BusinessUnitName { get; set; }
        public string Remark { get; set; }

        public byte IsActive { get; set; }
        public int ReferenceId { get; set; }
        public int TechFunctionId { get; set; }

        [NotMapped]
        public List<Client> ClientsList { get; set; }
    }

    public class BusinesssUnits
    {
        public int? BusinessUnitId { get; set; }
        public string BusinessUnitName { get; set; }       
        public int? TechFunctionId { get; set; }
        public string FunctionName { get; set; }
        public byte IsActive { get; set; }


    }
}
