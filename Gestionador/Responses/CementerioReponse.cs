using System.Collections.Generic;

namespace Gestionador.Responses
{
    public class CementerioReponse
    {
        public int ID { get; set; }
        public string Cementerio { get; set; }
        public bool IsActive { get; set; }
        public List<PatiosResponse> Patios { get; set; }
    }
}