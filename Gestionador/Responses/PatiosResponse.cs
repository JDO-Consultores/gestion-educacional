using System.Collections.Generic;

namespace Gestionador.Responses
{
    public class PatiosResponse
    {
        public int ID { get; set; }
        public string Patio { get; set; }
        public bool IsActive { get; set; }
        public int CementerioID { get; set; }
        public string Cementerio { get; set; }
        public List<SectoresResponse> Sectores { get; set; } = new List<SectoresResponse>();

        public string Text => Patio;
    }
}