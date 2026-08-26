using System.Collections.Generic;

namespace Gestionador.Responses
{
    public class SectoresResponse
    {
        public int ID { get; set; }
        public string Sector { get; set; }
        public bool IsActive { get; set; }
        public PatiosResponse Patios { get; set; }
        public List<SeccionConceptosResponse> SeccionConcepto { get; set; } = new List<SeccionConceptosResponse>();
        public string Text => Sector;
    }
}